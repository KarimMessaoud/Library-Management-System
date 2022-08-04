using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Library.Commands.Catalog;
using Library.Models.Catalog;
using Library.Models.Checkout;
using Library.Queries.Catalog;
using Library.Security;
using LibraryData;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;
        private readonly ILibraryBranch _branch;
        private readonly LibraryContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICheckout _checkout;
        private readonly ILogger<CatalogController> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CatalogController(
                        ILibraryAssetService assetsService,
                        IDataProtectionProvider dataProtectionProvider,
                        DataProtectionPurposeStrings dataProtectionPurposeStrings,
                        ILibraryBranch branch,
                        LibraryContext context,
                        IWebHostEnvironment webHostEnvironment,
                        ICheckout checkout,
                        ILogger<CatalogController> logger,
                        IMapper mapper, IMediator mediator)
        {
            _assetsService = assetsService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
            _branch = branch;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _checkout = checkout;
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString, string currentFilter, int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var libraryAssets = await _mediator.Send(new GetAllAssetsQuery(searchString, currentFilter, pageNumber));

            int pageSize = 10;

            return View(PaginatedList<AssetIndexListingViewModel>.Create(libraryAssets.AsQueryable(), pageNumber ?? 1, pageSize));
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public IActionResult CreateBook()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> CreateBook(AssetCreateBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(new CreateBookCommand(model));

                return RedirectToAction("Create", "Catalog");
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public IActionResult CreateVideo()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> CreateVideo(AssetCreateVideoViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(new CreateVideoCommand(model));

                return RedirectToAction("Create", "Catalog");
            }

            return View(model);
        }


        private string ProcessUploadedAssetFile(AssetCreateViewModel model)
        {
            string uniqueFileName = null;

            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Detail(string id)
        {
            var result = await _mediator.Send(new GetLibraryAssetQuery(id));

            if (result == null)
            {
                return View("AssetNotFound", id);
            }
             
            return View(result);
        }



        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> EditBook(string id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(id));

            var asset = await _assetsService.GetByIdAsync(decryptedId);

            if (asset == null)
            {
                Response.StatusCode = 404;
                return View("AssetNotFound", decryptedId);
            }

            var model = _mapper.Map<AssetEditBookViewModel>(asset);

            model.Id = id;
            model.Author = await _assetsService.GetAuthorOrDirectorAsync(decryptedId);
            model.ISBN = await _assetsService.GetIsbnAsync(decryptedId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> EditBook(AssetEditBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                int decryptedId = Convert.ToInt32(protector.Unprotect(model.Id));
                var book = await _assetsService.GetBookByIdAsync(decryptedId);

                if (book == null)
                {
                    Response.StatusCode = 404;
                    return View("AssetNotFound", decryptedId);
                }

                book.Title = model.Title;
                book.Author = model.Author;
                book.Year = model.Year;
                book.ISBN = model.ISBN;
                book.Cost = model.Cost;
                book.NumberOfCopies = model.NumberOfCopies;

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null && model.ExistingPhotoPath != "/images/")
                    {
                        string filePath = Path.Join(_webHostEnvironment.WebRootPath, model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }

                    string uniqueFileName = ProcessUploadedAssetFile(model);

                    book.ImageUrl = "/images/" + uniqueFileName;
                }

                book.Location = _branch.GetBranchByName(model.LibraryBranchName);

                await _assetsService.UpdateAsync(book);

                return RedirectToAction("Detail", "Catalog", new { id = model.Id });
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> EditVideo(string id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(id));

            var asset = await _assetsService.GetByIdAsync(decryptedId);

            if (asset == null)
            {
                Response.StatusCode = 404;
                return View("AssetNotFound", decryptedId);
            }

            var model = _mapper.Map<AssetEditVideoViewModel>(asset);

            model.Id = id;
            model.Director = await _assetsService.GetAuthorOrDirectorAsync(decryptedId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> EditVideo(AssetEditVideoViewModel model)
        {
            if (ModelState.IsValid)
            {
                int decryptedId = Convert.ToInt32(protector.Unprotect(model.Id));
                var video = await _assetsService.GetVideoByIdAsync(decryptedId);

                if (video == null)
                {
                    Response.StatusCode = 404;
                    return View("AssetNotFound", decryptedId);
                }

                video.Title = model.Title;
                video.Director = model.Director;
                video.Year = model.Year;
                video.Cost = model.Cost;
                video.NumberOfCopies = model.NumberOfCopies;

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null && model.ExistingPhotoPath != "/images/")
                    {
                        string filePath = Path.Join(_webHostEnvironment.WebRootPath, model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }

                    string uniqueFileName = ProcessUploadedAssetFile(model);

                    video.ImageUrl = "/images/" + uniqueFileName;
                }

                video.Location = _branch.GetBranchByName(model.LibraryBranchName);

                await _assetsService.UpdateAsync(video);

                return RedirectToAction("Detail", "Catalog", new { id = model.Id });
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(id));

            ViewBag.DecryptedId = decryptedId;

            var asset = await _assetsService.GetByIdAsync(decryptedId);

            if (asset == null)
            {
                Response.StatusCode = 404;
                return View("AssetNotFound", decryptedId);
            }

            var model = _mapper.Map<AssetEditBookViewModel>(asset);

            model.Id = id;
            model.Author = await _assetsService.GetAuthorOrDirectorAsync(decryptedId);
            model.ISBN = await _assetsService.GetIsbnAsync(decryptedId);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(id));

            var book = await _assetsService.GetByIdAsync(decryptedId);

            if (book == null)
            {
                Response.StatusCode = 404;
                return View("AssetNotFound", decryptedId);
            }

            try
            {
                await _assetsService.DeleteAsync(book);
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Checkout(string id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(id));

            var asset = await _assetsService.GetByIdAsync(decryptedId);

            if (asset == null)
            {
                Response.StatusCode = 404;
                return View("AssetNotFound", decryptedId);
            }

            var model = new CheckoutViewModel()
            {
                LibraryCardId = "",
                AssetId = id,
                Title = asset.Title,
                ImageUrl = asset.ImageUrl,
                IsCheckedOut = await _checkout.IsCheckedOutAsync(decryptedId)
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> PlaceCheckout(string assetId, int libraryCardId)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(assetId));

            await _checkout.CheckOutItemAsync(decryptedId, libraryCardId);

            return RedirectToAction("Detail", new { id = assetId });
        }

        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> CheckIn(string id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(id));

            await _checkout.CheckInItemAsync(decryptedId);

            return RedirectToAction("Detail", new { id = id });
        }

        [HttpGet]
        [Authorize(Roles = "Patron, Employee, Admin")]
        public async Task<IActionResult> Hold(string id)
        {
            if (id == null)
            {
                return View("NoIdFound");
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(id));

            var asset = await _assetsService.GetByIdAsync(decryptedId);

            if (asset == null)
            {
                Response.StatusCode = 404;
                return View("AssetNotFound", decryptedId);
            }

            var model = new CheckoutViewModel()
            {
                LibraryCardId = "",
                AssetId = id,
                Title = asset.Title,
                ImageUrl = asset.ImageUrl,
                IsCheckedOut = await _checkout.IsCheckedOutAsync(decryptedId)
            };

            var currentholds = await _checkout.GetCurrentHoldsAsync(decryptedId);
            model.HoldCount = currentholds.Count();

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Patron, Employee, Admin")]
        public async Task<IActionResult> PlaceHold(string assetId, int libraryCardId)
        {
            int decryptedId = Convert.ToInt32(protector.Unprotect(assetId));

            if (!await _checkout.PlaceHoldAsync(decryptedId, libraryCardId))
            {
                return RedirectToAction("Hold", new { id = assetId });
            }

            var patron = await _context.Users
                .FirstOrDefaultAsync(x => x.LibraryCard.Id == libraryCardId);

            var hold = await _context.Holds
                .FirstOrDefaultAsync(x => x.LibraryCard.Id == libraryCardId && x.LibraryAsset.Id == decryptedId);


            if (hold.FirstHold == true)
            {
                BackgroundJob.Enqueue<IEmailService>(x => x.SendEmailAsync(patron.FirstName, patron.Email, "Place hold on the book",
                $"You have placed hold on the asset: '{hold.LibraryAsset.Title}' from our library. " +
                "Now you have to come to us and take the item in 24 hours time. " +
                "If you will not take the item up to this time you will not be able to borrow it."));
            }


            return RedirectToAction("Detail", new { id = assetId });
        }

        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> MarkLost(string assetId)
        {
            if (assetId == null)
            {
                return View("NoIdFound");
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(assetId));

            await _checkout.MarkLostAsync(decryptedId);

            return RedirectToAction("Detail", new { id = assetId });
        }

        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> MarkFound(string assetId)
        {
            if (assetId == null)
            {
                return View("NoIdFound");
            }

            int decryptedId = Convert.ToInt32(protector.Unprotect(assetId));

            await _checkout.MarkFoundAsync(decryptedId);

            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}