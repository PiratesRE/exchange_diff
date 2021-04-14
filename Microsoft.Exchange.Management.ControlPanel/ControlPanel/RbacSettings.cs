using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools.Asp.Net;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class RbacSettings
	{
		public RbacSettings(HttpContext context)
		{
			ExTraceGlobals.RBACTracer.TraceInformation<string>(0, 0L, "Extracting RBAC settings from {0}.", context.GetRequestUrlForLog());
			Guid vdirId = Guid.Empty;
			Guid.TryParse(HttpContext.Current.Request.Headers["X-vDirObjectId"], out vdirId);
			this.ecpService = new Lazy<EcpService>(delegate()
			{
				if (vdirId == Guid.Empty)
				{
					return null;
				}
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\RBAC\\RbacSettings.cs", ".ctor", 707);
				return currentServiceTopology.FindAnyCafeService<EcpService>((EcpService service) => service.ADObjectId.ObjectGuid == vdirId, "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\RBAC\\RbacSettings.cs", ".ctor", 708);
			});
			this.LogonUserIdentity = context.User.Identity;
			this.OriginalUser = context.User;
			this.IsProxyLogon = context.Request.FilePath.EndsWith("/proxyLogon.ecp", StringComparison.OrdinalIgnoreCase);
			bool flag = context.IsAcsOAuthRequest();
			if (this.IsProxyLogon)
			{
				this.ProxySecurityAccessToken = new SerializedAccessToken(context.Request.InputStream);
			}
			else
			{
				this.ProxySecurityAccessToken = null;
			}
			string logonAccountSddlSid = context.Request.Headers["msExchLogonAccount"];
			string text = context.Request.Headers["msExchLogonMailbox"];
			string targetMailboxSddlSid = context.Request.Headers["msExchTargetMailbox"];
			Server inboundProxyCaller = RbacSettings.GetInboundProxyCaller(text, this.LogonUserIdentity as WindowsIdentity);
			if (inboundProxyCaller != null)
			{
				this.IsInboundProxyRequest = true;
				this.InboundProxyCallerName = inboundProxyCaller.Name;
				EcpLogonInformation identity = EcpLogonInformation.Create(logonAccountSddlSid, text, targetMailboxSddlSid, this.ProxySecurityAccessToken);
				this.EcpIdentity = new EcpIdentity(identity, "-ProxySession");
			}
			else
			{
				this.IsInboundProxyRequest = false;
				this.InboundProxyCallerName = string.Empty;
				string explicitUser = context.GetExplicitUser();
				string targetTenant = context.GetTargetTenant();
				string text2 = string.IsNullOrEmpty(targetTenant) ? "-RbacSession" : ("-RbacSession-@" + targetTenant);
				if (flag)
				{
					text2 += "-OAuthACS";
				}
				if (!string.IsNullOrEmpty(explicitUser))
				{
					this.EcpIdentity = new EcpIdentity(context.User, explicitUser, text2);
				}
				else
				{
					this.EcpIdentity = new EcpIdentity(this.LogonUserIdentity, text2);
				}
			}
			this.UserUniqueKeyForCanary = this.GetUserUniqueKey();
			this.IsExplicitSignOn = this.EcpIdentity.IsExplicitSignon;
			bool flag2 = null == context.Request.Cookies[RbacModule.SessionStateCookieName];
			if (flag2 && !flag)
			{
				context.Response.Cookies.Add(new HttpCookie(RbacModule.SessionStateCookieName, Guid.NewGuid().ToString())
				{
					HttpOnly = true
				});
				this.CacheKey = this.GetCacheKey();
				this.ExpireSession();
			}
			else
			{
				this.CacheKey = this.GetCacheKey();
			}
			ExTraceGlobals.RBACTracer.TraceInformation(0, 0L, "RBAC Settings for {0}: UserName: {1}, IsNewBrowserWindow={2}, IsInboundProxyRequest={3}, InboundProxyCallerName={4}, HasCachedSession={5}", new object[]
			{
				context.GetRequestUrlForLog(),
				this.UserName,
				flag2,
				this.IsInboundProxyRequest,
				this.InboundProxyCallerName,
				this.CachedSession != null
			});
		}

		public IIdentity OriginalLogonUserIdentity
		{
			get
			{
				return this.EcpIdentity.LogonUserIdentity;
			}
		}

		public string UserUniqueKeyForCanary { get; private set; }

		public IPrincipal OriginalUser { get; private set; }

		public bool HasFullAccess
		{
			get
			{
				return this.EcpIdentity.HasFullAccess;
			}
		}

		public bool LogonUserEsoSelf
		{
			get
			{
				return this.EcpIdentity.LogonUserEsoSelf;
			}
		}

		public string UserName
		{
			get
			{
				return this.EcpIdentity.UserName;
			}
		}

		public string TenantNameForMonitoringPurpose
		{
			get
			{
				ExchangePrincipal accessedUserExchangePrincipal = this.GetAccessedUserExchangePrincipal();
				if (accessedUserExchangePrincipal != null && accessedUserExchangePrincipal.MailboxInfo.OrganizationId != null)
				{
					ADObjectId organizationalUnit = accessedUserExchangePrincipal.MailboxInfo.OrganizationId.OrganizationalUnit;
					if (organizationalUnit != null)
					{
						return organizationalUnit.Name;
					}
				}
				return null;
			}
		}

		public IIdentity AccessedUserIdentity
		{
			get
			{
				return this.EcpIdentity.AccessedUserIdentity;
			}
		}

		public SecurityIdentifier AccessedUserSid
		{
			get
			{
				return this.EcpIdentity.AccessedUserSid;
			}
		}

		public RbacSession CachedSession
		{
			get
			{
				return (RbacSession)HttpRuntime.Cache[this.CacheKey];
			}
		}

		public RbacSession Session
		{
			get
			{
				RbacSession rbacSession = this.CachedSession;
				if (rbacSession == null)
				{
					object obj = null;
					lock (RbacSettings.perUserLocks)
					{
						if (!RbacSettings.perUserLocks.TryGetValue(this.CacheKey, out obj))
						{
							obj = new object();
							RbacSettings.perUserLocks.Add(this.CacheKey, obj);
						}
					}
					lock (obj)
					{
						rbacSession = this.CachedSession;
						if (rbacSession == null)
						{
							rbacSession = this.CreateSession();
						}
					}
					lock (RbacSettings.perUserLocks)
					{
						RbacSettings.perUserLocks.Remove(this.CacheKey);
					}
				}
				return rbacSession;
			}
		}

		public bool AdminEnabled
		{
			get
			{
				return Util.IsDataCenter || (this.ecpService.Value != null && this.ecpService.Value.AdminEnabled);
			}
		}

		public bool OwaOptionsEnabled
		{
			get
			{
				return Util.IsDataCenter || this.ecpService.Value == null || this.ecpService.Value.OwaOptionsEnabled;
			}
		}

		internal static void AddSessionToCache(string cacheKey, RbacSession session, bool canRemove, bool isNewSession)
		{
			Cache cache = HttpRuntime.Cache;
			CacheDependency dependencies = null;
			DateTime absoluteExpiration = (DateTime)ExDateTime.UtcNow.Add(RbacSection.Instance.RbacPrincipalMaximumAge);
			TimeSpan noSlidingExpiration = Cache.NoSlidingExpiration;
			CacheItemPriority priority = canRemove ? CacheItemPriority.High : CacheItemPriority.NotRemovable;
			CacheItemRemovedCallback onRemoveCallback;
			if (!isNewSession)
			{
				onRemoveCallback = null;
			}
			else
			{
				onRemoveCallback = delegate(string key, object value, CacheItemRemovedReason reason)
				{
					((RbacSession)value).SessionEnd();
				};
			}
			cache.Insert(cacheKey, session, dependencies, absoluteExpiration, noSlidingExpiration, priority, onRemoveCallback);
		}

		public void ExpireSession()
		{
			string key = this.CacheKey + "_Regional";
			if (HttpRuntime.Cache[key] != null)
			{
				HttpRuntime.Cache.Remove(key);
			}
			if (this.CachedSession != null)
			{
				HttpRuntime.Cache.Remove(this.CacheKey);
			}
		}

		internal string GetCacheKey()
		{
			string result;
			if (!Util.IsDataCenter)
			{
				Uri requestUrl = HttpContext.Current.GetRequestUrl();
				result = string.Concat(new object[]
				{
					this.EcpIdentity.GetCacheKey(),
					"_",
					HttpContext.Current.GetSessionID(),
					"_",
					requestUrl.Host,
					"_",
					requestUrl.Port
				});
			}
			else
			{
				result = this.EcpIdentity.GetCacheKey();
			}
			return result;
		}

		internal ExchangePrincipal GetAccessedUserExchangePrincipal()
		{
			return this.EcpIdentity.GetAccessedUserExchangePrincipal();
		}

		internal ExchangePrincipal GetLogonUserExchangePrincipal()
		{
			return this.EcpIdentity.GetLogonUserExchangePrincipal();
		}

		private static Server GetInboundProxyCaller(string securityContextHeader, WindowsIdentity identity)
		{
			if (string.IsNullOrEmpty(securityContextHeader))
			{
				ExTraceGlobals.ProxyTracer.TraceInformation(0, 0L, "Request does not carry a security context header.");
				return null;
			}
			if (identity == null)
			{
				ExTraceGlobals.ProxyTracer.TraceInformation(0, 0L, "Request does not carry a WindowsIdentity.");
				throw new CmdletAccessDeniedException(Strings.ProxyRequiresWindowsAuthentication);
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1177, "GetInboundProxyCaller", "f:\\15.00.1497\\sources\\dev\\admin\\src\\ecp\\RBAC\\RbacSettings.cs");
			topologyConfigurationSession.UseConfigNC = false;
			topologyConfigurationSession.UseGlobalCatalog = true;
			ADComputer adcomputer = topologyConfigurationSession.FindComputerBySid(identity.User);
			if (adcomputer == null)
			{
				ExTraceGlobals.ProxyTracer.TraceInformation<SecurityIdentifier>(0, 0L, "Identity in the request is not from an AD computer. {0}", identity.User);
				throw new CmdletAccessDeniedException(Strings.ProxyRequiresCallerToBeCAS);
			}
			topologyConfigurationSession.UseConfigNC = true;
			Server server = topologyConfigurationSession.FindServerByName(adcomputer.Name);
			if (server == null)
			{
				ExTraceGlobals.ProxyTracer.TraceInformation<string>(0, 0L, "Identity in the request is a computer but not an Exchange server. {0}", adcomputer.Name);
				throw new CmdletAccessDeniedException(Strings.ProxyRequiresCallerToBeCAS);
			}
			if (!server.IsClientAccessServer && !server.IsFfoWebServiceRole && !server.IsCafeServer && !server.IsOSPRole)
			{
				ExTraceGlobals.ProxyTracer.TraceInformation<string>(0, 0L, "Exchange Server {0} is not a Client Access Server.", server.Name);
				throw new CmdletAccessDeniedException(Strings.ProxyRequiresCallerToBeCAS);
			}
			if (!server.IsE14OrLater)
			{
				ExTraceGlobals.ProxyTracer.TraceInformation<string, string>(0, 0L, "Exchange Server {0} is not a E14 or later. {1}", server.Name, server.SerialNumber);
				throw new CmdletAccessDeniedException(Strings.ProxyRequiresCallerToBeCAS);
			}
			ExTraceGlobals.ProxyTracer.TraceInformation<string>(0, 0L, "Detected an inbound proxy call from server {0}.", server.Name);
			return server;
		}

		private RbacSession CreateSession()
		{
			RbacSession result;
			using (new AverageTimePerfCounter(EcpPerfCounters.AverageRbacSessionCreation, EcpPerfCounters.AverageRbacSessionCreationBase, true))
			{
				using (EcpPerformanceData.CreateRbacSession.StartRequestTimer())
				{
					RbacContext rbacContext = new RbacContext(this);
					RbacSession rbacSession = rbacContext.CreateSession();
					RbacSettings.AddSessionToCache(this.CacheKey, rbacSession, true, true);
					rbacSession.SessionStart();
					result = rbacSession;
				}
			}
			return result;
		}

		private string GetUserUniqueKey()
		{
			DelegatedPrincipal delegatedPrincipal = this.OriginalUser as DelegatedPrincipal;
			if (delegatedPrincipal != null)
			{
				return delegatedPrincipal.UserId;
			}
			if (DatacenterRegistry.IsForefrontForOffice())
			{
				return this.OriginalLogonUserIdentity.Name;
			}
			return this.OriginalLogonUserIdentity.GetSecurityIdentifier().Value;
		}

		public readonly bool IsProxyLogon;

		public readonly SerializedAccessToken ProxySecurityAccessToken;

		public readonly bool IsInboundProxyRequest;

		public readonly string InboundProxyCallerName;

		public readonly IIdentity LogonUserIdentity;

		public readonly bool IsExplicitSignOn;

		public readonly string CacheKey;

		private readonly EcpIdentity EcpIdentity;

		private static readonly Dictionary<string, object> perUserLocks = new Dictionary<string, object>();

		private Lazy<EcpService> ecpService;
	}
}
