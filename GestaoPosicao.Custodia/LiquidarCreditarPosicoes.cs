using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Custodia.Dtos;
using GestaoPosicao.Custodia.Modelos;

namespace GestaoPosicao.Custodia;

public class LiquidarCreditarPosicoes : IUseCase
{
    readonly OperacaoRegras _regras;
    readonly Posicoes _posicoes;
    readonly PosicoesAtividades _posicoesAtividades;
    readonly DtoLiquidacaoReq _dto;

    public LiquidarCreditarPosicoes(Bag<DtoLiquidacaoReq, DtoLiquidacaoResp> bag)
    {
        _dto = bag.Request;
        _ = bag.TryGet(LiquidarBag.OperacaoRegras, out _regras);
        _ = bag.TryGet(LiquidarBag.Posicoes, out _posicoes);
        _ = bag.TryGet(LiquidarBag.PosicoesAtividades, out _posicoesAtividades);
    }
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        //  Finaliza se não não for um dos comandos aptos a criação de posições
        if (_regras.CustodiaComando is not (EnumCustodiaComando.Criar or EnumCustodiaComando.Movimentar))
            return Task.FromResult(Result.GoSuccess());

        _ = CreditarPosicao(_regras.Parte);
        _ = CreditarPosicao(_regras.Contraparte);
        return Task.FromResult(Result.GoSuccess());
    }

    Task<Result> CreditarPosicao(OperacaoCaracteristicasParte? parte)
    {
        if (parte is null || parte.MovimentoSentido != EnumMovimentoSentido.CREDITO)
            return Task.FromResult(Result.GoSuccess());

        //  Registra a nova posição
        var posicao = new Posicao(
            Conta: parte.IndicadorParte == EnumOperacaoIndicadorParte.PARTE ? _dto.ContaParte : _dto.ContaContraparte ?? 0,
            Titulo: _dto.Titulo,
            DataRef: DateOnly.FromDateTime(DateTime.Now),
            PURef: _dto.PURef ?? 0,
            TaxaRef: _dto.TaxaRef ?? 0,
            Quantidade: _dto.Quantidade
        );
        _posicoes.Add(posicao);
        _posicoesAtividades.Add(new PosicaoAtividade(
            IdPosicao: posicao.IdPosicao,
            IdOperacao: _dto.IdOperacao,
            Quantidade: _dto.Quantidade
        ));

        return Task.FromResult(Result.GoSuccess());
    }
}
