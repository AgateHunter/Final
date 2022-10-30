using Final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Final.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext conext)
        {
            _context = conext;
        }
        public async Task<ActionResult> Index()
        {

            //Last 7 days of activities
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            //Total Purposeful Activities
            int TotalPurposefulActivities = SelectedTransactions
                .Where(i => i.Category.Type == "Purposeful")
                .Sum(j => j.Time);
            ViewBag.TotalPurposefulActivities = TotalPurposefulActivities.ToString() + " " + "Minutes";

            //Total Aimless Activities
            int TotalAimlessActivities = SelectedTransactions
                .Where(i => i.Category.Type == "Aimless")
                .Sum(j => j.Time);
            ViewBag.TotalAimlessActivities = TotalAimlessActivities.ToString() + " " + "Minutes";

            //Battery
            int Battery = TotalPurposefulActivities - TotalAimlessActivities;
            ViewBag.Battery = Battery.ToString();




            //Doughnut Chart - Purposeful Actions by Activity
            ViewBag.DoughnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Purposeful")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    time = k.Sum(j => j.Time),
                    formattedTime = k.Sum(j => j.Time).ToString(""),
                })
                .OrderByDescending(l => l.time)
            .ToList();


        

            //Spline Chart - Purposeful vs  Aimless 

            //Purposeful
            List<SplineChartData> PurposefulSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Purposeful")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    purposeful = k.Sum(l => l.Time)
                })
                .ToList();

            //Neutral
            List<SplineChartData> NeutralSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Neutral")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    neutral = k.Sum(l => l.Time)
                })
                .ToList();



            //Aimless
            List<SplineChartData> AimlessSummary = SelectedTransactions
                .Where(i => i.Category.Type == "Aimless")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    day = k.First().Date.ToString("dd-MMM"),
                    aimless = k.Sum(l => l.Time)
                })
                .ToList();

            //Combine Purposefu and Aimless
            string[] Last7Days = Enumerable.Range(0, 7)
                  .Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
                  .ToArray();

            ViewBag.SplineChartData = from day in Last7Days
                                      join purposeful in PurposefulSummary on day equals purposeful.day into dayPurposefulJoined
                                      from purposeful in dayPurposefulJoined.DefaultIfEmpty()
                                      join neutral in NeutralSummary on day equals neutral.day into neutralJoined
                                      from neutral in neutralJoined.DefaultIfEmpty()
                                      join aimless in AimlessSummary on day equals aimless.day into aimlessJoined
                                      from aimless in aimlessJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          purposeful = purposeful == null ? 0 : purposeful.purposeful,
                                          neutral = neutral == null ? 0 : neutral.neutral,
                                          aimless = aimless == null ? 0 : aimless.aimless,
                                      };

            //Recent Activities
            ViewBag.RecentTransactions = await _context.Transactions
                .Include(i => i.Category)
                .OrderByDescending(j => j.Date)
                .Take(8)
                .ToListAsync();


            //Line Chart - Draining Activities
            ViewBag.LineChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Aimless")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                    time = k.Sum(j => j.Time),
                    formattedTime = k.Sum(j => j.Time).ToString(""),
                })
                .OrderByDescending(l => l.time)
            .ToList();

            return View();
        }
    }
    public class SplineChartData
    {
        public string day;
        public int purposeful;
        public int neutral;
        public int aimless;

    }

 

}