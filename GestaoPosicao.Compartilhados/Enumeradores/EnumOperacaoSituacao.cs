namespace GestaoPosicao.Compartilhados.Enumeradores;

public enum EnumOperacaoSituacao : byte
{
    Registrada = 1,
    Confirmada = 2,
    Liquidada = 3,
    Cancelada = 4,
    Rejeitada = 5,
    Expirada = 6
}