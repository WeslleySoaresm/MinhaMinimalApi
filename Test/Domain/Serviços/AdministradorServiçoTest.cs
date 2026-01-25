using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Servicos;

[TestClass]    
public sealed class AdministradorServiçoTest
{
    private DbContexto CriarContextoTeste()
    {   

        var AssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(AssemblyPath ?? @"..\..\..\"));
        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();
        
        
        return new DbContexto(configuration);
       
    }


    [TestMethod]
    public void TestandoSalvarOAdministrador()
    {   
        // Arrange
        var context = CriarContextoTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;"); // Limpa a tabela antes do teste
        var adm = new Administrador();
        adm.Email = "administrador@teste.com";
        adm.Senha = "SenhaSegura123!";
        adm.Perfil = "adm";
        
        var administradorServiço = new AdministradorServico(context);


        // Act

        administradorServiço.Incluir(adm);
      

        // Assert
        Assert.AreEqual(1, administradorServiço.Todos(1).Count);
        Assert.AreEqual("administrador@teste.com", adm.Email);
        Assert.AreEqual("SenhaSegura123!", adm.Senha);

    }


    [TestMethod]
    public void TestandoBuscandoAdministradorPorId()
    {   
        // Arrange
        var context = CriarContextoTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;"); // Limpa a tabela antes do teste
        var adm = new Administrador();
        adm.Email = "administrador@teste.com";
        adm.Senha = "SenhaSegura123!";
        adm.Perfil = "adm";
        
        var administradorServiço = new AdministradorServico(context);


        // Act

        administradorServiço.Incluir(adm);
        var admDobanco =  administradorServiço.BuscarPorId(adm.Id);
      

        // Assert
        Assert.AreEqual(1, admDobanco.Id);
    }


    [TestMethod]
    public void TesteBuscarTodasPagina()
    {   
        // Arrange
        var context = CriarContextoTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores;"); // Limpa a tabela antes do teste
        var adm = new Administrador();
        adm.Email = "administrador@teste.com";
        adm.Senha = "SenhaSegura123!";
        adm.Perfil = "adm";
        
        var administradorServiço = new AdministradorServico(context);


        // Act

        administradorServiço.Incluir(adm);
        var admDobanco =  administradorServiço.Todos(adm.Id);
      

        // Assert
        Assert.AreEqual(1, admDobanco.Count);
    }
}
