using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Custodia.Dtos;
using GestaoPosicao.Custodia.Modelos;

namespace GestaoPosicao.Custodia;

public class LiquidarPrepararPipeline(Bag<DtoLiquidacaoReq, DtoLiquidacaoResp> bag, IList<OperacaoRegras> operacaoRegras) : IUseCase
{
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        var dtoMovimentacao = bag.Request;
        var regra = operacaoRegras.FirstOrDefault(r => r.Tipo == dtoMovimentacao.TipoOperacao);

        if (regra is null)
            return Task.FromResult(Result.GoFails(new Fail(-1, "LIQUIDAR_REGRAS_NAO_ENCONTRADA", $"Regras de liquidação para a operação {dtoMovimentacao.TipoOperacao} não encontradas", FailLevel.Error)));

        //  Persiste a regra para os próximos passos do pipeline
        bag.Add(LiquidarBag.OperacaoRegras, regra);
        bag.Add(LiquidarBag.Posicoes, new Posicoes());
        bag.Add(LiquidarBag.PosicoesAtividades, new PosicoesAtividades());
        bag.Add(LiquidarBag.Alocacoes, new Alocacoes());
        bag.Add(LiquidarBag.AlocacoesAtividades, new AlocacoesAtividades());
        bag.Add(LiquidarBag.Movimentos, new Movimentos());

        return Task.FromResult(Result.GoSuccess());
    }
}
