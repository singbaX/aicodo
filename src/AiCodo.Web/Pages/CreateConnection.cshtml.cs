using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using AiCodo.Web.Models;
using System.Threading.Tasks;
using AiCodo.Data;
using System.Collections.Generic;

namespace AiCodo.Web.Pages
{
    public class CreateConnectionModel : PageModel
    {
        public void OnGet()
        {
        }

        public IEnumerable<string> Providers
        {
            get
            {
                return DbProviderFactories.GetProviderNames();
            }
        }

        [BindProperty]
        [Required]
        public ConnectionItem Connection { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var data = Connection.ToDynamicEntity();
            ConfigService.CreateConnection(data);
            return RedirectToPage("./Index");
        }
    }
}
