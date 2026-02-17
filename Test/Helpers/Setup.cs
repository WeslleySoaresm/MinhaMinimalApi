using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Entidades;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Test.Helpers;

public class Setup
{
    public static WebApplicationFactory<Program> Factory { get; set; } = default!;
    public static HttpClient Client { get; set; } = default!;

    public static void ClassInitializeAttribute(TestContext context)
    {
        var Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                // Adicione esta linha para garantir que ele ache os arquivos compilados
                builder.UseContentRoot(Directory.GetCurrentDirectory());
            });

        Setup.Factory = Factory;
        Setup.Client = Factory.CreateClient();
    }
    public static void ClassCleanup()
    {
        Factory.Dispose();
    }

    // Método auxiliar para obter o Contexto do Banco de Dados
    public static DbContexto GetContext()
    {
        var scope = Factory.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<DbContexto>();
    }
}