using Azure.Core;
using Microsoft.AspNetCore.Mvc.Rendering;
using TravelOrdersApp.Domain.Entities;
using TravelOrdersApp.Domain.Requests;

namespace TravelOrdersApp.Web.Models
{
    public class TravelOrderAddUpdateViewModel : TravelOrderUpdateRequest
    {
        public List<SelectListItem> Emploees { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Cities { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Transports { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TravelOrderStates { get; set; } = new List<SelectListItem>();

        public List<string> Errors { get; set; } = new List<string>();

        public TravelOrderAddUpdateViewModel() { }

        public TravelOrderAddUpdateViewModel(TravelOrder travelOrder) 
        {
            EmployeeId = travelOrder.EmployeeId;
            StartingLocationCityId = travelOrder.EmployeeId;
            DestinationCityId = travelOrder.DestinationCityId;
            BusinessTripStart = travelOrder.BusinessTripStart;
            BusinessTripEnd = travelOrder.BusinessTripEnd;
            TravelOrderStateId = travelOrder.TravelOrderStateId;
            TransportIdList = travelOrder.Transports?.Select(t=>t.Id)?.ToList() ?? new List<int>();
        }

        public TravelOrderAddUpdateViewModel(TravelOrderAddRequest request) : base()
        {
            EmployeeId = request.EmployeeId;
            StartingLocationCityId = request.EmployeeId;
            DestinationCityId = request.DestinationCityId;
            BusinessTripStart = request.BusinessTripStart;
            BusinessTripEnd = request.BusinessTripEnd;
            TravelOrderStateId = request.TravelOrderStateId;
            TransportIdList = request.TransportIdList;
        }

        public TravelOrderAddUpdateViewModel(TravelOrderUpdateRequest request) : base()
        {
            Id = request.Id;
            EmployeeId = request.EmployeeId;
            StartingLocationCityId = request.EmployeeId;
            DestinationCityId = request.DestinationCityId;
            BusinessTripStart = request.BusinessTripStart;
            BusinessTripEnd = request.BusinessTripEnd;
            TravelOrderStateId = request.TravelOrderStateId;
            TransportIdList = request.TransportIdList;
        }

    }
}
