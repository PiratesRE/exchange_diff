using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public abstract class HttpHandlerFactoryBase<TService> : IHttpHandlerFactory
	{
		private static IHttpHandlerFactory CreateWcfHttpHandlerFactory()
		{
			IHttpHandlerFactory result;
			try
			{
				result = (IHttpHandlerFactory)Activator.CreateInstance("System.ServiceModel.Activation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", "System.ServiceModel.Activation.ServiceHttpHandlerFactory").Unwrap();
			}
			catch (Exception arg)
			{
				ExTraceGlobals.CoreTracer.TraceError<string, Exception>(0L, "An exception occurred while trying to load HttpHandlerFactory {0}. Exception: {1}.", "System.ServiceModel.Activation.ServiceHttpHandlerFactory", arg);
				result = null;
			}
			return result;
		}

		private static IHttpHandler GetWcfHttpHandler(HttpContext httpContext, string requestType, string url, string pathTranslated)
		{
			if (HttpHandlerFactoryBase<TService>.wcfHttpHandlerFactory.Member != null)
			{
				return HttpHandlerFactoryBase<TService>.wcfHttpHandlerFactory.Member.GetHandler(httpContext, requestType, url, pathTranslated);
			}
			return null;
		}

		internal abstract bool UseHttpHandlerFactory(HttpContext httpContext);

		internal abstract bool TryGetServiceMethod(string actionName, out ServiceMethodInfo methodInfo);

		internal abstract TService CreateServiceInstance();

		internal abstract IHttpAsyncHandler CreateAsyncHttpHandler(HttpContext httpContext, TService service, ServiceMethodInfo methodInfo);

		internal abstract IHttpHandler CreateHttpHandler(HttpContext httpContext, TService service, ServiceMethodInfo methodInfo);

		internal abstract string SelectOperation(string url, HttpRequest httpRequest, string requestType);

		public virtual IHttpHandler GetHandler(HttpContext httpContext, string requestType, string url, string pathTranslated)
		{
			if (this.UseHttpHandlerFactory(httpContext))
			{
				string text = this.SelectOperation(url, httpContext.Request, requestType);
				ServiceMethodInfo serviceMethodInfo;
				if (!string.IsNullOrEmpty(text) && this.TryGetServiceMethod(text, out serviceMethodInfo))
				{
					TService service = this.CreateServiceInstance();
					if (serviceMethodInfo.IsAsyncPattern)
					{
						return this.CreateAsyncHttpHandler(httpContext, service, serviceMethodInfo);
					}
					return this.CreateHttpHandler(httpContext, service, serviceMethodInfo);
				}
			}
			return HttpHandlerFactoryBase<TService>.GetWcfHttpHandler(httpContext, requestType, url, pathTranslated);
		}

		public virtual void ReleaseHandler(IHttpHandler handler)
		{
		}

		private const string wcfHttpHandlerAssemblyName = "System.ServiceModel.Activation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

		private const string wcfHttpHandlerTypeName = "System.ServiceModel.Activation.ServiceHttpHandlerFactory";

		private static LazyMember<IHttpHandlerFactory> wcfHttpHandlerFactory = new LazyMember<IHttpHandlerFactory>(() => HttpHandlerFactoryBase<TService>.CreateWcfHttpHandlerFactory());
	}
}
