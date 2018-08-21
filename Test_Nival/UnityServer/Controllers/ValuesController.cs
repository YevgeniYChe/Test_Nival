using System;
using System.Diagnostics;
using System.Web.Http;
using UnityServer.GameLogic;
using UnityServer.Models;

namespace UnityServer.Controllers
{
    [RoutePrefix("api")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [Route("NewGame")]
        public State GetStartMap()
        {
            var MapGen = new MapGenerator();
            var state = MapGen.GetNewMap();
            return state;
        }

        //GET api/values/
        [HttpPost]
        [Route("GetStep")]
        public State GetNextStep(State state)
        {
            Debug.WriteLine(DateTime.Now.ToString());
            var Update = new PathFinder();
            var UpdState = Update.GetPath(state);
            Debug.WriteLine(DateTime.Now.ToString());
            return UpdState;
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
