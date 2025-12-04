using Microsoft.AspNetCore.Mvc.Rendering;
using TravelOrdersApp.Domain.Entities;

namespace TravelOrdersApp.Web.Models
{
    public class HomePageViewModel
    {
        public List<SelectListItem> Emploees { get; set; } = new List<SelectListItem>();

        public List<TravelOrder> TravelOrders { get; set; } = new List<TravelOrder>();
    }
}
