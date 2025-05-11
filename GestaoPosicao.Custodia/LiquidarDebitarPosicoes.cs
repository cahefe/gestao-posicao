using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Custodia.Dtos;
using GestaoPosicao.Custodia.Modelos;

namespace GestaoPosicao.Custodia;

public class LiquidarDebitarPosicoes : IUseCase
{
    readonly OperacaoRegras _regras;
    readonly Posicoes _repoPosicoes;
    readonly PosicoesAtividades _posicoesAtividades;
    readonly DtoLiquidacaoReq _dto;

    public LiquidarDebitarPosicoes(Bag<DtoLiquidacaoReq, DtoLiquidacaoResp> bag, Posicoes repoPosicoes)
    {
        _dto = bag.Request;
        _repoPosicoes = repoPosicoes;
        _ = bag.TryGet(LiquidarBag.OperacaoRegras, out _regras);
        _ = bag.TryGet(LiquidarBag.PosicoesAtividades, out _posicoesAtividades);
    }

    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        //  Finaliza se não não for um dos comandos aptos a criação de posições
        if (_regras.CustodiaComando is not (EnumCustodiaComando.Movimentar or EnumCustodiaComando.Extinguir))
            return Task.FromResult(Result.GoSuccess());

        _ = DebitarPosicao(_regras.Parte);
        _ = DebitarPosicao(_regras.Contraparte);

        return Task.FromResult(Result.GoSuccess());
    }
    Task<Result> DebitarPosicao(OperacaoCaracteristicasParte? parte)
    {
        //  Condições para encerrar execução
        if (parte is null || parte.MovimentoSentido != EnumMovimentoSentido.DEBITO || _regras.CustodiaAlgoritmoDebito is EnumCustodiaAlgoritmoDebito.NAO_APLICAVEL or EnumCustodiaAlgoritmoDebito.ALOCACAO)
            return Task.FromResult(Result.GoSuccess());

        //  Avalia o tipo de algorítimo de débito a utilizar...
        var conta = parte.IndicadorParte == EnumOperacaoIndicadorParte.PARTE ? _dto.ContaParte : _dto.ContaContraparte ?? 0;

        var posicoesSelecionadas = _repoPosicoes
            .Where(p => p.Conta == conta && p.Titulo == _dto.Titulo && p.Quantidade > 0m)
            .OrderBy(p => _regras.CustodiaAlgoritmoDebito is EnumCustodiaAlgoritmoDebito.POSICAO_FIFO or EnumCustodiaAlgoritmoDebito.POSICAO_FIFO_PROPORCIONAL ? p.IdPosicao : -p.IdPosicao);

        if (_regras.CustodiaAlgoritmoDebito is EnumCustodiaAlgoritmoDebito.POSICAO_FIFO or EnumCustodiaAlgoritmoDebito.POSICAO_LIFO)
            DebitarSequencial(posicoesSelecionadas);
        else
            DebitarProporcional(posicoesSelecionadas);

        return Task.FromResult(Result.GoSuccess());
    }

    void DebitarSequencial(IOrderedEnumerable<Posicao> posicoes)
    {
        var quantidadeRestante = _dto.Quantidade;
        foreach (var posicao in posicoes)
        {
            var quantidadeConsumidaPosicao = posicao.Quantidade <= quantidadeRestante ? posicao.Quantidade : quantidadeRestante;
            posicao.Quantidade -= quantidadeConsumidaPosicao;
            quantidadeRestante -= quantidadeConsumidaPosicao;
            _posicoesAtividades.Add(new(
                IdPosicao: posicao.IdPosicao,
                IdOperacao: _dto.IdOperacao,
                Quantidade: -quantidadeConsumidaPosicao
            ));
            //  Finaliza consumo posição a quantidade restante a consumir
            if (quantidadeRestante == 0)
                continue;
        }
    }
    void DebitarProporcional(IOrderedEnumerable<Posicao> posicoes)
    {
        var casasDecimais = _regras.LimiteDecimaisQuantidade;
        //  1) Identificando a quantidade total disponível nas posições origem
        var quantidadeTotalOrigem = posicoes.Sum(p => p.Quantidade);
        //  2) Calculando a razão da proporção entre a quantidade desejada e a quantidade total na origem (buscando consumir qualquer quantidade em todas as posições).
        //  Se não for usado um cálculo proporcional então a razão é sempre 1
        var razao = _dto.Quantidade / quantidadeTotalOrigem;
        //  3) Consome posições valorizadas proporcionalmente
        decimal quantidadeResiduoPosicao = 0;

        foreach (var posicao in posicoes)
        {
            //  3.1) Calcula a quantidade sem arredondamentos
            var quantidadeProporcionalPosicao = (posicao.Quantidade + quantidadeResiduoPosicao) * razao;
            //  3.2) Calcula a quantidade que "pode" ser consumida (a partir do arredondamento)
            var quantidadeConsumidaPosicao = decimal.Round(quantidadeProporcionalPosicao, casasDecimais);
            //  3.3) Calcula o resíduo entre as quantidades proporcional e a quantidade consumida
            quantidadeResiduoPosicao = quantidadeProporcionalPosicao - quantidadeConsumidaPosicao;
            //  3.4) Se existir uma quantidade consumida (>0) então debita da origem
            if (quantidadeConsumidaPosicao > 0)
            {
                _posicoesAtividades.Add(new(
                    IdPosicao: posicao.IdPosicao,
                    IdOperacao: _dto.IdOperacao,
                    Quantidade: quantidadeConsumidaPosicao
                ));
                posicao.Quantidade -= quantidadeConsumidaPosicao;
            }
        }
    }
}
