using ACool_Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Filters;

namespace ACool_Backend.Attributes
{
    public class ACoolExceptionCatchAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string language = Thread.CurrentThread.CurrentUICulture.ToString();

            if (actionExecutedContext.Exception is ACoolException)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);

                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(actionExecutedContext.Exception);

                response.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                actionExecutedContext.Response = response;
            }

            base.OnException(actionExecutedContext);
        }
    }
}