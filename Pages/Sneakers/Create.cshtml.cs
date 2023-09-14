using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Sneakers
{
	public class CreateModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<CreateModel> _logger;
        public IFormFile? imageFile1;
        public IFormFile? imageFile2;

        public CreateModel(SneakerShopService sneakerShopService, ILogger<CreateModel> logger)
        {
            _sneakerShopService = sneakerShopService;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Sneaker Sneaker { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (Sneaker == null)
            {
                return Page();
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            imageFile1 = Request.Form.Files.GetFile("picture1");
            imageFile2 = Request.Form.Files.GetFile("picture2");

            MemoryStream dataStream = new MemoryStream();
            await imageFile1.CopyToAsync(dataStream);
            Sneaker.Picture1 = dataStream.ToArray();
            dataStream = new MemoryStream();
            await imageFile2.CopyToAsync(dataStream);
            Sneaker.Picture2 = dataStream.ToArray();

            await _sneakerShopService.CreateSneakerAsync(Sneaker);

            stopwatch.Stop();
            _logger.LogInformation("Sneaker Create Time: {0}", stopwatch.ElapsedMilliseconds);

            return RedirectToPage("/Index");
        }
    }
}
