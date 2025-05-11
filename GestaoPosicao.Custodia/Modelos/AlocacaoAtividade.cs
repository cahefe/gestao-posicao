using GestaoPosicao.Compartilhados.Enumeradores;

namespace GestaoPosicao.Custodia.Modelos;
public class AlocacoesAtividades : List<AlocacaoAtividade> { }
public record AlocacaoAtividade
(
    int IdAlocacao,
    int IdOperacao,
    decimal Quantidade
)
{
    public override string ToString() => $"Atv Al:{IdAlocacao,4} Op:{IdOperacao,4} Qt:{Quantidade,10}";
}
