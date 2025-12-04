using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Reflection;
using TravelOrdersApp.Domain.Entities;
using TravelOrdersApp.Domain.Requests;
using TravelOrdersApp.Infrastructure.Repositories;
using TravelOrdersApp.Web.Models;

namespace TravelOrdersApp.Web.Controllers
{
    public class HomeController : Controller
    {
        readonly IEmployeeRepository _employeeRepository;
        readonly ITravelOrderRepository _travelOrderRepository;
        readonly ICityRepository _cityRepository;
        readonly ITransportRepository _transportRepository;
        readonly ITravelOrderStateRepository _travelOrderStateRepository;
        public HomeController(IEmployeeRepository employeeRepository,
            ITravelOrderRepository  travelOrderRepository,
            ICityRepository cityRepository,
            ITransportRepository transportRepository,
            ITravelOrderStateRepository travelOrderStateRepository) 
        {
            _employeeRepository = employeeRepository;
            _travelOrderRepository = travelOrderRepository;
            _cityRepository = cityRepository;
            _transportRepository = transportRepository;
            _travelOrderStateRepository = travelOrderStateRepository;
        }

        public async Task<IActionResult> Index(int? employeeId)
        {
            var model = new HomePageViewModel();
            model.Emploees = (await _employeeRepository.GetEmployeeList())
                .Select(e => new SelectListItem(e.FullName, e.Id.ToString(), selected: e.Id == employeeId))
                .ToList();

            model.TravelOrders = await _travelOrderRepository.GetTravelOrderList(employeeId.HasValue ?  new TravelOrderFilterListRequest { EmployeeId = employeeId.Value} : null); 
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddUpdate(int? id = null)
        {
            var model = new TravelOrderAddUpdateViewModel 
            {
                BusinessTripStart = DateTime.Now,
                BusinessTripEnd = DateTime.Now
            };

            if (id is not null)
            {
                var travelOrder = await _travelOrderRepository.GetTravelOrder(id.Value);
                if (travelOrder is null)
                    return NotFound();
                
                model = new TravelOrderAddUpdateViewModel(travelOrder);
            }

                model.Emploees = (await _employeeRepository.GetEmployeeList())
                .Select(e => new SelectListItem(e.FullName, e.Id.ToString()))
                .ToList();

            model.Cities = (await _cityRepository.GetCityList())
                .Select(e => new SelectListItem(e.CityName, e.Id.ToString()))
                .ToList();

            model.TravelOrderStates = (await _travelOrderStateRepository.GetTravelOrderStateList())
                .Select(e => new SelectListItem(e.Name, e.Id.ToString()))
                .ToList();

            model.Transports = (await _transportRepository.GetTransportList())
                .Select(e => new SelectListItem(e.Name, e.Id.ToString()))
                .ToList();

           


            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddUpdate(TravelOrderAddUpdateViewModel request)
        {


            if (!ModelState.IsValid)
            {
                var model = request;

                model.Emploees = (await _employeeRepository.GetEmployeeList())
                    .Select(e => new SelectListItem(e.FullName, e.Id.ToString()))
                    .ToList();

                model.Cities = (await _cityRepository.GetCityList())
                    .Select(e => new SelectListItem(e.CityName, e.Id.ToString()))
                    .ToList();

                model.TravelOrderStates = (await _travelOrderStateRepository.GetTravelOrderStateList())
                    .Select(e => new SelectListItem(e.Name, e.Id.ToString()))
                    .ToList();

                model.Transports = (await _transportRepository.GetTransportList())
                    .Select(e => new SelectListItem(e.Name, e.Id.ToString()))
                    .ToList();


                model.Errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return View(model);
            }
            if (request.Id == 0)
                await _travelOrderRepository.Add(request);
            else
                await _travelOrderRepository.Update(request);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _travelOrderRepository.Delete(id);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
