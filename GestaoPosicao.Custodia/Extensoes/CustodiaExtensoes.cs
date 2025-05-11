using cahefe.UseCases;
using GestaoPosicao.Custodia.Dtos;
using Microsoft.Extensions.DependencyInjection;

namespace GestaoPosicao.Custodia.Extensoes;

public static class CustodiaExtensoes
{
    public static IServiceCollection IncluirCasosUsoCustodia(this IServiceCollection services) => services
        .AddScoped<Pipeline<DtoLiquidacaoReq, DtoLiquidacaoResp>>()
        .AddScoped<Bag<DtoLiquidacaoReq, DtoLiquidacaoResp>>()
        .AddTransient<LiquidarPrepararPipeline>()
        .AddTransient<LiquidarCreditarPosicoes>()
        .AddTransient<LiquidarDebitarPosicoes>()
        .AddTransient<LiquidarManterAlocacoes>()
        .AddTransient<LiquidarManterMovimentos>()
        .AddTransient<LiquidarPersistir>()
        ;
}
