using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.HttpProxy
{
	internal sealed class HealthCheckResponder
	{
		private HealthCheckResponder()
		{
		}

		public static HealthCheckResponder Instance
		{
			get
			{
				if (HealthCheckResponder.instance == null)
				{
					lock (HealthCheckResponder.staticLock)
					{
						if (HealthCheckResponder.instance == null)
						{
							HealthCheckResponder.instance = new HealthCheckResponder();
						}
					}
				}
				return HealthCheckResponder.instance;
			}
		}

		public bool IsHealthCheckRequest(HttpContext httpContext)
		{
			return httpContext.Request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) && httpContext.Request.Url.AbsolutePath.EndsWith(Constants.HealthCheckPage, StringComparison.OrdinalIgnoreCase);
		}

		public void CheckHealthStateAndRespond(HttpContext httpContext)
		{
			if (!HealthCheckResponder.HealthCheckResponderEnabled.Value)
			{
				this.RespondSuccess(httpContext);
			}
			else
			{
				ServerComponentEnum serverComponent = ServerComponentEnum.None;
				if (!HealthCheckResponder.ProtocolServerComponentMap.TryGetValue(HttpProxyGlobals.ProtocolType, out serverComponent))
				{
					throw new InvalidOperationException("Unknown protocol type " + HttpProxyGlobals.ProtocolType);
				}
				if (HealthCheckResponder.HealthCheckResponderServerComponentOverride.Value != ServerComponentEnum.None)
				{
					serverComponent = HealthCheckResponder.HealthCheckResponderServerComponentOverride.Value;
				}
				DateTime utcNow = DateTime.UtcNow;
				if (this.componentStateNextLookupTime <= utcNow)
				{
					this.isComponentOnline = ServerComponentStateManager.IsOnline(serverComponent);
					this.componentStateNextLookupTime = utcNow.AddSeconds(15.0);
				}
				if (!this.isComponentOnline)
				{
					this.RespondFailure(httpContext);
				}
				else
				{
					this.RespondSuccess(httpContext);
				}
			}
			httpContext.ApplicationInstance.CompleteRequest();
		}

		private void RespondSuccess(HttpContext httpContext)
		{
			PerfCounters.HttpProxyCountersInstance.LoadBalancerHealthChecks.RawValue = 1L;
			httpContext.Response.StatusCode = 200;
			httpContext.Response.Write(Constants.HealthCheckPageResponse);
			httpContext.Response.Write("<br/>");
			httpContext.Response.Write(HttpProxyGlobals.LocalMachineFqdn.Member);
		}

		private void RespondFailure(HttpContext httpContext)
		{
			PerfCounters.HttpProxyCountersInstance.LoadBalancerHealthChecks.RawValue = 0L;
			httpContext.Response.Close();
		}

		private const int ComponentStateLookupTimeIntervalInSeconds = 15;

		private static readonly BoolAppSettingsEntry HealthCheckResponderEnabled = new BoolAppSettingsEntry(HttpProxySettings.Prefix("HealthCheckResponderEnabled"), true, ExTraceGlobals.VerboseTracer);

		private static readonly EnumAppSettingsEntry<ServerComponentEnum> HealthCheckResponderServerComponentOverride = new EnumAppSettingsEntry<ServerComponentEnum>(HttpProxySettings.Prefix("HealthCheckResponderServerComponentOverride"), ServerComponentEnum.None, ExTraceGlobals.VerboseTracer);

		private static readonly Dictionary<ProtocolType, ServerComponentEnum> ProtocolServerComponentMap = new Dictionary<ProtocolType, ServerComponentEnum>
		{
			{
				ProtocolType.Autodiscover,
				ServerComponentEnum.AutoDiscoverProxy
			},
			{
				ProtocolType.Eas,
				ServerComponentEnum.ActiveSyncProxy
			},
			{
				ProtocolType.Ecp,
				ServerComponentEnum.EcpProxy
			},
			{
				ProtocolType.Ews,
				ServerComponentEnum.EwsProxy
			},
			{
				ProtocolType.Mapi,
				ServerComponentEnum.MapiProxy
			},
			{
				ProtocolType.Oab,
				ServerComponentEnum.OabProxy
			},
			{
				ProtocolType.Owa,
				ServerComponentEnum.OwaProxy
			},
			{
				ProtocolType.OwaCalendar,
				ServerComponentEnum.OwaProxy
			},
			{
				ProtocolType.PushNotifications,
				ServerComponentEnum.PushNotificationsProxy
			},
			{
				ProtocolType.PowerShell,
				ServerComponentEnum.RpsProxy
			},
			{
				ProtocolType.PowerShellLiveId,
				ServerComponentEnum.RpsProxy
			},
			{
				ProtocolType.ReportingWebService,
				ServerComponentEnum.RwsProxy
			},
			{
				ProtocolType.RpcHttp,
				ServerComponentEnum.RpcProxy
			},
			{
				ProtocolType.Xrop,
				ServerComponentEnum.XropProxy
			}
		};

		private static HealthCheckResponder instance = null;

		private static object staticLock = new object();

		private bool isComponentOnline;

		private DateTime componentStateNextLookupTime = DateTime.UtcNow;
	}
}
