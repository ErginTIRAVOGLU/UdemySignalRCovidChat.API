using CovidChart.API.Models;
using CovidChart.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CovidChart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidsController : ControllerBase
    {
        private readonly CovidService _covidService;

        public CovidsController(CovidService covidService)
        {
            _covidService = covidService;
        }


        [HttpPost]
        public async Task<IActionResult> SaveCovid(Covid covid)
        {
            await _covidService.SaveCovid(covid);
            //IQueryable<Covid> covidList = _covidService.GetList();
            return Ok(_covidService.GetCovidChartList());
        }

        [HttpGet]
        public async Task<IActionResult> InitializeCovid()
        {
            Random rnd = new Random();
            foreach (var item in Enumerable.Range(1, 10))
            {
                foreach (ECity city in Enum.GetValues(typeof(ECity)))
                {
                    var newCovid = new Covid
                    {
                        City = city,
                        Count = rnd.Next(100, 1000),
                        CovidDate = DateTime.Now.AddDays(item),
                    };

                    await _covidService.SaveCovid(newCovid);
                   
                }
                System.Threading.Thread.Sleep(1000);
            }

            return Ok("Covid dataları kayıt edildi.");
        }
    }
}
