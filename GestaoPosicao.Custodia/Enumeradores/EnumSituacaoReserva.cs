namespace GestaoPosicao.Custodia.Enumeradores;

internal enum EnumSituacaoReserva : byte
{
    Registrada = 1,
    Liquidada = 2,
    Expirada = byte.MaxValue,
}
