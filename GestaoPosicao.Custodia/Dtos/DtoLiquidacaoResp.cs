using GestaoPosicao.Custodia.Enumeradores;

namespace GestaoPosicao.Custodia.Dtos;

public record class DtoLiquidacaoResp
(
    int IdOperacao,
    EnumSituacaoMovimentacao Situacao
)
{ }
