using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages.Users
{
	public class IndexModel : PageModel
    {
        private readonly SneakerShopService _sneakerShopService;
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(SneakerShopService sneakerShopService, ILogger<IndexModel> logger)
        {
            _sneakerShopService = sneakerShopService;
            _logger = logger;
        }

        public IList<User> User { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (User == null)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                User = await _sneakerShopService.GetUserAsync();
                stopwatch.Stop();
                _logger.LogInformation("User Index Time: {0}", stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
