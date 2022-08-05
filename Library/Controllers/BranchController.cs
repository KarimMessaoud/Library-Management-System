using System.Threading.Tasks;
using Library.Queries.Branch;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [AllowAnonymous]
    public class BranchController : Controller
    {
        private readonly IMediator _mediator;

        public BranchController(IMediator mediator)
        {
            _mediator = mediator;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _mediator.Send(new GetAllBranchesQuery()));
        }


        public async Task<IActionResult> Detail(int? id)
        {
            var result = await _mediator.Send(new GetBranchByIdQuery(id.Value));

            if(result == null) return View("BranchNotFound", id);

            return View(result);
        }
    }
}