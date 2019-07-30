using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bot2.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;

namespace Bot2.Controllers
{
    public class HomeController : Controller
    {
        // Global values to hold the custom settings
        private static string _OcpApimSubscriptionKey;
        private static string _KnowledgeBase;

        // Get the CustomSettings using dependency injection
        public HomeController(IOptions<CustomSettings> CustomSettings)
        {
            // Set the custom values
            _OcpApimSubscriptionKey = CustomSettings.Value.OcpApimSubscriptionKey;
            _KnowledgeBase = CustomSettings.Value.KnowledgeBase;
        }

        public async Task<ActionResult> Index(string searchString)
        {
            QnAQuery objQnAResult = new QnAQuery();

            try
            {
                if (searchString != null)
                {
                    objQnAResult = await QueryQnABot(searchString);
                }
                return View(objQnAResult);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error: " + ex);
                return View(objQnAResult);
            }
        }

        /*public IActionResult Index()
        {
            return View();
        }*/

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static async Task<QnAQuery> QueryQnABot(string Query)
        {
            QnAQuery QnAQueryResult = new QnAQuery();

            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                string RequestURI = String.Format("{0}{1}{2}{3}{4}", @"https://westus.api.cognitive.microsoft.com/", @"qnamaker/v1.0/", @"knowledgebases/", _KnowledgeBase, @"/generateAnswer");

                var httpContent = new StringContent($"{{\"question\": \"{Query}\"}}", Encoding.UTF8, "application/json");

                httpContent.Headers.Add("Ocp-Apim-Subscription-Key", _OcpApimSubscriptionKey);

                System.Net.Http.HttpResponseMessage msg = await client.PostAsync(RequestURI, httpContent);
            
                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();

                    QnAQueryResult = JsonConvert.DeserializeObject<QnAQuery>(JsonDataResponse);
                }
            }
            return QnAQueryResult;
        }
    }
}
