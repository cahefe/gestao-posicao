using GestaoPosicao.Compartilhados.Enumeradores;

namespace GestaoPosicao.Custodia.Dtos;

public record DtoReserva
(
    int IdOperacao,
    int Conta,
    EnumCarteira Carteira,
    byte Titulo,
    decimal Quantidade
) {}
