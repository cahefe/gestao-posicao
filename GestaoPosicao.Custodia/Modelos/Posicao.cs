namespace GestaoPosicao.Custodia.Modelos;
public class Posicoes : List<Posicao> {}
public record Posicao
(
    int Conta,
    byte Titulo,
    DateOnly DataRef,
    decimal PURef,
    decimal TaxaRef,
    decimal Quantidade
)
{
    static int _idPosicao = 0;
    public int IdPosicao { get; init; } = ++_idPosicao;
    public decimal Quantidade { get; set; } = Quantidade;
    public override string ToString() => $"Pos Pid:{IdPosicao,4} Co:{Conta,4} Ti:{Titulo,4} Dr:{DataRef:dd/MM} Qt:{Quantidade,10}";
}
