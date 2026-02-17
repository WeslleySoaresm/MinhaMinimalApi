using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;


namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{   
    public DbContexto(DbContextOptions<DbContexto> options) : base(options) { }

    private readonly IConfiguration _configuracaoAppSettings;
    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }
        public DbSet<Administrador> Administradores { get; set; } = default!;
        public DbSet<Veiculo> Veiculos { get; set; } = default!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Se as opções já foram configuradas (pelo UseInMemoryDatabase no teste), não faça nada!
        if (optionsBuilder.IsConfigured) return;

        if (_configuracaoAppSettings != null)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("mysql");
            if (!string.IsNullOrWhiteSpace(stringConexao))
            {
                // Só entra aqui se houver uma string de conexão real
                optionsBuilder.UseMySql(stringConexao, ServerVersion.AutoDetect(stringConexao));
            }
        }
    }
}

