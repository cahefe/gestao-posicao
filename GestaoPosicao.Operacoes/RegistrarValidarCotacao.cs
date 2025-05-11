using cahefe.UseCases;
using GestaoPosicao.Operacoes.Dtos;
using GestaoPosicao.Operacoes.Modelos;

namespace GestaoPosicao.Operacoes;

public class RegistrarValidarCotacao(Bag<DtoOperacao, Operacao> bag) : IUseCase
{
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        var dtoOperacao = bag.Request;

        if (dtoOperacao.Titulo <= 0)
        {
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_ATIVO_INVALIDO", $"Ativo ({dtoOperacao.Titulo}) inválido", FailLevel.Error)));
        }
        if(dtoOperacao.PURef <= 0)
        {
            return Task.FromResult(Result.GoFails(new Fail(-2, "OPERACAO_PU_INVALIDO", $"PU Ativo ({dtoOperacao.PURef}) inválido", FailLevel.Error)));
        }

        return Task.FromResult(Result.GoSuccess());
    }
}
