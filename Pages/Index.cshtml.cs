using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SneakerShopMongoDB.Models;
using SneakerShopMongoDB.Services;
using System.Diagnostics;

namespace SneakerShopMongoDB.Pages
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

        public IList<Sneaker> Sneaker { get; set; } = default!;

        public int PageSize { get; set; } = 4;
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPages;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? Brands { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? ItemBrands { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? PageSizeString { get; set; }

        public async Task OnGetAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (Sneaker == null)
            {
                Sneaker = await _sneakerShopService.GetSneakerAsync();
                Brands = new SelectList(Sneaker.Select(item => item.Brand).Distinct().ToList());
                if (!string.IsNullOrEmpty(SearchString))
                {
                    Sneaker = Sneaker.Where(s => s.Name.Contains(SearchString)
                                           || s.Brand.Contains(SearchString)).ToList();
                }
                if (!string.IsNullOrEmpty(ItemBrands))
                {
                    Sneaker = Sneaker.Where(s => s.Brand.Equals(ItemBrands)).ToList();
                }
                if (!string.IsNullOrEmpty(SortBy))
                {
                    switch (SortBy)
                    {
                        case "NameDESC": Sneaker = Sneaker.OrderByDescending(x => x.Name).ToList(); break;
                        case "PriceASC": Sneaker = Sneaker.OrderBy(x => x.Price).ToList(); break;
                        case "PriceDESC": Sneaker = Sneaker.OrderByDescending(x => x.Price).ToList(); break;
                        default: break;
                    }
                }
                if (!string.IsNullOrEmpty(PageSizeString))
                {
                    PageSize = int.Parse(PageSizeString);
                }
                TotalPages = Sneaker.Count / PageSize + Convert.ToInt32(Sneaker.Count % PageSize > 0);
                if (PageIndex == 0) PageIndex = 1;
                Sneaker = Sneaker.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();
            }
            stopwatch.Stop();
            _logger.LogInformation("Welcome Load Time: {0}", stopwatch.ElapsedMilliseconds);
        }
    }
}