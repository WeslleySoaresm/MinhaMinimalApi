namespace MinimalApi.DTOs;

public record VeiculoDTO
{
        public object Id { get;  set; } = default!;
        
        public string Nome { get; set; } = default!;
        
        
        public string Marca{ get; set; } = default!;

        public int Ano { get; set; } = default!;
      
}