using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageCenter.Portal
{
    public abstract class HandleExceptionAttribute : ExceptionFilterAttribute
    {
        public HandleExceptionAttribute()
        {
        }

        protected abstract bool HandleException(Exception ex);

        protected abstract ActionResult BuildAjaxJsonActionResult(Exception ex, bool isLocalRequest);

        protected abstract ActionResult BuildAjaxHtmlActionResult(Exception ex, bool isLocalRequest);

        protected abstract ActionResult BuildAjaxXmlActionResult(Exception ex, bool isLocalRequest);

        protected abstract ActionResult BuildWebPageActionResult(Exception ex, bool isLocalRequest, ExceptionContext filterContext);

        protected virtual ActionResult BuildResult(Exception ex, ExceptionContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var headers = request.GetTypedHeaders();
            bool isAjaxRequest = headers.Headers.TryGetValue("X-Requested-With", out var header) && header.ToString().ToLower() == "XMLHttpRequest".ToLower();
            bool isLocalRequest = request.Host.Host.ToLower().Contains("localhost") || request.Host.Host.Contains("127.0.0.1");
            ActionResult result;
            if (isAjaxRequest)
            {
                var acceptTypes = from at in headers.Accept select at.MediaType.ToString().ToLower();
                if (acceptTypes.Contains("application/json"))
                {
                    result = this.BuildAjaxJsonActionResult(ex,isLocalRequest);
                }
                else if (acceptTypes.Contains("text/html"))
                {
                    result = this.BuildAjaxHtmlActionResult(ex, isLocalRequest);
                }
                else
                {
                    result = this.BuildAjaxXmlActionResult(ex, isLocalRequest);
                }
            }
            else
            {
                result = this.BuildWebPageActionResult(ex, isLocalRequest, filterContext);
            }
            return result;
        }

        public override void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled)
            {
                // TODO: Pass additional detailed data via ViewData
                context.HttpContext.Response.Clear();
                context.HttpContext.Response.StatusCode = 200;
                context.ExceptionHandled = true; // mark exception as handled
                context.Result = this.BuildResult(context.Exception, context);
            }
        }
    }

    public class HandleErrorInfo
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public Exception Exception { get; set; }
    }
}
