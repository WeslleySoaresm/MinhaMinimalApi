using System.Reflection;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Servicos;

[TestClass]    
public sealed class AdministradorServiçoTest
{
    private DbContexto CriarContextoTeste()
{
    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory()) // Usa o diretório de execução atual
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables();

    var configuration = builder.Build();

    var options = new DbContextOptionsBuilder<DbContexto>()
        
        .UseInMemoryDatabase(databaseName: "TesteDatabase_" + Guid.NewGuid().ToString())
        .Options;

    return new DbContexto(options);
}
    [TestMethod]
    public void TestandoSalvarOAdministrador()
    {   
        // Arrange
        var context = CriarContextoTeste();
        context.Administradores.RemoveRange(context.Administradores);
        context.SaveChanges();
        context.Database.ExecuteSqlRaw("DELETE FROM administradores;"); // TRUNCATE pode falhar dependendo da FK, DELETE é mais seguro em testes
        var adm = new Administrador {
            Email = "administrador@teste.com",
            Senha = "SenhaSegura123!",
            Perfil = "adm"
        };
        
        var administradorServiço = new AdministradorServico(context);

        // Act
        administradorServiço.Incluir(adm);

        // Assert
        Assert.AreEqual(1, administradorServiço.Todos(1).Count);
        Assert.AreEqual("administrador@teste.com", adm.Email);
    }

    [TestMethod]
    public void TestandoBuscandoAdministradorPorId()
    {   
        // Arrange
        var context = CriarContextoTeste();
        context.Database.ExecuteSqlRaw("DELETE FROM administradores;");
        var adm = new Administrador {
            Email = "administrador@teste.com",
            Senha = "SenhaSegura123!",
            Perfil = "adm"
        };
        
        var administradorServiço = new AdministradorServico(context);

        // Act
        administradorServiço.Incluir(adm);
        var admDobanco = administradorServiço.BuscarPorId(adm.Id);
      
        // Assert
        Assert.IsNotNull(admDobanco); // Resolve o warning CS8602
        Assert.AreEqual(adm.Id, admDobanco.Id);
    }

    [TestMethod]
    public void TesteBuscarTodasPagina()
    {   
        // Arrange
        var context = CriarContextoTeste();
        context.Database.ExecuteSqlRaw("DELETE FROM administradores;");
        var adm = new Administrador {
            Email = "administrador@teste.com",
            Senha = "SenhaSegura123!",
            Perfil = "adm"
        };
        
        var administradorServiço = new AdministradorServico(context);

        // Act
        administradorServiço.Incluir(adm);
        // CORREÇÃO: Passando a página 1, em vez do ID do administrador
        var lista = administradorServiço.Todos(1);
      
        // Assert
        Assert.AreEqual(1, lista.Count);
    }
}