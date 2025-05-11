using System.Collections.Concurrent;
using GestaoPosicao.Compartilhados.Enumeradores;

namespace GestaoPosicao.Compartilhados;

public static class Numerador
{
    private static readonly ConcurrentDictionary<EnumOperacaoTipo, int> _controleNumeracao = new();
    public static int Obter(EnumOperacaoTipo operacaoTipo) => _controleNumeracao.AddOrUpdate(operacaoTipo, (int)operacaoTipo * 100, (key, old) => old + 1);
}