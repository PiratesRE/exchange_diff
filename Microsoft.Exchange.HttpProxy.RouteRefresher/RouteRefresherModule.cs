using System;
using System.Web;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy.RouteRefresher
{
	internal class RouteRefresherModule : IHttpModule
	{
		public RouteRefresherModule()
		{
		}

		public RouteRefresherModule(IRouteRefresher routeRefresher, IRouteRefresherDiagnostics routeRefresherDiagnostics)
		{
			if (routeRefresher == null)
			{
				throw new ArgumentNullException("routeRefresher");
			}
			if (routeRefresherDiagnostics == null)
			{
				throw new ArgumentNullException("routeRefresherDiagnostics");
			}
			this.routeRefresher = routeRefresher;
			this.diagnostics = routeRefresherDiagnostics;
		}

		public void Init(HttpApplication application)
		{
			application.PreSendRequestHeaders += delegate(object sender, EventArgs args)
			{
				this.OnPreSendRequestHeaders(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
		}

		void IHttpModule.Dispose()
		{
		}

		internal void OnPreSendRequestHeaders(HttpContextBase context)
		{
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				this.OnPreSendRequestHeadersInternal(context);
			});
		}

		internal void OnPreSendRequestHeadersInternal(HttpContextBase context)
		{
			if (!HttpProxySettings.RouteRefresherEnabled.Value)
			{
				return;
			}
			this.CheckForRoutingUpdates(context);
		}

		internal void CheckForRoutingUpdates(HttpContextBase context)
		{
			HttpResponseBase response = context.Response;
			string routingUpdatesHeaderValue = response.Headers["X-RoutingEntryUpdate"];
			if (string.IsNullOrEmpty(routingUpdatesHeaderValue))
			{
				return;
			}
			response.Headers.Remove("X-RoutingEntryUpdate");
			if (this.diagnostics == null)
			{
				RequestLogger logger = RequestLogger.GetLogger(context);
				this.diagnostics = new RouteRefresherDiagnostics(logger);
			}
			if (this.routeRefresher == null)
			{
				this.routeRefresher = new RouteRefresher(this.diagnostics);
			}
			this.diagnostics.LogRouteRefresherLatency(delegate
			{
				this.routeRefresher.ProcessRoutingUpdates(routingUpdatesHeaderValue);
			});
			this.routeRefresher = null;
			this.diagnostics = null;
		}

		private IRouteRefresher routeRefresher;

		private IRouteRefresherDiagnostics diagnostics;
	}
}
