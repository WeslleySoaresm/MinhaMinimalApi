
using MinimalApi.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;
using System.Linq;

namespace MinimalApi.Dominio.Servicos;

    public class VeiculoServico : IVeiculosServico
    {
        private readonly DbContexto _Contexto;

        public VeiculoServico(DbContexto contexto)
        {
            _Contexto = contexto;
        }

    public void Atualizar(Veiculo veiculo)
    {
        _Contexto.Veiculos.Update(veiculo);
        _Contexto.SaveChanges();
    }

    public Veiculo? BuscarPorId(int id)
    {
        return _Contexto.Veiculos.Find(id);
    }

    public void Deletar(Veiculo veiculo)
    {
        _Contexto.Veiculos.Remove(veiculo);
        _Contexto.SaveChanges();    
    }

    public void Incluir(Veiculo veiculo)
    {
        _Contexto.Veiculos.Add(veiculo);
        _Contexto.SaveChanges();
    }

    public List<Veiculo> Login(int pagina = 1, string? nome = null, string? marca = null)
    {
        throw new NotImplementedException();
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var veiculos = _Contexto.Veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            veiculos = veiculos.Where(v => v.Nome.Contains(nome));
        }

        if (!string.IsNullOrEmpty(marca))
        {
            veiculos = veiculos.Where(v => v.Marca.Contains(marca));
        }

        if (pagina.HasValue)
        {
            veiculos = veiculos
            .Skip(((int)pagina - 1) * 10)
            .Take(10);
        }
         


        return veiculos.ToList();
    }
}