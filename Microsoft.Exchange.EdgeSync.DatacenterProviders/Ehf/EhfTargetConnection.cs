using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Ehf;
using Microsoft.Exchange.EdgeSync.Common;
using Microsoft.Exchange.EdgeSync.Datacenter;
using Microsoft.Exchange.EdgeSync.Logging;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal abstract class EhfTargetConnection : DatacenterTargetConnection
	{
		public EhfTargetConnection(int localServerVersion, EhfTargetServerConfig config, EnhancedTimeSpan syncInterval, EdgeSyncLogSession logSession) : base(localServerVersion, config, syncInterval, logSession, ExTraceGlobals.TargetConnectionTracer)
		{
			this.config = config;
			this.configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 104, ".ctor", "f:\\15.00.1497\\sources\\dev\\EdgeSync\\src\\EHF\\EhfTargetConnection.cs");
		}

		public EhfTargetConnection(int localServerVersion, EhfTargetServerConfig config, EdgeSyncLogSession logSession, EhfPerfCounterHandler perfCounterHandler, IProvisioningService provisioningService, IManagementService managementService, IAdminSyncService adminSyncService, EhfADAdapter adapter, EnhancedTimeSpan syncInterval) : this(localServerVersion, config, syncInterval, logSession)
		{
			this.provisioningService = new EhfProvisioningService(provisioningService, managementService, adminSyncService);
			this.adapter = adapter;
			this.perfCounterHandler = perfCounterHandler;
		}

		public EhfProvisioningService ProvisioningService
		{
			get
			{
				return this.provisioningService;
			}
		}

		public EhfADAdapter ADAdapter
		{
			get
			{
				return this.adapter;
			}
		}

		public EhfTargetServerConfig Config
		{
			get
			{
				return this.config;
			}
		}

		public EhfPerfCounterHandler PerfCounterHandler
		{
			get
			{
				return this.perfCounterHandler;
			}
		}

		protected override string LeaseFileName
		{
			get
			{
				return "ehf.lease";
			}
		}

		protected override IConfigurationSession ConfigSession
		{
			get
			{
				return this.configSession;
			}
		}

		public static ADObjectId GetCookieContainerId(IConfigurationSession configSession)
		{
			ADObjectId orgContainerId = configSession.GetOrgContainerId();
			return orgContainerId.GetChildId("Transport Settings").GetChildId("EHF Sync Cookies");
		}

		public static ADObjectId GetPerimeterConfigObjectIdFromConfigUnitId(ADObjectId configUnitId)
		{
			return configUnitId.GetChildId("Transport Settings").GetChildId("Tenant Perimeter Settings");
		}

		public virtual void AbortSyncCycle(Exception cause)
		{
			if (cause is EdgeSyncCycleFailedException)
			{
				throw cause;
			}
			throw new EdgeSyncCycleFailedException(cause);
		}

		public override bool OnSynchronizing()
		{
			if (this.perfCounterHandler == null)
			{
				this.perfCounterHandler = new EhfPerfCounterHandler();
			}
			if (this.provisioningService == null)
			{
				this.provisioningService = new EhfProvisioningService(this.config);
			}
			if (this.adapter == null)
			{
				this.adapter = new EhfADAdapter();
			}
			return true;
		}

		public override void OnConnectedToSource(Connection sourceConnection)
		{
			this.adapter.SetConnection(sourceConnection);
		}

		public override SyncResult OnRenameEntry(ExSearchResultEntry entry)
		{
			return SyncResult.None;
		}

		public override void Dispose()
		{
			if (this.provisioningService != null)
			{
				this.provisioningService.Dispose();
				this.provisioningService = null;
			}
			base.Dispose();
		}

		public override bool TryReadCookie(out Dictionary<string, Cookie> cookies)
		{
			if (!base.TryReadCookie(out cookies))
			{
				return false;
			}
			ADObjectId adobjectId;
			if (!this.TryGetConfigurationNamingContext(out adobjectId))
			{
				base.DiagSession.LogAndTraceError("Failed to get Configuration Naming context. DomainController=<{0}>", new object[]
				{
					this.ConfigSession.DomainController
				});
				return false;
			}
			Cookie cookie;
			if (!cookies.TryGetValue(adobjectId.DistinguishedName, out cookie) || cookie == null)
			{
				base.DiagSession.LogAndTraceError("Could not find the config sync cookie. This is expected only if we are trying to do a fullSync or during initial sync cycles. Cookies=<{0}>", new object[]
				{
					Util.GetCookieInformationToLog(cookies)
				});
				return true;
			}
			if (string.IsNullOrEmpty(cookie.DomainController))
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "ConfigCookie DC is null or emtpy", new object[0]);
				return true;
			}
			if (cookie.DomainController.Equals(this.ConfigSession.DomainController, StringComparison.OrdinalIgnoreCase))
			{
				base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "Edgesync is connecting to the configsync cookie DC. ConfigCookie=<{0}>; ADDriverDC=<{1}>", new object[]
				{
					cookie,
					this.ConfigSession.DomainController
				});
			}
			else
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(cookie.DomainController, true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 379, "TryReadCookie", "f:\\15.00.1497\\sources\\dev\\EdgeSync\\src\\EHF\\EhfTargetConnection.cs");
				Dictionary<string, Cookie> dictionary;
				if (base.TryReadCookie(tenantOrTopologyConfigurationSession, out dictionary))
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Medium, "Read cookies {0} from cookie Domain controller {1}.", new object[]
					{
						Util.GetCookieInformationToLog(dictionary),
						cookie.DomainController
					});
					cookies = dictionary;
				}
				else
				{
					base.DiagSession.LogAndTraceError("Failed to read cookie from config cookie DC <{0}>. Using the cookie from ADDriver DC <{1}>.", new object[]
					{
						cookie.DomainController,
						this.ConfigSession.DomainController
					});
				}
			}
			this.PointCookiesToConfigCookieDC(cookies, cookie.DomainController);
			return true;
		}

		protected override ADObjectId GetCookieContainerId()
		{
			if (EhfTargetConnection.cookieContainerId == null)
			{
				EhfTargetConnection.cookieContainerId = EhfTargetConnection.GetCookieContainerId(this.ConfigSession);
			}
			return EhfTargetConnection.cookieContainerId;
		}

		private bool TryGetConfigurationNamingContext(out ADObjectId configurationNamingContextId)
		{
			ADObjectId tempADObjectId = null;
			configurationNamingContextId = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				tempADObjectId = this.ConfigSession.ConfigurationNamingContext;
			}, 3);
			if (!adoperationResult.Succeeded)
			{
				base.DiagSession.LogAndTraceException(adoperationResult.Exception, "Failed to read Configuration Naming Context from AD", new object[0]);
			}
			else
			{
				configurationNamingContextId = tempADObjectId;
			}
			return adoperationResult.Succeeded;
		}

		private void PointCookiesToConfigCookieDC(Dictionary<string, Cookie> cookies, string configCookieDC)
		{
			foreach (Cookie cookie in cookies.Values)
			{
				if (cookie.DomainController != null && !cookie.DomainController.Equals(configCookieDC, StringComparison.OrdinalIgnoreCase))
				{
					base.DiagSession.LogAndTraceInfo(EdgeSyncLoggingLevel.Low, "CookieDC for {0} is not the same as the ConfigSync CookieDC. Setting the CookieDC to {1}. AllCookies = <{2}>", new object[]
					{
						cookie,
						configCookieDC,
						Util.GetCookieInformationToLog(cookies)
					});
					cookie.DomainController = configCookieDC;
				}
			}
		}

		public const string EhfLeaseFileName = "ehf.lease";

		private const string TransportSettingsContainerName = "Transport Settings";

		private const string PerimeterSettingsObjectName = "Tenant Perimeter Settings";

		private const string CookieContainerName = "EHF Sync Cookies";

		private static ADObjectId cookieContainerId;

		private EhfTargetServerConfig config;

		private EhfProvisioningService provisioningService;

		private EhfADAdapter adapter;

		private EhfPerfCounterHandler perfCounterHandler;

		private IConfigurationSession configSession;
	}
}
