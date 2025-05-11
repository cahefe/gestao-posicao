using GestaoPosicao.Compartilhados.Enumeradores;

namespace GestaoPosicao.Custodia.Dtos;
public record DtoLiquidacaoReq
(
    int IdOperacao,
    EnumOperacaoTipo TipoOperacao,
    int ContaParte,
    EnumCarteira CarteiraParte,
    byte Titulo,
    decimal Quantidade,
    decimal? PURef,
    decimal? TaxaRef,
    decimal? ValorTotal = null,
    int? ContaContraparte = null,
    EnumCarteira? CarteiraContraparte = null
) {}
