using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Operacoes.Dtos;
using GestaoPosicao.Operacoes.Modelos;

namespace GestaoPosicao.Operacoes;

public class RegistrarPrepararPipeline(Bag<DtoOperacao, Operacao> bag, IList<OperacaoRegras> operacaoRegras) : IUseCase
{
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        var dtoOperacao = bag.Request;
        var regra = operacaoRegras.FirstOrDefault(r => r.Tipo == dtoOperacao.TipoOperacao);

        if (regra is null)
        {
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_REGRAS_NAO_ENCONTRADA", $"Regras para operação {dtoOperacao.TipoOperacao} não encontradas", FailLevel.Error)));
        }
        //  Persiste a regra para os próximos passos do pipeline
        bag.Add(RegistrarItensBag.OperacaoRegras, regra);
        return Task.FromResult(Result.GoSuccess());
    }
}
