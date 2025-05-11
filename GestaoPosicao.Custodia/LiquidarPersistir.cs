using cahefe.UseCases;
using GestaoPosicao.Custodia.Dtos;
using GestaoPosicao.Custodia.Modelos;

namespace GestaoPosicao.Custodia;

public class LiquidarPersistir(Bag<DtoLiquidacaoReq, DtoLiquidacaoResp> bag, Posicoes posicoes, PosicoesAtividades posicoesAtividades, Alocacoes alocacoes, AlocacoesAtividades alocacoesAtividades, Movimentos movimentos) : IUseCase
{
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {

        _ = bag.TryGet<Posicoes>(LiquidarBag.Posicoes, out var posicoesOperacao);
        _ = bag.TryGet<PosicoesAtividades>(LiquidarBag.PosicoesAtividades, out var posicoesAtividadesOperacao);
        _ = bag.TryGet<Alocacoes>(LiquidarBag.Alocacoes, out var alocacoesOperacao);
        _ = bag.TryGet<AlocacoesAtividades>(LiquidarBag.AlocacoesAtividades, out var alocacoesAtividadesOperacao);
        _ = bag.TryGet<Movimentos>(LiquidarBag.Movimentos, out var movimentosOperacao);

        if (posicoesOperacao.Any())
            posicoes.AddRange(posicoesOperacao);
        if (posicoesAtividadesOperacao.Any())
            posicoesAtividades.AddRange(posicoesAtividadesOperacao);
        if (alocacoesOperacao.Any())
            alocacoes.AddRange(alocacoesOperacao);
        if (alocacoesAtividadesOperacao.Any())
            alocacoesAtividades.AddRange(alocacoesAtividadesOperacao);
        if (movimentosOperacao.Any())
            movimentos.AddRange(movimentosOperacao);

        bag.Response = new DtoLiquidacaoResp(bag.Request.IdOperacao, Enumeradores.EnumSituacaoMovimentacao.Liquidada);

        return Task.FromResult(Result.GoSuccess());
    }
}
