using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]    
public sealed class AdministradorTest
{
    [TestMethod]
    public void TestMethodGetSetPropriedades()
    {   
        // Arrange
        var adm = new Administrador();

        // Act
        adm.Id = 1;
        adm.Email = "administrador@teste.com";
        adm.Senha = "SenhaSegura123!";

        // Assert
        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("administrador@teste.com", adm.Email);
        Assert.AreEqual("SenhaSegura123!", adm.Senha);

    }
}
