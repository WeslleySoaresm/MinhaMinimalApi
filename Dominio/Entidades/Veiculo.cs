using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entidades
{
    public class Veiculo
    {

        [Key] //chave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // deixando o campo autoincrementado
        public int Id { get; set; } = default!;

        [Required] //deixa o campo primordial par ser prenchido 
        [StringLength(150)]
        public string Nome { get; set; } = default!;
        
        [Required] //deixa o campo primordial par ser prenchido 
        [StringLength(100)]
        public string Marca{ get; set; } = default!;

        [Required] //deixa o campo primordial par ser prenchido 
        public int Ano { get; set; } = default!;
    }
}