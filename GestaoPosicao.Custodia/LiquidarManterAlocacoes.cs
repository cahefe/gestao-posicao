using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Custodia.Dtos;
using GestaoPosicao.Custodia.Modelos;

namespace GestaoPosicao.Custodia;

public class LiquidarManterAlocacoes : IUseCase
{
    readonly OperacaoRegras _regras;
    readonly Alocacoes _alocacoes;
    readonly AlocacoesAtividades _alocacoesAtividades;
    readonly Alocacoes _repoAlocacoes;
    readonly DtoLiquidacaoReq _dto;
    public LiquidarManterAlocacoes(Bag<DtoLiquidacaoReq, DtoLiquidacaoResp> bag, Alocacoes repoAlocacoes)
    {
        _repoAlocacoes = repoAlocacoes;
        _dto = bag.Request;
        _ = bag.TryGet(LiquidarBag.OperacaoRegras, out _regras);
        _ = bag.TryGet(LiquidarBag.Alocacoes, out _alocacoes);
        _ = bag.TryGet(LiquidarBag.AlocacoesAtividades, out _alocacoesAtividades);
    }
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        //  Finaliza se não não for um dos comandos aptos a criação de posições
        if (_regras.CustodiaComando is not (EnumCustodiaComando.Criar or EnumCustodiaComando.Movimentar or EnumCustodiaComando.Extinguir))
            return Task.FromResult(Result.GoSuccess());

        _ = ManterAlocacao(_regras.Parte);
        _ = ManterAlocacao(_regras.Contraparte);

        return Task.FromResult(Result.GoSuccess());
    }

    Task<Result> ManterAlocacao(OperacaoCaracteristicasParte? parte)
    {
        //  Condições para encerrar execução
        if (parte is null || parte.MovimentoSentido == EnumMovimentoSentido.NAO_APLICAVEL)
            return Task.FromResult(Result.GoSuccess());

        var quantidade = parte.MovimentoSentido == EnumMovimentoSentido.CREDITO ? _dto.Quantidade : -_dto.Quantidade;

        //  Identificar se já existe alocação para o título e conta
        var alocacao = _repoAlocacoes.FirstOrDefault(a => a.Conta == (parte.IndicadorParte == EnumOperacaoIndicadorParte.PARTE ? _dto.ContaParte : _dto.ContaContraparte ?? 0) && a.Titulo == _dto.Titulo);

        //  Se já existir alocação para a conta e título, atualiza a quantidade
        if (alocacao is not null)
            alocacao.Quantidade += quantidade;
        //  Se não, cria nova alocação
        else
        {
            alocacao = new Alocacao(
                Conta: parte.IndicadorParte == EnumOperacaoIndicadorParte.PARTE ? _dto.ContaParte : _dto.ContaContraparte ?? 0,
                Carteira: parte.Carteira,
                Titulo: _dto.Titulo,
                Quantidade: quantidade
            );
            _alocacoes.Add(alocacao);
        }
        _alocacoesAtividades.Add(new AlocacaoAtividade(
            IdOperacao: _dto.IdOperacao,
            IdAlocacao: alocacao.IdAlocacao,
            Quantidade: quantidade
        ));
        return Task.FromResult(Result.GoSuccess());
    }
}
