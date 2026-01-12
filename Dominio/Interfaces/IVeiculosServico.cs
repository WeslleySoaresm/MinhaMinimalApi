using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;


namespace MinimalApi.Dominio.Interfaces;

    
    public interface IVeiculosServico
    {
       List<Veiculo> Login(int pagina = 1, string? nome=null, string? marca=null);

       Veiculo? BuscarPorId(int id);

       void Incluir(Veiculo veiculo);

       void Atualizar(Veiculo veiculo);

        void Deletar(Veiculo veiculo);
    }

