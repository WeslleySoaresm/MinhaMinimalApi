using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;

namespace Test.Mocks;   

public class AdministradorServicoMock : IAdministradorServico
{
    private readonly List<Administrador> _administradores = new List<Administrador>();

    // CORREÇÃO: Mudado de 'void' para 'Administrador' para bater com a Interface
    public Administrador Incluir(Administrador administrador)
    {
        administrador.Id = _administradores.Count() + 1;
        _administradores.Add(administrador);

        return administrador; // Agora o retorno é permitido
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        return _administradores.FirstOrDefault(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha);
    }

    // CORREÇÃO: Ajustado para aceitar int? (Nullable) se for assim que está na Interface
    public List<Administrador> Todos(int? pagina)
    {
        return _administradores;
    }

    public Administrador? BuscarPorId(int id)
    {
        return _administradores.FirstOrDefault(a => a.Id == id);
    }

    public object Incluir(AdministradorDTO administradorDTO)
    {
        throw new NotImplementedException();
    }
}