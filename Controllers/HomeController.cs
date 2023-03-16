using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WeatherProject.Data;
using WeatherProject.Models;

namespace WeatherProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly WeatherProjectContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, WeatherProjectContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Validation(string username, string password)
        {
            //Verificar se o user e pass estão corretos
            if(username=="admin@admin.pt" && password=="1234")
            {
                Weather wm = new Weather();
                wm.period = "Today";
                return View("Main",wm);
            }
            else
            {
                ViewData["auth_error"] = "Username e/ou password inválidos";
                return View("Index");
            }
        }

        public IActionResult Results()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> WeatherAPIForm(Weather wm)
        {
            //Check the intended period
            string API_URL;
            switch(wm.Period)
            {
                case "Today":
                    API_URL = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/" + wm.LocationName + "/today?unitGroup=metric&key=KT998THUCKM395HUR6LLQRWYH&contentType=json";
                    break;
                case "Tomorrow":
                    API_URL = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/" + wm.LocationName + "/tomorrow?unitGroup=metric&key=KT998THUCKM395HUR6LLQRWYH&contentType=json";
                    break;
                case "Next 7 days":
                    API_URL = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/" + wm.LocationName + "/next7days?unitGroup=metric&key=KT998THUCKM395HUR6LLQRWYH&contentType=json";
                    break;
                default:
                    API_URL = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/" + wm.LocationName + "/today?unitGroup=metric&key=KT998THUCKM395HUR6LLQRWYH&contentType=json";
                    break;
            }

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, API_URL);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode(); // Throw an exception if error
            var body = await response.Content.ReadAsStringAsync();

            dynamic weather = JsonConvert.DeserializeObject(body);
            List<string> results = new List<string>();
            List<string> stationIds = new List<string>();
            foreach (var day in weather.days)
            {
                results.Add("Forecast for date: " + day.datetime);
                results.Add(" General conditions: " + day.description);
                results.Add(" The high temperature will be: " + day.tempmax);
                results.Add(" The low temperature will be: " + day.tempmin);
                results.Add("");
                //Insert into database
                Forecast forecastRecord = new Forecast();
                forecastRecord.Date = day.datetime;
                forecastRecord.Description = day.description;
                forecastRecord.TempMax = day.tempmax;
                forecastRecord.TempMin = day.tempmin;
                if (ModelState.IsValid)
                {
                    _context.Add(forecastRecord);
                    await _context.SaveChangesAsync();
                }
            }

            results.Add("------------------------------------");
            results.Add("Here is the information about the associated stations for this location:");
            var weatherStations = weather.currentConditions["stations"];
            string stationDetails = "";
            foreach (string stationID in weatherStations)
            {
                //Station details are distance, latitude, longitude, useCount, id, name, quality, contribution
                results.Add("Station ID: " + weather.stations[stationID].id);
                results.Add("Distance: " + weather.stations[stationID].distance);
                results.Add("Latitude: " + weather.stations[stationID].latitude);
                results.Add("Longitude: " + weather.stations[stationID].longitude);
                results.Add("Use Count: " + weather.stations[stationID].useCount);
                results.Add("Name: " + weather.stations[stationID].name);
                results.Add("Quality: " + weather.stations[stationID].quality);
                results.Add("Contribution: " + weather.stations[stationID].contribution);
                results.Add("");
            }         
            ViewBag.Output = results;
            return View("Results", wm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}