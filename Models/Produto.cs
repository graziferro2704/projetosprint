using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models

{
    [Table("Produtos")]
    public class Produto
    {
        [Key]
        public int ProdutoId { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public float Preco { get; set; }
        public string Foto { get; set; }
        public bool Ativo { get; set; }
        public int CategoriaID { get; set; }
        public virtual Categoria Categoria { get; set; }
    }
}