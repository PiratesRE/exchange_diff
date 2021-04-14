using System;
using System.Security.Principal;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.FailFast.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;

namespace Microsoft.Exchange.Configuration.FailFast
{
	public class ThrottlingHttpModule : IHttpModule
	{
		void IHttpModule.Init(HttpApplication context)
		{
			context.PostAuthenticateRequest += this.OnPostAuthenticateRequest;
		}

		void IHttpModule.Dispose()
		{
		}

		private void OnPostAuthenticateRequest(object sender, EventArgs e)
		{
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[ThrottlingHttpModule::OnPostAuthenticateRequest] Enter");
			HttpContext httpContext = HttpContext.Current;
			if (!httpContext.Request.IsAuthenticated || httpContext.User == null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ThrottlingHttpModule::OnPostAuthenticateRequest] Request is not authenticated.");
				return;
			}
			using (new MonitoredScope("ThrottlingHttpModule", "ThrottlingHttpModule", HttpModuleHelper.HttpPerfMonitors))
			{
				this.ThrottleRequest(httpContext);
			}
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[ThrottlingHttpModule::OnPostAuthenticateRequest] Exit");
		}

		private void ThrottleRequest(HttpContext context)
		{
			ExTraceGlobals.HttpModuleTracer.TraceFunction((long)this.GetHashCode(), "[ThrottlingHttpModule::ThrottleRequest] Enter");
			WinRMInfo winRMInfo = context.Items["X-RemotePS-WinRMInfo"] as WinRMInfo;
			if (winRMInfo == null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ThrottlingHttpModule::ThrottleRequest] WinRMInfo = null.");
				return;
			}
			string action = winRMInfo.Action;
			if (string.IsNullOrEmpty(action))
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ThrottlingHttpModule::ThrottleRequest] WinRMInfo.Action = null.");
				return;
			}
			if (!action.EndsWith(":Command", StringComparison.OrdinalIgnoreCase) && !action.Equals("Command", StringComparison.OrdinalIgnoreCase))
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug<string>((long)this.GetHashCode(), "[ThrottlingHttpModule::ThrottleRequest] Not Command Request. WinRMInfo.Action = {0}.", action);
				return;
			}
			IPowerShellBudget powerShellBudget = null;
			OverBudgetException ex = null;
			try
			{
				try
				{
					using (new MonitoredScope("ThrottlingHttpModule", "GetBudget", HttpModuleHelper.HttpPerfMonitors))
					{
						powerShellBudget = this.GetBudget(context);
					}
					if (powerShellBudget == null)
					{
						ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ThrottlingHttpModule::ThrottleRequest] budget = null.");
						return;
					}
					using (new MonitoredScope("ThrottlingHttpModule", "CheckBudgetAndStartCmdlet", HttpModuleHelper.HttpPerfMonitors))
					{
						powerShellBudget.StartCmdlet(null);
						powerShellBudget.TryCheckOverBudget(CostType.CMDLET, out ex);
					}
				}
				finally
				{
					if (powerShellBudget != null)
					{
						powerShellBudget.Dispose();
					}
				}
				if (ex != null)
				{
					string windowsLiveId = context.CurrentUserToken().WindowsLiveId;
					if (windowsLiveId != null)
					{
						FailFastUserCache.Instance.AddUserToCache(windowsLiveId, BlockedType.NewRequest, TimeSpan.Zero);
						HttpLogger.SafeAppendColumn(RpsCommonMetadata.ContributeToFailFast, "AuthZ-UserSelf", LoggerHelper.GetContributeToFailFastValue("User", windowsLiveId, BlockedType.NewRequest.ToString(), -1.0));
					}
					HttpModuleHelper.EndPowerShellRequestWithFriendlyError(context, FailureCategory.AuthZ, ex.GetType().Name, Strings.ErrorOperationTarpitting(ex.BackoffTime / 1000) + string.Format("{2}Policy: {0}; {2}Snapshot: {1}", ex.ThrottlingPolicyDN, ex.Snapshot, Environment.NewLine), "ThrottlingHttpModule", false);
				}
			}
			catch (Exception ex2)
			{
				if (!(ex2 is ThreadAbortException))
				{
					HttpLogger.SafeAppendGenericError("ThrottlingHttpModule", ex2, new Func<Exception, bool>(KnownException.IsUnhandledException));
					ExTraceGlobals.HttpModuleTracer.TraceError<Exception>((long)this.GetHashCode(), "[ThrottlingHttpModule::ThrottleRequest] Get un-Excpected Exception. {0}", ex2);
				}
			}
		}

		private IPowerShellBudget GetBudget(HttpContext context)
		{
			if (context.User is DelegatedPrincipal)
			{
				using (new MonitoredScope("ThrottlingHttpModule", "AcquireDelegatedPrincipalBudget", HttpModuleHelper.HttpPerfMonitors))
				{
					return PowerShellBudget.Acquire(new DelegatedPrincipalBudgetKey((DelegatedPrincipal)context.User, BudgetType.WSMan));
				}
			}
			if (context.CurrentUserToken() == null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ThrottlingHttpModule::GetBudget] No CAT.");
				return null;
			}
			SecurityIdentifier userSid = context.CurrentUserToken().UserSid;
			if (userSid == null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ThrottlingHttpModule::GetBudget] sid = null.");
				return null;
			}
			OrganizationId organization = context.CurrentUserToken().Organization;
			if (organization == null)
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ThrottlingHttpModule::GetBudget] orgId = null.");
				return null;
			}
			ADObjectId adobjectId;
			if (organization.Equals(OrganizationId.ForestWideOrgId))
			{
				ExTraceGlobals.HttpModuleTracer.TraceDebug((long)this.GetHashCode(), "[ThrottlingHttpModule::GetBudget] Forest Wide Org Id.");
				adobjectId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			}
			else
			{
				adobjectId = ADSystemConfigurationSession.GetRootOrgContainerId(organization.PartitionId.ForestFQDN, null, null);
			}
			ExTraceGlobals.HttpModuleTracer.TraceDebug<string>((long)this.GetHashCode(), "[ThrottlingHttpModule::GetBudget] rootOrgId = {0}.", (adobjectId == null) ? null : adobjectId.DistinguishedName);
			IPowerShellBudget result;
			using (new MonitoredScope("ThrottlingHttpModule", "AcquireBudget", HttpModuleHelper.HttpPerfMonitors))
			{
				result = PowerShellBudget.Acquire(userSid, BudgetType.WSMan, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(adobjectId, organization, organization, true));
			}
			return result;
		}

		private const string GroupNameForMonitor = "ThrottlingHttpModule";
	}
}
