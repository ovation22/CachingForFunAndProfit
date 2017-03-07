using System.Linq;
using Example.Services.Interfaces;
using Example.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Example.Web.Controllers
{
    public class ServiceCachedController : Controller
    {
        private readonly IHorseService _horseService;
        private readonly IMapper<Dto.Horse, Models.HorseSummary> _horseSummaryMapper;
        private readonly IMapper<Dto.Horse, Models.HorseDetail> _horseDetailMapper;

        public ServiceCachedController(IHorseService horseService,
            IMapper<Dto.Horse, Models.HorseSummary> horseSummaryMapper, 
            IMapper<Dto.Horse, Models.HorseDetail> horseDetailMapper)
        {
            _horseService = horseService;
            _horseSummaryMapper = horseSummaryMapper;
            _horseDetailMapper = horseDetailMapper;
        }

        public ActionResult Index()
        {
            var horses = _horseService.GetAllCached();

            var model = horses.Select(_horseSummaryMapper.Map).ToList();

            ViewBag.Message = "Service Cached";

            return View("Horses", model);
        }

        public ActionResult Detail(int id)
        {
            var horse = _horseService.GetCached(id); 

            var model = _horseDetailMapper.Map(horse);

            ViewBag.Message = "Service Cached";

            return View("Detail", model);
        }
    }
}
