using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Custodia.Dtos;
using GestaoPosicao.Custodia.Modelos;

namespace GestaoPosicao.Custodia;

public class LiquidarManterMovimentos : IUseCase
{
    readonly OperacaoRegras _regras;
    readonly DtoLiquidacaoReq _dto;
    readonly Movimentos _movimentos;

    public LiquidarManterMovimentos(Bag<DtoLiquidacaoReq, DtoLiquidacaoResp> bag)
    {
        _dto = bag.Request;
        _ = bag.TryGet(LiquidarBag.OperacaoRegras, out _regras);
        _ = bag.TryGet(LiquidarBag.Movimentos, out _movimentos);
    }

    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        _ = MovimentarParte(_regras.Parte);
        _ = MovimentarParte(_regras.Contraparte);
        return Task.FromResult(Result.GoSuccess());
    }

    Task<Result> MovimentarParte(OperacaoCaracteristicasParte? parte)
    {
        if (parte is null || parte.MovimentoSentido == EnumMovimentoSentido.NAO_APLICAVEL)
            return Task.FromResult(Result.GoSuccess());

        //  Se o movimento for de crédito, cria a posição e registra na Bag
        _movimentos.Add(new Movimento(
            TipoOperacao: _dto.TipoOperacao,
            IdOperacao: _dto.IdOperacao,
            MovimentoTipo: parte.MovimentoTipo,
            ContaParte: parte.IndicadorParte == EnumOperacaoIndicadorParte.PARTE ? _dto.ContaParte : _dto.ContaContraparte ?? 0,
            CarteiraParte: parte.IndicadorParte == EnumOperacaoIndicadorParte.PARTE ? _dto.CarteiraParte : _dto.CarteiraContraparte ?? 0,
            Titulo: _dto.Titulo,
            PURef: _dto.PURef,
            TaxaRef: _dto.TaxaRef,
            Quantidade: _dto.Quantidade,
            ValorTotal: _dto.ValorTotal
        ));
        return Task.FromResult(Result.GoSuccess());
    }
}
