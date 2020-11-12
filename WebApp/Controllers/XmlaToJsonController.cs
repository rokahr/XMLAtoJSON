using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using static XMLAWEB.Models.XmlaToJson;
using System.Net.Http;

namespace XmlaWeb.Controllers
{
    public class XmlaToJson : Controller
    {
        // 
        // GET: /HelloWorld/

        public ActionResult Index(string clientid, string secret, string query, string workspace, string dataset)
        {
            return Ok(returnJson(clientid, secret, query, workspace, dataset));

            //return Content (returnJson(clientid, secret, query, workspace, dataset),"application/json");
        }

        // 
        // GET: /HelloWorld/Welcome/ 

        
    }
}