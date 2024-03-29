﻿using System.Linq;
using System.Threading.Tasks;
using Library.Commands.Catalog;
using Library.Enums;
using Library.Models.Catalog;
using Library.Queries.Catalog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IMediator _mediator;

        public CatalogController(IMediator mediator)
        {
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
            var result = await _mediator.Send(new EditBookQuery(id));

            if (result == null)
            {
                return View("AssetNotFound", id);
            }

            return View(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> EditBook(AssetEditBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new EditBookCommand(model));

                if (result == null) return View("AssetNotFound", model.Id);

                return RedirectToAction("Detail", "Catalog", new { id = model.Id });
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> EditVideo(string id)
        {
            var result = await _mediator.Send(new EditVideoQuery(id));

            if (result == null)
            {
                return View("AssetNotFound", id);
            }

            return View(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> EditVideo(AssetEditVideoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new EditVideoCommand(model));

                if (result == null) return View("AssetNotFound", model.Id);

                return RedirectToAction("Detail", "Catalog", new { id = model.Id });
            }

            return View(model);
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _mediator.Send(new DeleteLibraryAssetQuery(id));

            if(result == null) return View("AssetNotFound", id);

            ViewBag.DecryptedId = result.DecryptedId;

            return View(result);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await _mediator.Send(new DeleteLibraryAssetCommand(id));

            if(result == ViewResponse.NotFound) return View("AssetNotFound", id);

            else if(result == ViewResponse.OK) return RedirectToAction("Index");

            return RedirectToAction(nameof(Delete), new { id = id });
        }


        [HttpGet]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> Checkout(string id)
        {
            var result = await _mediator.Send(new CheckoutLibraryAssetQuery(id));

            if (result == null) return View("AssetNotFound", id);

            return View(result);
        }


        [HttpPost]
        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> PlaceCheckout(string assetId, int libraryCardId)
        {
            await _mediator.Send(new CheckoutLibraryAssetCommand(assetId, libraryCardId));

            return RedirectToAction("Detail", new { id = assetId });
        }


        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> CheckIn(string id)
        {
            var result = await _mediator.Send(new CheckInLibraryAssetCommand(id));

            if(result == ViewResponse.NotFound) return View("AssetNotFound", id);

            return RedirectToAction("Detail", new { id = id });
        }


        [HttpGet]
        [Authorize(Roles = "Patron, Employee, Admin")]
        public async Task<IActionResult> Hold(string id)
        {
            var result = await _mediator.Send(new HoldLibraryAssetQuery(id));

            if(result == null) return View("AssetNotFound", id);

            return View(result);
        }


        [HttpPost]
        [Authorize(Roles = "Patron, Employee, Admin")]
        public async Task<IActionResult> PlaceHold(string assetId, int libraryCardId)
        {
            var result = await _mediator.Send(new PlaceHoldCommand(assetId, libraryCardId));

            return result == ViewResponse.OK ?
                    RedirectToAction("Detail", new { id = assetId })
                  : RedirectToAction("Hold", new { id = assetId });
        }


        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> MarkLost(string assetId)
        {
            var result = await _mediator.Send(new MarkLibraryAssetLostCommand(assetId));

            if(result == ViewResponse.NotFound) return View("AssetNotFound", assetId);

            return RedirectToAction("Detail", new { id = assetId });
        }


        [Authorize(Roles = "Admin, Employee")]
        public async Task<IActionResult> MarkFound(string assetId)
        {
            var result = await _mediator.Send(new MarkLibraryAssetFoundCommand(assetId));

            if(result == ViewResponse.NotFound) return View("AssetNotFound", assetId);

            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}