using Microsoft.EntityFrameworkCore;
using MinhaMinimalApi.Dominio.Entidades;

namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{
    public DbSet<Administrador> Administradores { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(
            "String de conexão com o banco de dados",
            ServerVersion.AutoDetect("String de conexão com o banco de dados")

        );
    }
}