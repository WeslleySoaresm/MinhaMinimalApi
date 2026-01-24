
using MinimalApi.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;
using System.Linq;

namespace MinimalApi.Dominio.Servicos;

    public class AdministradorServico : IAdministradorServico
    {
        private readonly DbContexto _Contexto;

        public AdministradorServico(DbContexto contexto)
        {
            _Contexto = contexto;
        }


          public Administrador? BuscarPorId(int id)
            {
                return _Contexto.Administradores.Find(id);
            }

    public Administrador Incluir(Administrador administrador)
    {
        _Contexto.Administradores.Add(administrador);
        _Contexto.SaveChanges();
        return administrador;
    }

    public object Incluir(AdministradorDTO administradorDTO)
    {
        throw new NotImplementedException();
    }

    public Administrador? Login(DTOs.LoginDTO loginDTO)
        {
            var administradores = _Contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();

            return administradores;
        }

    public List<Administrador> Todos(int? pagina)
    {
        var query = _Contexto.Administradores.AsQueryable();

        if (pagina != null)
        {
            query = query
            .Skip(((int)pagina - 1) * 10)
            .Take(10);
        }

        return query.ToList();
    }

   
}