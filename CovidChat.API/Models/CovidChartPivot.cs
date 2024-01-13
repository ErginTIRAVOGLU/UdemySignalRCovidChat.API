namespace CovidChart.API.Models
{
    public class CovidChartPivot
    {
        public CovidChartPivot()
        {
            Counts = new List<int>();
        }
        public string CovidDate { get; set; }
        public List<int> Counts { get; set; }
    }
}
