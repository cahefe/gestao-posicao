using GestaoPosicao.Compartilhados.Enumeradores;
using GestaoPosicao.Custodia.Enumeradores;

namespace GestaoPosicao.Custodia.Modelos;
public class Reservas : List<Reserva>{ }
public record Reserva
(
    int IdOperacao,
    int Conta,
    EnumCarteira Carteira,
    byte Ativo,
    decimal Quantidade
)
{
    internal EnumSituacaoReserva Situacao { get; set; } = EnumSituacaoReserva.Registrada;
    internal DateTime DataReserva { get; init; } = DateTime.Now;
}
