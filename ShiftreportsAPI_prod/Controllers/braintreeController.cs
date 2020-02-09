using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ShiftReportApi.Controllers
{
    public class braintreeController : ApiController
    {
        // GET: api/braintree
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/braintree/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/braintree
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/braintree/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/braintree/5
        public void Delete(int id)
        {
        }
    }
}
