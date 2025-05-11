using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Operacoes.Dtos;
using GestaoPosicao.Operacoes.Modelos;

namespace GestaoPosicao.Operacoes;

public class RegistrarValidarQuantidade(Bag<DtoOperacao, Operacao> bag, IList<OperacaoRegras> operacaoRegras) : IUseCase
{
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        var dtoOperacao = bag.Request;
        var regra = operacaoRegras.First(r => r.Tipo == dtoOperacao.TipoOperacao);

        //  Validar se a quantidade mínima foi informada corretamente
        if (regra.QuantidadeMinima == EnumOperacaoQuantidadeMinima.MAIOR_QUE_ZERO && dtoOperacao.Quantidade <= 0)
        {
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_QUANTIDADE_MINIMA_INVALIDA", $"Quantidade mínima ({dtoOperacao.Quantidade}) inválida", FailLevel.Error)));
        }
        if(regra.QuantidadeMinima == EnumOperacaoQuantidadeMinima.PREDEFINIDA && dtoOperacao.Quantidade < regra.QuantidadeMinimaPredefinida)
        {
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_QUANTIDADE_MINIMA_INVALIDA", $"Quantidade mínima ({dtoOperacao.Quantidade}) inválida - prédefinida: {regra.QuantidadeMinimaPredefinida}", FailLevel.Error)));
        }

        //  Validar se há estouro no limite de decimais informado
        decimal restoQuantidade = dtoOperacao.Quantidade % (decimal)Math.Pow(10, -regra.LimiteDecimaisQuantidade);
        if (restoQuantidade != 0)
        {
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_QUANTIDADE_ESTOURO_FRACAO", $"Quantidade ({dtoOperacao.Quantidade}) inválida: fração mínima ({Math.Pow(10, -regra.LimiteDecimaisQuantidade)})", FailLevel.Error)));
        }
        return Task.FromResult(Result.GoSuccess());
    }
}
