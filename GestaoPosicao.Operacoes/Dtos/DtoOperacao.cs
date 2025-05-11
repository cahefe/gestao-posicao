using GestaoPosicao.Compartilhados.Enumeradores;

namespace GestaoPosicao.Operacoes.Dtos;

public record DtoOperacao
(
    EnumOperacaoTipo TipoOperacao,
    byte Titulo,
    decimal Quantidade,
    decimal? PURef = null,
    decimal? TaxaRef = null,
    int? ContaParte = null,
    int? ContaContraparte = null
);
