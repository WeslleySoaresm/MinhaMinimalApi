using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.DTOs;
using Test.Helpers;

namespace Test.Requests;

[TestClass]


public class AdministradorRequestTest


{
    
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        Setup.ClassInitializeAttribute(context);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestarLoginComSucesso()
    {
        // --- ARRANGE ---
        var emailTeste = "admin@empresa.com";
        var senhaTeste = "123456";

        // Inserindo o administrador no banco de teste para evitar o erro 401
        using (var db = Setup.GetContext())
        {
            // Limpa se já existir para garantir um estado limpo
            var admExistente = db.Administradores.FirstOrDefault(a => a.Email == emailTeste);
            if (admExistente != null) db.Administradores.Remove(admExistente);

            db.Administradores.Add(new Administrador
            {
                Email = emailTeste,
                Senha = senhaTeste, // Se sua API usa HASH, você deve gerar o hash aqui!
                Perfil = "Adm"
            });
            db.SaveChanges();
        }

        var loginDTO = new LoginDTO
        {
            Email = emailTeste,
            Senha = senhaTeste
        };

        // --- ACT ---
        // Certifique-se de que a rota "/api/administradores/login" é a mesma do seu Program.cs
        var response = await Setup.Client.PostAsJsonAsync("/api/administradores/login", loginDTO);

        // --- ASSERT ---
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Esperava 200 OK mas recebeu {response.StatusCode}");

        var result = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, options);

        Assert.IsNotNull(admLogado);
        Assert.AreEqual(emailTeste, admLogado.Email);
        Assert.IsNotNull(admLogado.Token);

        
    } 

    
}