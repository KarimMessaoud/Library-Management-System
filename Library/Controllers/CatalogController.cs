using System;
using System.Linq;
using Library.Models.Catalog;
using Library.Security;
using LibraryData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly IDataProtector protector;

        public CatalogController(
                        ILibraryAssetService assetsService,
                        IDataProtectionProvider dataProtectionProvider,
                        DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _assetsService = assetsService;
            protector = dataProtectionProvider.CreateProtector(dataProtectionPurposeStrings.AssetIdRouteValue);
        }

        [AllowAnonymous]
        public IActionResult Index(string searchString)
        {
            var assetModels = _assetsService.GetAll()
                .Select(x => 
                {
                    x.EncryptedId = protector.Protect(x.Id.ToString());
                    return x;
                })
                .OrderBy(x => x.Title);


            var listingResult = assetModels
                .Select(a => new AssetIndexListingViewModel
                {
                    Id = a.EncryptedId,
                    ImageUrl = a.ImageUrl,
                    AuthorOrDirector = _assetsService.GetAuthorOrDirector(a.Id),
                    Title = _assetsService.GetTitle(a.Id),
                    Type = _assetsService.GetType(a.Id)
                }).ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                listingResult = listingResult
                    .Where(x => x.Title.ToUpper().Contains(searchString.ToUpper()))
                    .ToList();
            }

            var model = new AssetIndexViewModel
            {
                Assets = listingResult
            };

            return View(model);
        }
    }
}