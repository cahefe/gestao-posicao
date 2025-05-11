using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Operacoes.Dtos;
using GestaoPosicao.Operacoes.Modelos;

namespace GestaoPosicao.Operacoes;

public class RegistrarValidarPartes(Bag<DtoOperacao, Operacao> bag) : IUseCase
{
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        var dtoOperacao = bag.Request;
        if (!bag.TryGet<OperacaoRegras>(RegistrarItensBag.OperacaoRegras, out var regra))
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_REGRAS_NAO_ENCONTRADA", $"Regras para operação {dtoOperacao.TipoOperacao} não encontradas", FailLevel.Error)));

        var pontaOrigem = regra.Parte;
        var pontaDestino = regra.Contraparte;

        //  Validar informações de Parte
        if (pontaOrigem.ComportamentoParte == EnumOperacaoComportamentoParte.DECLARADA && dtoOperacao.ContaParte <= 0)
        {
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_PARTE_CONTA_INVALIDA", $"Conta parte ({dtoOperacao.ContaParte}) não informada", FailLevel.Error)));
        }
        if (pontaOrigem.ComportamentoParte == EnumOperacaoComportamentoParte.PREDEFINIDA && dtoOperacao.ContaParte != null)
        {
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_PARTE_CONTA_INVALIDA", $"Conta parte ({dtoOperacao.ContaParte}) informada indevidamente", FailLevel.Error)));
        }

        //  Validar informações de Contraparte (somente se for aplicável)
        if (pontaDestino is not null && pontaDestino.ComportamentoParte == EnumOperacaoComportamentoParte.DECLARADA && dtoOperacao.ContaContraparte <= 0)
        {
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_CONTRAPARTE_CONTA_INVALIDA", $"Conta contraparte ({dtoOperacao.ContaContraparte}) não informada", FailLevel.Error)));
        }
        if (pontaDestino is not null && pontaDestino.ComportamentoParte == EnumOperacaoComportamentoParte.PREDEFINIDA && dtoOperacao.ContaContraparte != null)
        {
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_CONTRAPARTE_CONTA_INVALIDA", $"Conta parte ({dtoOperacao.ContaParte}) informada indevidamente", FailLevel.Error)));
        }
        return Task.FromResult(Result.GoSuccess());
    }
}
