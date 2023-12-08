using App.Models;
using X.PagedList;
namespace App.Models;
public class ProdutoCategoriaViewModel
{
    public IPagedList<Produto> ListaProduto { get; set; }
    public IEnumerable<Categoria> ListaCategoria { get; set; }
    public IEnumerable<Banner> ListaBanner { get; set; }
}