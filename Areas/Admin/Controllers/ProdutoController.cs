using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Context;
using App.Models;
using X.PagedList;
using System.Xml;
using System.Text;

using App.Filters;

namespace ProjetoSprint.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ProdutoController : Controller
    {
        private readonly AppDbContext _context;

        public ProdutoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Produto
        public async Task<IActionResult> Index(string botao, string Categoria, string? txtFiltro, string? selOrdenacao, int pagina = 1)
        {

            int pageSize = 6;

            var appDbContext = _context.Produtos.Include(p => p.Categoria);

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

            if (botao == "XML")
            {
                return ExportarXML(listaView.ToList());
            }
            else if (botao == "Json")
            {
                return ExportarJson(listaView.ToList());
            }

            ProdutoCategoriaViewModel vm = new ProdutoCategoriaViewModel
        {
            ListaProduto = listaView.ToPagedList(pagina, pageSize),
            ListaCategoria = _context.Categorias.OrderBy(item => item.CategoriaNome).ToList()
        };


            return View(vm);
        }

        
        private IActionResult ExportarXML(List<Produto> listaView)
        {
            var stream = new StringWriter();
            var xml = new XmlTextWriter(stream);

            xml.Formatting = System.Xml.Formatting.Indented;

            xml.WriteStartDocument();
            xml.WriteStartElement("Produto");

            xml.WriteStartElement("Produtos");
            foreach (var produto in listaView)
            {
                xml.WriteStartElement("Produto");
                xml.WriteElementString("Id", produto.ProdutoId.ToString());
                xml.WriteElementString("Nome", produto.Nome);
                xml.WriteElementString("Descricao", produto.Descricao);
                xml.WriteElementString("Preco", produto.Preco.ToString());
                xml.WriteElementString("Categoria", produto.Categoria.CategoriaNome);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();

            xml.WriteEndElement();
            return File(Encoding.UTF8.GetBytes(stream.ToString()), "application/xml", "dados_produtos.xml");
        }

        private IActionResult ExportarJson(List<Produto> listaView)
        {
            var json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine("  \"Produtos\": [");
            int total = 0;
            foreach (var produto in listaView)
            {
                json.AppendLine("    {");
                json.AppendLine($"      \"Id\": {produto.ProdutoId},");
                json.AppendLine($"      \"Nome\": \"{produto.Nome}\",");
                json.AppendLine($"      \"Descricao\": \"{produto.Descricao}\",");
                json.AppendLine($"      \"Categoria\": \"{produto.Categoria.CategoriaNome}\",");
                json.AppendLine($"      \"Preco\": {produto.Preco.ToString()}");
                json.AppendLine("    }");
                total++;
                if (total < listaView.Count())
                {
                    json.AppendLine("    ,");
                }
            }
            json.AppendLine("  ]");
            json.AppendLine("}");

            return File(Encoding.UTF8.GetBytes(json.ToString()), "application/json", "dados_produto.json");
        }

        // GET: Produto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Produtos == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produto/Create
        public IActionResult Create()
        {
            ViewData["CategoriaID"] = new SelectList(_context.Categorias, "CategoriaID", "CategoriaNome");
            return View();
        }

        // POST: Produto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Produto produto)
        {
            if (true)
            {
                _context.Add(produto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaID"] = new SelectList(_context.Categorias, "CategoriaIDID", "CategoriaIDID", produto.CategoriaID);
            return View(produto);
        }

        // GET: Produto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Produtos == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                return NotFound();
            }
            ViewData["CategoriaID"] = new SelectList(_context.Categorias, "CategoriaID", "CategoriaNome", produto.CategoriaID);
            return View(produto);
        }

        // POST: Produto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                return NotFound();
            }

            if (true)
            {
                try
                {
                    _context.Update(produto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProdutoExists(produto.ProdutoId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaID"] = new SelectList(_context.Categorias, "CategoriaIDID", "CategoriaIDID", produto.CategoriaID);
            return View(produto);
        }

        // GET: Produto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Produtos == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.ProdutoId == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Produtos == null)
            {
                return Problem("Entity set 'AppDbContext.Produtos'  is null.");
            }
            var produto = await _context.Produtos.FindAsync(id);
            if (produto != null)
            {
                _context.Produtos.Remove(produto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProdutoExists(int id)
        {
            return (_context.Produtos?.Any(e => e.ProdutoId == id)).GetValueOrDefault();
        }
    }
}
