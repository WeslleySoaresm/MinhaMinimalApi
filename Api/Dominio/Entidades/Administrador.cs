using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Dominio.Entidades
{
    public class Administrador
    {

        [Key] //chave primaria
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // deixando o campo autoincrementado
        public int Id { get; set; } = default!;

        [Required] //deixa o campo primordial par ser prenchido 
        [StringLength(255)]
        public string Email { get; set; } = default!;
        
        [Required] //deixa o campo primordial par ser prenchido 
        [StringLength(50)]
        public string Senha { get; set; } = default!;
        
        [Required] //deixa o campo primordial par ser prenchido 
        [StringLength(10)]
        public string Perfil { get; set; } = default!;
    }
}