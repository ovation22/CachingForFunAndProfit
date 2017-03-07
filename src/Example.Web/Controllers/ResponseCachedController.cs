using Example.Services.Interfaces;
using Example.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Example.Web.Controllers
{
    public class ResponseCachedController : Controller
    {
        private readonly IHorseService _horseService;
        private readonly IMapper<Dto.Horse, Models.HorseSummary> _horseSummaryMapper;
        private readonly IMapper<Dto.Horse, Models.HorseDetail> _horseDetailMapper;

        public ResponseCachedController(IHorseService horseService,
            IMapper<Dto.Horse, Models.HorseSummary> horseSummaryMapper,
            IMapper<Dto.Horse, Models.HorseDetail> horseDetailMapper)
        {
            _horseService = horseService;
            _horseSummaryMapper = horseSummaryMapper;
            _horseDetailMapper = horseDetailMapper;
        }       

        [ResponseCache(Duration=20)]
        public ActionResult Index()
        {
            var horses = _horseService.GetAll();

            var model = horses.Select(_horseSummaryMapper.Map).ToList();

            ViewBag.Message = "Client Cached";

            return View("Horses", model);
        }       

        [ResponseCache(Duration = 20)]
        public ActionResult Detail(int id)
        {
            var horse = _horseService.Get(id);

            var model = _horseDetailMapper.Map(horse);

            ViewBag.Message = "Client Cached";

            return View("Detail", model);
        }
    }
}
