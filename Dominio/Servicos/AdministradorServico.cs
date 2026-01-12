
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

        public Administrador? Login(DTOs.LoginDTO loginDTO)
        {
            var administradores = _Contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();

            return administradores;
        }
    }
