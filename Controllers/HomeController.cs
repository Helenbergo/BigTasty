using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Projeto_Lanchonete.Models;
using App.Context;
using App.Models;
using X.PagedList;

namespace Projeto_Lanchonete.Controllers;

public class HomeController : Controller
{
    //
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Serie
    public IActionResult Index()
    {
        var appDbContext = _context.Produtos;
        return View(appDbContext.ToList());
    }

    public IActionResult Cardapio(string botao, string? txtFiltro, string? selOrdenacao, int pagina = 1)
        {
            int pageSize = 8; // Número de itens por página

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

            return View(listaView.ToPagedList(pagina, pageSize));
        }

    
    //

    public IActionResult Privacy()
    {
        return View();
    }
}
