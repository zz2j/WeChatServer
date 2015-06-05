using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;

namespace Wechat.Menu
{
    class Mvc
    {
        //Next we make a small *mvc.
        //author:zz2j
        //time:2015.6.3
    }

    //public abstract class RouteBase
    //{
    //    public abstract RouteData GetRouteData(HttpContextBase httpContext);
    //}

    //public class RouteData
    //{
    //    public IDictionary<string, object> Values { get; private set; }
    //    public IDictionary<string, object> DataTokens { get; private set; }
    //    public IRouteHandler RouteHandler { get; set; }
    //    public RouteBase Route { get; set; }

    //    public RouteData()
    //    {
    //        this.Values = new Dictionary<string,object>();
    //        this.DataTokens = new Dictionary<string,object>();
    //        this.DataTokens.Add("namespaces",new List<string>());
    //    }
    //    public string Controller
    //    {
    //        get{
    //            object controllerName = string.Empty;
    //            this.Values.TryGetValue("controller",out controllerName);
    //            return controllerName.ToString();
    //        }
    //    }
    //    public string ActionName
    //    {
    //        get{
    //            object actionName = string.Empty;
    //            this.Values.TryGetValue("action",out actionName);
    //            return actionName.ToString();
    //        }
    //    }
    //    public IEnumerable<string> Namespaces
    //    {
    //        get{
    //        return (IEnumerable<string>)this.DataTokens["namespaces"];
    //        }
    //    }
    //}

    //public interface IRouteHandler
    //{
    //    IHttpHandler GetHttpHandler(RequestContext requestContext);
    //}

    //public class RequestContext
    //{
    //    public virtual HttpContextBase HttpContext { get; set; }
    //    public virtual RouteData RouteData { get; set; }
    //}

    //public class Route : RouteBase
    //{
    //    public string Url { get; set; }
    //    public IDictionary<string, object> DataTokens{get;private set;}
    //    public IRouteHandler RouteHandler { get; set; }
    //    public Route()
    //    {
    //        DataTokens = new Dictionary<string,object>();
    //        RouteHandler = new MvcRouteHandler();
    //    }
    //    public override RouteData GetRouteData(HttpContextBase httpContext)
    //    {
    //        IDictionary<string,object> variables;
    //        //"~/index.aspx"
    //        if (Match(httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2), out variables))
    //        {
    //            RouteData routeData = new RouteData();
    //            foreach (var item in variables)
    //            {
    //                routeData.Values.Add(item.Key, item.Value);
    //            }
    //            foreach (var item in DataTokens)
    //            {
    //                routeData.DataTokens.Add(item.Key,item.Value);
    //            }
    //            routeData.RouteHandler = RouteHandler;
    //            return routeData;
    //        }
    //        return null;
    //        //throw new NotImplementedException();
    //    }
    //    protected bool Match(string requestUrl, out IDictionary<string, object> variables)
    //    {
    //        variables = new Dictionary<string, object>();
    //        var strArr1 = requestUrl.Split('/');
    //        var strArr2 = this.Url.Split('/');
    //        if (strArr1.Length != strArr2.Length) return false;
    //        for (int i = 0; i < strArr2.Length; i++)
    //        { 
    //            if(strArr2[i].StartsWith("{")&&strArr2[i].EndsWith("}"))
    //            {
    //                variables.Add(strArr2[i].Trim('{','}'), strArr1[i]);
    //            }
    //        }
    //        return true;
    //    }

    //    public class UrlRoutingModule : IHttpModule
    //    {
    //        public void Dispose()
    //        {
    //            //throw new NotImplementedException();
    //        }

    //        public void Init(HttpApplication context)
    //        {
    //            //hrow new NotImplementedException();
    //        }
    //        protected virtual void OnPostResolveRequestCache(object sender, EventArgs e)
    //        {
    //            HttpContextWrapper httpContext = new HttpContextWrapper(HttpContext.Current);
    //            RouteData routeData = RouteTable.Routes.GetRouteData(httpContext);
    //            if (routeData == null) return;
    //            RequestContext requestContext = new RequestContext() { RouteData = routeData, HttpContext = httpContext };
    //            IHttpHandler handler = routeData.RouteHandler.GetHttpHandler(requestContext);
    //            httpContext.RemapHandler(handler);

    //        }
    //    }
    //}
}
