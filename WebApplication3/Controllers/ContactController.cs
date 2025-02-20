using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication3.Models;
using WebApplication3.Models.Services;

namespace WebApplication3.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View(_contactService.GetAll());
        }
        
        [Authorize(Roles = "admin")]
        public IActionResult Add()
        {
            ContactModel model = new ContactModel();
            model.Organizations = _contactService.GetOrganizations()
                .Select(e => new SelectListItem()
                {
                    Text = e.Name,
                    Value = e.Id.ToString()
                }).ToList();
            return View(model);
        }
        
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Add(ContactModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            _contactService.Add(model);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id, ContactModel model)
        {
            _contactService.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            return View(_contactService.GetById(id));
        }

        public IActionResult Details(int id)
        {
            return View(_contactService.GetById(id));
        }
    }
}