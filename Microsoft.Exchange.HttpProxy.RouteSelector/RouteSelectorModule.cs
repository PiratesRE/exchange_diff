using System;
using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.HttpProxy.Routing;
using Microsoft.Exchange.HttpProxy.Routing.Serialization;

namespace Microsoft.Exchange.HttpProxy.RouteSelector
{
	internal class RouteSelectorModule : IHttpModule
	{
		public RouteSelectorModule() : this(new RouteSelector(), null)
		{
		}

		public RouteSelectorModule(IServerLocatorFactory routeSelector, IRouteSelectorModuleDiagnostics testDiagnostics)
		{
			if (routeSelector == null)
			{
				throw new ArgumentNullException("routeSelector");
			}
			this.routeSelector = routeSelector;
			this.serverLocator = routeSelector.GetServerLocator(HttpProxyGlobals.ProtocolType);
			this.diagnostics = testDiagnostics;
		}

		internal static bool IsTesting { get; set; }

		public void Init(HttpApplication application)
		{
			application.PostAuthorizeRequest += delegate(object sender, EventArgs args)
			{
				this.OnPostAuthorizeRequest(new HttpContextWrapper(((HttpApplication)sender).Context));
			};
		}

		public void Dispose()
		{
		}

		internal void OnPostAuthorizeRequest(HttpContextBase context)
		{
			Diagnostics.SendWatsonReportOnUnhandledException(delegate()
			{
				this.OnPostAuthorizeInternal(context);
			});
		}

		private void OnPostAuthorizeInternal(HttpContextBase context)
		{
			if (!HttpProxySettings.RouteSelectorEnabled.Value && !RouteSelectorModule.IsTesting)
			{
				return;
			}
			if (!RouteSelectorModule.IsTesting)
			{
				RequestLogger logger = RequestLogger.GetLogger(context);
				this.diagnostics = new RouteSelectorDiagnostics(logger);
			}
			this.diagnostics.SaveRoutingLatency(delegate
			{
				ServerLocatorReturn serverLocatorReturn = null;
				IRoutingKey[] array = (IRoutingKey[])context.Items["RoutingKeys"];
				if (array == null)
				{
					return;
				}
				if (array.Length == 0)
				{
					return;
				}
				serverLocatorReturn = this.serverLocator.LocateServer(array, this.diagnostics);
				this.diagnostics.LogLatencies();
				this.diagnostics.ProcessLatencyPerfCounters();
				if (serverLocatorReturn != null && !string.IsNullOrEmpty(serverLocatorReturn.ServerFqdn))
				{
					if (!context.Request.IsProxyTestProbeRequest())
					{
						context.Request.Headers.Add("X-ProxyTargetServer", serverLocatorReturn.ServerFqdn);
						context.Request.Headers.Add("X-ProxyTargetServerVersion", (serverLocatorReturn.ServerVersion != null) ? serverLocatorReturn.ServerVersion.ToString() : string.Empty);
					}
					this.diagnostics.SetTargetServer(serverLocatorReturn.ServerFqdn);
					RouteSelectorDiagnostics.UpdateRoutingFailurePerfCounter(serverLocatorReturn.ServerFqdn, false);
					if (serverLocatorReturn.RoutingEntries.Count > 0)
					{
						StringBuilder stringBuilder = new StringBuilder();
						bool flag = true;
						foreach (IRoutingEntry routingEntry in serverLocatorReturn.RoutingEntries)
						{
							if (!flag)
							{
								stringBuilder.Append(",");
							}
							string value = RoutingEntryHeaderSerializer.Serialize(routingEntry);
							stringBuilder.Append(value);
							flag = false;
							this.diagnostics.AddRoutingEntry(value);
						}
						context.Request.Headers.Add("X-RoutingEntry", stringBuilder.ToString());
					}
					int versionNumber = serverLocatorReturn.ServerVersion ?? 0;
					ServerVersion serverVersion = new ServerVersion(versionNumber);
					string targetServerVersion = string.Format(CultureInfo.InvariantCulture, "{0:d}.{1:d2}.{2:d4}.{3:d3}", new object[]
					{
						serverVersion.Major,
						serverVersion.Minor,
						serverVersion.Build,
						serverVersion.Revision
					});
					this.diagnostics.SetTargetServerVersion(targetServerVersion);
					return;
				}
				string value2 = "RouteNotFoundError";
				this.diagnostics.AddErrorInfo(value2);
				context.Response.StatusCode = 500;
				bool wasFailure = true;
				foreach (IRoutingKey routingKey in array)
				{
					if (routingKey.RoutingItemType == RoutingItemType.LiveIdMemberName || routingKey.RoutingItemType == RoutingItemType.Smtp)
					{
						wasFailure = false;
						context.Response.StatusCode = 404;
						break;
					}
				}
				RouteSelectorDiagnostics.UpdateRoutingFailurePerfCounter(null, wasFailure);
				context.ApplicationInstance.CompleteRequest();
			});
		}

		private readonly IServerLocatorFactory routeSelector;

		private readonly IServerLocator serverLocator;

		private IRouteSelectorModuleDiagnostics diagnostics;
	}
}
