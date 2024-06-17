using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Accidentabilidad.Pages
{
    public class PrivacyModel : PageModel
    {
        public List<Item> Items { get; set; }
        public void OnGet()
        {
            Items = new List<Item>
            {
            new Item { Id = 1, Name = "Item1" },
            new Item { Id = 2, Name = "Item2" },
            new Item { Id = 3, Name = "Item3" }
            };
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}