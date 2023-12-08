using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models

{
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        public int CategoriaID { get; set; }
        public string CategoriaNome { get; set; }
        public string Descricao { get; set; }
        public List<Produto> Produtos { get; set; }
    }
}