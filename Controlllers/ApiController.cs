using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using System;

namespace Console_MvcHttpClientApplication.MvcHttpClientApplicationModule.Controlllers
{
    
    [Route("[controller]/[action]")]
    public class ApiController : Controller
    {
        public IActionResult Index() => Redirect($"/{GetType().Name.Replace("Controller", "")}/GetApplicationModel");
        public object GetApplicationModel()
        {
            try
            {
                return JsonConvert.SerializeObject(MyApplicationModel.GetExecutingModel(), Formatting.Indented).Replace("\n","");
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
            
        }
    }
}
