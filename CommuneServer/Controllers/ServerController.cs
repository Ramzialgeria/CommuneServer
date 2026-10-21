using System.Configuration;
using System.Web.Http;

namespace CommuneServer.Controllers
{
    [RoutePrefix("api/server")]
    public class ServerController : ApiController
    {
        [HttpGet]
        [Route("info")]
        public IHttpActionResult Info()
        {
            return Ok(new
            {
                serverCode = ConfigurationManager.AppSettings["ServerCode"],
                communeCode = ConfigurationManager.AppSettings["CommuneCode"],
                apiVersion = "1.0"
            });
        }

        [HttpGet]
        [Route("ping")]
        public IHttpActionResult Ping()
        {
            return Ok(new
            {
                success = true
            });
        }
    }
}