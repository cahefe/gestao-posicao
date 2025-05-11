using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Compartilhados.Modelos;

namespace GestaoPosicao.Custodia.Modelos;

public class Movimentos : List<Movimento> { }
public record Movimento
(
    EnumOperacaoTipo TipoOperacao,
    int IdOperacao,
    EnumMovimentoTipo MovimentoTipo,
    int ContaParte,
    EnumCarteira CarteiraParte,
    byte Titulo,
    decimal? PURef,
    decimal? TaxaRef,
    decimal Quantidade,
    decimal? ValorTotal,
    int? ContaContraparte = null,
    EnumCarteira? CarteiraContraparte = null
)
{
    static int _idMovimento;
    public int IdMovimento { get; init; } = ++_idMovimento;
    public DateTime DataMovimento { get; init; } = DateTime.Now;

    public override string ToString() => $"Mov Mv:{IdMovimento,4} Op:{IdOperacao,4} To:{TipoOperacao,-20} Mt:{MovimentoTipo} (Co:{ContaParte,4}/Ka:{CarteiraParte}) Ti:{Titulo,4} Qt:{Quantidade,10} Vt:{ValorTotal,10} (Cp:{ContaContraparte,4}/Kp:{CarteiraContraparte})";
}
