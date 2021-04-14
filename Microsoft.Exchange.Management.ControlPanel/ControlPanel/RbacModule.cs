using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class RbacModule : IHttpModule
	{
		static RbacModule()
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			string configStringValue;
			if (Util.IsDataCenter)
			{
				configStringValue = AppConfigLoader.GetConfigStringValue("IframeablePagesForDataCenter", null);
			}
			else
			{
				configStringValue = AppConfigLoader.GetConfigStringValue("IframeablePagesForOnPremises", null);
			}
			if (!string.IsNullOrEmpty(configStringValue))
			{
				if (configStringValue == "*")
				{
					RbacModule.bypassXFrameOptions = true;
				}
				else
				{
					RbacModule.bypassXFrameOptions = false;
					foreach (string item in configStringValue.Split(new char[]
					{
						','
					}))
					{
						hashSet.Add(item);
					}
				}
			}
			RbacModule.xFrameOptionsExceptionList = hashSet;
		}

		public void Init(HttpApplication application)
		{
			lock (RbacModule.initSync)
			{
				if (!RbacModule.initialized)
				{
					RbacModule.initialized = true;
					RbacModule.RegisterQueryProcessors();
				}
			}
			application.PostAuthenticateRequest += this.Application_PostAuthenticateRequest;
			application.EndRequest += this.Application_EndRequest;
		}

		internal static void RegisterQueryProcessors()
		{
			OrganizationCacheQueryProcessor<bool>.XPremiseEnt.Register();
			OrganizationCacheQueryProcessor<bool>.XPremiseDC.Register();
			RbacQuery.RegisterQueryProcessor("ClosedCampus", new ClosedCampusQueryProcessor());
			RbacQuery.RegisterQueryProcessor("OrgHasManagedDomains", new AcceptedManagedDomainsQueryProcessor());
			RbacQuery.RegisterQueryProcessor("OrgHasFederatedDomains", new AcceptedFederatedDomainsQueryProcessor());
			RbacQuery.RegisterQueryProcessor("SendAddressAvailable", new SendAddressAvailableQueryProcessor());
			RbacQuery.RegisterQueryProcessor("PopImapDisabled", new PopImapDisabledQueryProcessor());
			RbacQuery.RegisterQueryProcessor("IPSafelistingEhfSyncEnabledRole", new IPSafelistingEhfSyncEnabledQueryProcessor());
			RbacQuery.RegisterQueryProcessor("IPSafelistingSmpEnabledRole", new IPSafelistingSmpEnabledQueryProcessor());
			RbacQuery.RegisterQueryProcessor("EhfAdminCenterEnabledRole", new EhfAdminCenterEnabledQueryProcessor());
			RbacQuery.RegisterQueryProcessor("BusinessLiveId", LiveIdInstanceQueryProcessor.BusinessLiveId);
			RbacQuery.RegisterQueryProcessor("ConsumerLiveId", LiveIdInstanceQueryProcessor.ConsumerLiveId);
			RbacQuery.RegisterQueryProcessor("IsDehydrated", new IsDehydratedQueryProcessor());
			RbacQuery.RegisterQueryProcessor("SoftDeletedFeatureEnabled", new SoftDeletedFeatureStatusQueryProcessor());
			RbacQuery.RegisterQueryProcessor("OfficeStoreAvailable", new OfficeStoreAvailableQueryProcessor());
			foreach (EcpFeature ecpFeature in ClientRbac.HybridEcpFeatures)
			{
				new EcpFeatureQueryProcessor(ecpFeature).Register();
			}
			new AliasQueryProcessor("ControlPanelAdmin", "OrgMgmControlPanel+Admin").Register();
			new AliasQueryProcessor("HybridAdmin", "ControlPanelAdmin+XPremiseEnt,ControlPanelAdmin+LiveID").Register();
			RbacQuery.ConditionalQueryProcessors.Regist(delegate(string rbacQuery, out RbacQuery.RbacQueryProcessor processor)
			{
				if (EacFlightUtility.FeaturePrefixs.Any((string prefix) => rbacQuery.StartsWith(prefix)))
				{
					processor = new FlightQueryProcessor(rbacQuery);
					return true;
				}
				processor = null;
				return false;
			});
		}

		public void Dispose()
		{
		}

		private void Application_PostAuthenticateRequest(object sender, EventArgs e)
		{
			HttpContext httpContext = HttpContext.Current;
			string text = httpContext.Request.Headers["msExchProxyUri"];
			if (!string.IsNullOrEmpty(text))
			{
				Uri uri = new Uri(text);
				string text2 = (uri.Segments.Length > 1) ? uri.Segments[1].TrimEnd(new char[]
				{
					'/'
				}) : string.Empty;
				if (text2.Equals(RbacModule.ecpAppPath.Value, StringComparison.OrdinalIgnoreCase) && !text2.Equals(RbacModule.ecpAppPath.Value))
				{
					string url = "/" + RbacModule.ecpAppPath + uri.PathAndQuery.Substring(RbacModule.ecpAppPath.Value.Length + 1);
					httpContext.Response.Redirect(url, true);
					return;
				}
			}
			if (httpContext.Request.HttpMethod == "GET" && !RbacModule.bypassXFrameOptions && !RbacModule.xFrameOptionsExceptionList.Contains(httpContext.Request.AppRelativeCurrentExecutionFilePath))
			{
				httpContext.Response.Headers.Set("X-Frame-Options", "SameOrigin");
			}
			AuthenticationSettings authenticationSettings = new AuthenticationSettings(httpContext);
			httpContext.User = authenticationSettings.Session;
			authenticationSettings.Session.SetCurrentThreadPrincipal();
			if (!httpContext.IsAcsOAuthRequest())
			{
				httpContext.CheckCanary();
			}
			authenticationSettings.Session.RequestReceived();
			if (authenticationSettings.Session is RbacPrincipal)
			{
				if (!OAuthHelper.IsWebRequestAllowed(httpContext))
				{
					ErrorHandlingUtil.TransferToErrorPage("notavailableforpartner");
				}
				if (!LoginUtil.CheckUrlAccess(httpContext.Request.FilePath))
				{
					ErrorHandlingUtil.TransferToErrorPage("noroles");
					return;
				}
				this.FlightRewrite(httpContext);
			}
		}

		private void Application_EndRequest(object sender, EventArgs e)
		{
			IRbacSession rbacSession = HttpContext.Current.User as IRbacSession;
			if (rbacSession != null)
			{
				rbacSession.RequestCompleted();
			}
		}

		private void FlightRewrite(HttpContext context)
		{
			string relativePathToAppRoot = EcpUrl.GetRelativePathToAppRoot(context.Request.FilePath);
			if (relativePathToAppRoot != null)
			{
				string rewriteUrl = EacFlightProvider.Instance.GetRewriteUrl(relativePathToAppRoot);
				if (rewriteUrl != null)
				{
					string str = EcpUrl.ReplaceRelativePath(context.Request.FilePath, rewriteUrl, true);
					context.RewritePath(str + context.Request.PathInfo);
				}
			}
		}

		public static readonly string SessionStateCookieName = "ASP.NET_SessionId";

		private static object initSync = new object();

		private static bool initialized;

		private static HashSet<string> xFrameOptionsExceptionList;

		private static bool bypassXFrameOptions;

		private static Lazy<string> ecpAppPath = new Lazy<string>(() => HttpRuntime.AppDomainAppVirtualPath.TrimStart(new char[]
		{
			'/'
		}));
	}
}
