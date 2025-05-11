using GestaoPosicao.Compartilhados.Enumeradores;

namespace GestaoPosicao.Compartilhados.Modelos;

public record OperacaoCaracteristicasParte
(
    EnumOperacaoIndicadorParte IndicadorParte,
    EnumOperacaoComportamentoParte ComportamentoParte,
    EnumCarteira Carteira,
    EnumMovimentoTipo MovimentoTipo,
    EnumMovimentoSentido MovimentoSentido,
    int PartePredefinida = 0
)
{ }
