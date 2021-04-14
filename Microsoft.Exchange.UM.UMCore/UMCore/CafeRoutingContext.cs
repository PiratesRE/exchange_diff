using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CafeRoutingContext : DisposableBase, IRoutingContext
	{
		public SipRoutingHelper RoutingHelper { get; internal set; }

		public string CallId
		{
			get
			{
				return this.CallInfo.CallId;
			}
		}

		public UMDialPlan DialPlan { get; set; }

		public bool IsSecuredCall
		{
			get
			{
				return this.CallInfo.RemoteCertificate != null;
			}
		}

		public PlatformSipUri RequestUriOfCall
		{
			get
			{
				return this.CallInfo.RequestUri;
			}
		}

		public PlatformSipUri RedirectUri { get; set; }

		public int RedirectCode
		{
			get
			{
				return this.RoutingHelper.RedirectResponseCode;
			}
		}

		public bool IsDiagnosticCall { get; private set; }

		public bool IsActiveMonitoring { get; private set; }

		public PlatformCallInfo CallInfo { get; private set; }

		public DiagnosticHelper Tracer { get; private set; }

		public string ReferredByHeader { get; private set; }

		public bool IsDivertedCall
		{
			get
			{
				return this.CallInfo.DiversionInfo.Count > 0;
			}
		}

		public string RemoteMatchedFqdn
		{
			get
			{
				return this.remoteMatchedFqdn;
			}
			set
			{
				this.remoteMatchedFqdn = value;
				this.RemotePeer = (string.IsNullOrEmpty(this.remoteMatchedFqdn) ? new UMSmartHost(this.CallInfo.RemotePeer.ToString()) : new UMSmartHost(this.remoteMatchedFqdn));
			}
		}

		public UMIPGateway Gateway { get; set; }

		public UMAutoAttendant AutoAttendant { get; set; }

		public UMHuntGroup HuntGroup { get; set; }

		public IADSystemConfigurationLookup ScopedADConfigurationSession { get; set; }

		public PhoneNumber CalledParty { get; set; }

		public PhoneNumber CallingParty { get; set; }

		public UMRecipient DivertedUser { get; set; }

		public Guid TenantGuid { get; set; }

		public bool IsAnonymousCaller
		{
			get
			{
				return UtilityMethods.IsAnonymousNumber(this.CallInfo.RequestUri.User);
			}
		}

		public string PilotNumber
		{
			get
			{
				if (!this.IsAnonymousCaller)
				{
					return this.CallInfo.RequestUri.User;
				}
				return null;
			}
		}

		public UMADSettings CallRouterConfiguration { get; private set; }

		public PlatformSipUri ToUri
		{
			get
			{
				return this.CallInfo.CalledParty.Uri;
			}
		}

		public PlatformSipUri FromUri
		{
			get
			{
				return this.CallInfo.CallingParty.Uri;
			}
		}

		public bool IsAccessProxyCall
		{
			get
			{
				return this.RoutingHelper.SupportsMsOrganizationRouting;
			}
		}

		public UMSmartHost RemotePeer { get; private set; }

		public string UMPodRedirectTemplate { get; set; }

		internal ExDateTime CallReceivedTime
		{
			get
			{
				return this.callStartTime;
			}
		}

		private ExDateTime CallFinishTime
		{
			get
			{
				return this.callFinishTime;
			}
			set
			{
				this.callFinishTime = value;
			}
		}

		internal TimeSpan CallLatency
		{
			get
			{
				return this.CallFinishTime.UniversalTime.Subtract(this.CallReceivedTime.UniversalTime);
			}
		}

		public CafeRoutingContext(PlatformCallInfo callInfo, UMADSettings config)
		{
			ValidateArgument.NotNull(callInfo, "PlatformCallInfo");
			ValidateArgument.NotNull(config, "UMADSettings");
			this.CallRouterConfiguration = config;
			this.UMPodRedirectTemplate = this.CallRouterConfiguration.UMPodRedirectTemplate;
			this.IsDiagnosticCall = SipPeerManager.Instance.IsLocalDiagnosticCall(callInfo.RemotePeer, callInfo.RemoteHeaders);
			this.IsActiveMonitoring = (!string.IsNullOrEmpty(callInfo.RemoteUserAgent) && callInfo.RemoteUserAgent.IndexOf("ActiveMonitoringClient", StringComparison.OrdinalIgnoreCase) > 0);
			this.RoutingHelper = SipRoutingHelper.Create(callInfo);
			this.Tracer = new DiagnosticHelper(this, ExTraceGlobals.UMCallRouterTracer);
			this.CallInfo = callInfo;
			this.remoteMatchedFqdn = (callInfo.RemoteMatchedFQDN ?? string.Empty);
			this.RemotePeer = (string.IsNullOrEmpty(this.remoteMatchedFqdn) ? new UMSmartHost(this.CallInfo.RemotePeer.ToString()) : new UMSmartHost(this.remoteMatchedFqdn));
			this.ReferredByHeader = RouterUtils.GetReferredByHeader(this.CallInfo.RemoteHeaders);
			this.Tracer.Trace("RouterCallHandler : CallId:{0}, remoteEP: {1}, remoteFqdn: {2}, RoutingHelperType {3}", new object[]
			{
				callInfo.CallId,
				callInfo.RemotePeer,
				string.IsNullOrEmpty(callInfo.RemoteMatchedFQDN) ? "null" : callInfo.RemoteMatchedFQDN,
				this.RoutingHelper.GetType().Name
			});
		}

		internal void AddDiagnosticsTimer(Timer timer)
		{
			if (timer != null)
			{
				this.diagnosticsTimers.Add(timer);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.CallFinishTime = ExDateTime.UtcNow;
				CafeLoggingHelper.LogCallStatistics(this);
				if (this.DivertedUser != null)
				{
					this.DivertedUser.Dispose();
					this.DivertedUser = null;
				}
				this.diagnosticsTimers.ForEach(delegate(Timer o)
				{
					o.Dispose();
				});
				this.diagnosticsTimers.Clear();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CafeRoutingContext>(this);
		}

		private string remoteMatchedFqdn;

		private List<Timer> diagnosticsTimers = new List<Timer>(2);

		private ExDateTime callStartTime = ExDateTime.UtcNow;

		private ExDateTime callFinishTime = ExDateTime.UtcNow;
	}
}
