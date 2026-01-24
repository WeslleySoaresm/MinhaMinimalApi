using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;


namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{   
    private readonly IConfiguration _configuracaoAppSettings;
    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }
        public DbSet<Administrador> Administradores { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        var stringConexao = _configuracaoAppSettings.GetConnectionString("Mysql")?.ToString(); //variavel de string de conexão
        if (!string.IsNullOrEmpty(stringConexao))
        {
           optionsBuilder.UseMySql(
            stringConexao,
            ServerVersion.AutoDetect(stringConexao)

            ); 
        }

        
    }
}

