using CovidChart.API.Hubs;
using CovidChart.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CovidChart.API.Services
{
    public class CovidService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<CovidHub> _hubContext;

        public CovidService(AppDbContext context, IHubContext<CovidHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public IQueryable<Covid> GetList()
        {
            return _context.Covids.AsQueryable();
        }

        public async Task SaveCovid(Covid covid)
        {
            await _context.Covids.AddAsync(covid);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveCovidList", GetCovidChartList());
        }

        public List<CovidChartPivot> GetCovidChartList()
        {
            List<CovidChartPivot> covidCharts = new List<CovidChartPivot>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Select tarih, [1],[2],[3],[4],[5] from (Select [City],[Count],Cast([CovidDate] as date) as tarih from Covids) as covidTable PIVOT (Sum(Count) for City IN([1],[2],[3],[4],[5])) as pivotTable order by tarih asc";

                command.CommandType = System.Data.CommandType.Text;
                _context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CovidChartPivot covidChartPivot = new CovidChartPivot();
                        covidChartPivot.CovidDate = reader.GetDateTime(0).ToShortDateString();

                        Enumerable.Range(1, 5).ToList().ForEach(x =>
                        {
                            if (System.DBNull.Value.Equals(reader[x]))
                            {
                                covidChartPivot.Counts.Add(0);
                            }
                            else
                            {
                                covidChartPivot.Counts.Add(reader.GetInt32(x));
                            }
                        });
                        covidCharts.Add(covidChartPivot);
                    }
                }
                
            }

            _context.Database.CloseConnection();

            return covidCharts;
        }

    }
}

