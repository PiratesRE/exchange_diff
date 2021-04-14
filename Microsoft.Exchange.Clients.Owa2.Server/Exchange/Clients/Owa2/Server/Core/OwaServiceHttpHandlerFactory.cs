using System;
using System.Web;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaServiceHttpHandlerFactory : HttpHandlerFactoryBase<OWAService>
	{
		internal override string SelectOperation(string url, HttpRequest httpRequest, string requestType)
		{
			string text = httpRequest.Headers["Action"];
			if (string.IsNullOrEmpty(text))
			{
				string path = httpRequest.Path;
				if (!string.IsNullOrEmpty(path))
				{
					string[] array = path.Split(new char[]
					{
						'/'
					});
					if (array != null && array.Length > 0)
					{
						text = array[array.Length - 1];
					}
				}
			}
			return text;
		}

		internal override bool UseHttpHandlerFactory(HttpContext httpContext)
		{
			if (OwaServiceHttpHandlerFactory.FlightEnableOverride.Member)
			{
				return true;
			}
			if (Globals.IsAnonymousCalendarApp)
			{
				return false;
			}
			if (EsoRequest.IsEsoRequest(httpContext.Request))
			{
				return false;
			}
			UserContext userContext = UserContextManager.GetMailboxContext(httpContext, null, true) as UserContext;
			return userContext != null && userContext.FeaturesManager.ServerSettings.OwaHttpHandler.Enabled;
		}

		internal override IHttpAsyncHandler CreateAsyncHttpHandler(HttpContext httpContext, OWAService service, ServiceMethodInfo methodInfo)
		{
			return new OwaServiceHttpAsyncHandler(httpContext, service, methodInfo);
		}

		internal override IHttpHandler CreateHttpHandler(HttpContext httpContext, OWAService service, ServiceMethodInfo methodInfo)
		{
			return new OwaServiceHttpHandler(httpContext, service, methodInfo);
		}

		internal override OWAService CreateServiceInstance()
		{
			return new OWAService();
		}

		internal override bool TryGetServiceMethod(string actionName, out ServiceMethodInfo methodInfo)
		{
			return OwaServiceHttpHandlerFactory.methodMap.Member.TryGetMethodInfo(actionName, out methodInfo);
		}

		private static LazyMember<OwaServiceMethodMap> methodMap = new LazyMember<OwaServiceMethodMap>(() => new OwaServiceMethodMap(typeof(OWAService)));

		internal static LazyMember<bool> FlightEnableOverride = new LazyMember<bool>(() => BaseApplication.GetAppSetting<bool>("EnableOwaHttpHandler", false));
	}
}
