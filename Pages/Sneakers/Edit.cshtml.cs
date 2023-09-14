using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Sneakers
{
	public class EditModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<EditModel> _logger;
        public IFormFile? imageFile1;
        public IFormFile? imageFile2;
        public EditModel(SneakerShopService sneakerShopService, ILogger<EditModel> logger)
        {
            _sneakerShopService = sneakerShopService;
            _logger = logger;
        }

        [BindProperty]
        public Sneaker Sneaker { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sneaker = await _sneakerShopService.GetSneakerAsync(id);
            if (sneaker == null)
            {
                return NotFound();
            }
            Sneaker = sneaker;
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var sneaker = await _sneakerShopService.GetSneakerAsync(Sneaker.ID);

            if (Request.Form.Files.GetFile("picture1") == null)
            {
                Sneaker.Picture1 = sneaker.Picture1;
            }
            else
            {
                imageFile1 = Request.Form.Files.GetFile("picture1");
                MemoryStream dataStream = new MemoryStream();
                await imageFile1.CopyToAsync(dataStream);
                Sneaker.Picture1 = dataStream.ToArray();
            }
            if (Request.Form.Files.GetFile("picture2") == null)
            {
                Sneaker.Picture1 = sneaker.Picture1;
            }
            else
            {
                imageFile2 = Request.Form.Files.GetFile("picture2");
                MemoryStream dataStream = new MemoryStream();
                await imageFile2.CopyToAsync(dataStream);
                Sneaker.Picture2 = dataStream.ToArray();
            }

            try
            {
                await _sneakerShopService.UpdateSneakerAsync(Sneaker.ID, Sneaker);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SneakerExists(Sneaker.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            stopwatch.Stop();
            _logger.LogInformation("Sneaker Edit Time: {0}", stopwatch.ElapsedMilliseconds);
            return RedirectToPage("/Index");
        }

        private bool SneakerExists(string id)
        {
            return _sneakerShopService.GetSneakerAsync(id).IsCompletedSuccessfully;
        }
    }
}
