using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace App.Models
{

[Table("Categorias")]
public class Categoria
{
[Key]
public int CategoriaId { get; set; }
public string CategoriaNome { get; set; }
public List<Produto> Produtos { get; set; }
}
}