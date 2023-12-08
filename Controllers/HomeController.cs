using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjetoSprint.Models;
using App.Context;
using Microsoft.EntityFrameworkCore;
using App.Models;
using X.PagedList;

namespace ProjetoSprint.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string botao, string Categoria, string? txtFiltro, string? selOrdenacao, int pagina = 1)
        {

            int pageSize = 12;

            var appDbContext = _context.Produtos.Include(c => c.Categoria);

            IQueryable<Produto> listaView = _context.Produtos.Include(c => c.Categoria);

            if (botao == "Relatorio")
            {
                pageSize = listaView.Count();
            }

            if (txtFiltro != null && txtFiltro != "")
            {
                ViewData["txtFiltro"] = txtFiltro;
                listaView = listaView.Where(item => item.Nome.ToLower().Contains(txtFiltro.ToLower())
                                            || item.Categoria.CategoriaNome.ToLower().Contains(txtFiltro.ToLower()));
            }

            if (Categoria != null && Categoria != "")
            {
                listaView = listaView.Where(item => item.Categoria.CategoriaNome == Categoria);
            }

            ViewData["Ordem"] = selOrdenacao;

            if (selOrdenacao == "Nome" || selOrdenacao == null)
            {
                listaView = listaView.OrderBy(item => item.Nome.ToLower());
            }
            else if (selOrdenacao == "Categoria")
            {
                listaView = listaView.OrderBy(item => item.Categoria.CategoriaNome.ToLower());
            }
            else if (selOrdenacao == "MenorPreço")
            {
                listaView = listaView.OrderBy(item => item.Preco);
            }
            else if (selOrdenacao == "MaiorPreço")
            {
                listaView = listaView.OrderByDescending(item => item.Preco);
            }

            ProdutoCategoriaViewModel vm = new ProdutoCategoriaViewModel
        {
            ListaProduto = listaView.ToPagedList(pagina, pageSize),
            ListaBanner = _context.Banners.OrderBy(item => item.BannerId).ToList(),
            ListaCategoria = _context.Categorias.OrderBy(item => item.CategoriaNome).ToList()
        };


            return View(vm);
        }

        public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

    

