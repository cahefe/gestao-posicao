using cahefe.UseCases;
using GestaoPosicao.Compartilhados;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Operacoes.Dtos;
using GestaoPosicao.Operacoes.Modelos;

namespace GestaoPosicao.Operacoes;

public class RegistrarObterNumeroOperacao(Bag<DtoOperacao, Operacao> bag) : IUseCase
{
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        var dtoOperacao = bag.Request;
        if (!bag.TryGet<OperacaoRegras>(RegistrarItensBag.OperacaoRegras, out var regra))
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_REGRAS_NAO_ENCONTRADA", $"Regras para operação {dtoOperacao.TipoOperacao} não encontradas", FailLevel.Error)));

        var numeroOperacao = Numerador.Obter(regra.Tipo);

        bag.Add(RegistrarItensBag.NumeroOperacao, numeroOperacao);
        return Task.FromResult(Result.GoSuccess());
    }
}
