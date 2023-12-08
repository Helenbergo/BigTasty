using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Context;
using App.Models;
using App.Filters;
using X.PagedList;
using System.Xml;
using System.Text;

namespace Projeto_Lanchonete.Controllers
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
        public IActionResult Index(string botao, string? txtFiltro, string? selOrdenacao, int pagina = 1)
        {
            int pageSize = 4; // Número de itens por página

            IQueryable<Produto> listaView = _context.Produtos;

            if (botao == "Relatorio")
            {
                pageSize = listaView.Count();
            }

            if (!string.IsNullOrEmpty(txtFiltro))
            {
                ViewData["txtFiltro"] = txtFiltro;
                txtFiltro = txtFiltro.ToLower();
                listaView = listaView.Where(item => item.NomeProduto.ToLower().Contains(txtFiltro));
            }

            if (selOrdenacao == "Nome" || selOrdenacao == null)
            {
                listaView = listaView.OrderBy(item => item.NomeProduto.ToLower());
            }
            else if (selOrdenacao == "Maior_preco")
            {
                listaView = listaView.OrderByDescending(item => item.Preco);
            }
            else if (selOrdenacao == "Menor_preco")
            {
                listaView = listaView.OrderBy(item => item.Preco);
            }

            //Verificando se o botão clicado foi o XML
            if (botao == "XML")
            {
                //Chamando o método para exportar o XML enviando como parâmentro a lista já filtrada
                return ExportarXML(listaView.ToList());
            }
            else if (botao == "Json")
            {
                //Chamando o método para exportar o Json enviando como parâmentro a lista já filtrada
                return ExportarJson(listaView.ToList());
            }

            return View(listaView.ToPagedList(pagina, pageSize));
        }

        // GET: Produto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Produtos == null)
            {
                return NotFound();
            }

            var produto = await _context.Produtos
                .FirstOrDefaultAsync(m => m.ProdutoID == id);
            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // GET: Produto/Create
        public IActionResult Create()
        {
            ViewData["Categorias"] = new SelectList(_context.Categoria, "CategoriaId", "CategoriaNome");
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
            ViewData["Categorias"] = new SelectList(_context.Categoria, "CategoriaId", "CategoriaNome");
            return View(produto);
        }

        // POST: Produto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Produto produto)
        {
            if (id != produto.ProdutoID)
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
                    if (!ProdutoExists(produto.ProdutoID))
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
                .FirstOrDefaultAsync(m => m.ProdutoID == id);
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
            return (_context.Produtos?.Any(e => e.ProdutoID == id)).GetValueOrDefault();
        }

        private IActionResult ExportarXML(List<Produto> lista)
        {
            var stream = new StringWriter();
            var xml = new XmlTextWriter(stream);
            xml.Formatting = System.Xml.Formatting.Indented;
            xml.WriteStartDocument();
            xml.WriteStartElement("Dados");
            xml.WriteStartElement("Produtos");
            foreach (var item in lista)
            {
                xml.WriteStartElement("Produto");
                xml.WriteElementString("Id", item.ProdutoID.ToString());
                xml.WriteElementString("Nome", item.NomeProduto);
                xml.WriteElementString("Preço", item.Preco.ToString());
                xml.WriteEndElement(); // </Usuario>
            }
            xml.WriteEndElement(); // </Usuarios>
            xml.WriteEndElement(); // </Dados>
            return File(Encoding.UTF8.GetBytes(stream.ToString()), "application/xml", "dados_usuarios.xml");
        }

        private IActionResult ExportarJson(List<Produto> lista)
        {
            var json = new StringBuilder();
            json.AppendLine("{");
            json.AppendLine(" \"Produtos\": [");
            int total = 0;
            foreach (var item in lista)
            {
                json.AppendLine(" {");
                json.AppendLine($" \"Id\": {item.ProdutoID},");
                json.AppendLine($" \"Nome\": \"{item.NomeProduto}\",");
                json.AppendLine($" \"Preço\": \"{item.Preco}\",");
                json.AppendLine(" }");
                total++;
                if (total < lista.Count())
                {
                    json.AppendLine(" ,");
                }
            }
            json.AppendLine(" ]");
            json.AppendLine("}");
            return File(Encoding.UTF8.GetBytes(json.ToString()), "application/json", "dados_usuarios.json");
        }
    }
}
