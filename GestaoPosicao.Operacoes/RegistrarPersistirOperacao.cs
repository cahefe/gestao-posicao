using cahefe.UseCases;
using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Compartilhados.Modelos;
using GestaoPosicao.Operacoes.Dtos;
using GestaoPosicao.Operacoes.Modelos;

namespace GestaoPosicao.Operacoes;

public class RegistrarPersistirOperacao(Bag<DtoOperacao, Operacao> bag, Modelos.Operacoes listaOperacoes) : IUseCase
{
    public Task<Result> Execute(CancellationToken cancellationToken = default)
    {
        var dtoOperacao = bag.Request;
        if (!bag.TryGet<OperacaoRegras>(RegistrarItensBag.OperacaoRegras, out var regra))
            return Task.FromResult(Result.GoFails(new Fail(-1, "OPERACAO_REGRAS_NAO_ENCONTRADA", $"Regras para operação {dtoOperacao.TipoOperacao} não encontradas", FailLevel.Error)));

        if (!bag.TryGet<int>(RegistrarItensBag.NumeroOperacao, out var idOperacao))
            return Task.FromResult(Result.GoFails(new Fail(-1, "NUMERO_OPERACAO_NAO_ENCONTRADO", $"Número da operação não encontrado", FailLevel.Error)));

        var operacao = new Operacao
        (
            IdOperacao: idOperacao,
            TipoOperacao: dtoOperacao.TipoOperacao,
            Parte: IdentificacaoParte(dtoOperacao.ContaParte, regra.Parte) ?? 0,
            CarteiraParte: regra.Parte.Carteira,
            Titulo: dtoOperacao.Titulo,
            Quantidade: dtoOperacao.Quantidade,
            PURef: dtoOperacao.PURef,
            TaxaRef: dtoOperacao.TaxaRef,
            ValorTotal: CalcularValorOperacao(dtoOperacao, regra.CalculoValorOperacao),
            Contraparte: IdentificacaoParte(dtoOperacao.ContaContraparte, regra.Contraparte),
            CarteiraContraparte: regra.Contraparte?.Carteira ?? null
        );

        listaOperacoes.Add(operacao);
        bag.Response = operacao;
        return Task.FromResult(Result.GoSuccess());
    }

    static decimal? CalcularValorOperacao(DtoOperacao dto, EnumOperacaoCalculoValorTotal regraCalculoValorOperacao) => dto.PURef is null ? null : regraCalculoValorOperacao switch
    {
        EnumOperacaoCalculoValorTotal.SOMENTE_QUANTIDADE_X_PU => Math.Truncate(dto.PURef.Value * dto.Quantidade * 100) / 100,
        EnumOperacaoCalculoValorTotal.QUANTIDADE_X_PU_SUBTRAI_TAXAS => Math.Truncate(dto.PURef.Value * dto.Quantidade * 100) / 100,
        EnumOperacaoCalculoValorTotal.QUANTIDADE_X_PU_SOMA_TAXAS => Math.Truncate(dto.PURef.Value * dto.Quantidade * 100) / 100,
        _ => null
    };
    static int? IdentificacaoParte(int? parte, OperacaoCaracteristicasParte? regrasParte) => regrasParte is null ? null : regrasParte.ComportamentoParte switch
    {
        EnumOperacaoComportamentoParte.DECLARADA => parte,
        EnumOperacaoComportamentoParte.PREDEFINIDA => regrasParte.PartePredefinida,
        _ => null
    };
}
