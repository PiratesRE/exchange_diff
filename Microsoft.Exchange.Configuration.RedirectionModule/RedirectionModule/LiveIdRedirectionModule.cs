using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Configuration.RedirectionModule.EventLog;
using Microsoft.Exchange.Configuration.TenantMonitoring;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RedirectionModule;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Configuration.RedirectionModule
{
	public class LiveIdRedirectionModule : IHttpModule
	{
		static LiveIdRedirectionModule()
		{
			RedirectionHelper.InitTenantsOnCurrentSiteCache();
			LiveIdRedirectionModule.perfCounter = RemotePowershellPerformanceCounters.GetInstance("RemotePS-LiveID");
			LiveIdRedirectionModule.perfCounter.PID.RawValue = (long)Process.GetCurrentProcess().Id;
			Globals.InitializeMultiPerfCounterInstance("RemotePS-LiveID");
		}

		void IHttpModule.Init(HttpApplication application)
		{
			application.PostAuthenticateRequest += LiveIdRedirectionModule.OnPostAuthenticateRequestHandler;
		}

		void IHttpModule.Dispose()
		{
		}

		internal static bool TryResolveCurrentUserInLocalForest(IPrincipal user, TraceSource traceSrc, out string tenantName)
		{
			tenantName = null;
			ADRawEntry adrawEntry = null;
			if (user != null)
			{
				SecurityIdentifier securityIdentifier = null;
				PartitionId partitionId = null;
				GenericSidIdentity genericSidIdentity = user.Identity as GenericSidIdentity;
				if (genericSidIdentity != null)
				{
					securityIdentifier = genericSidIdentity.Sid;
					if (!string.IsNullOrEmpty(genericSidIdentity.PartitionId))
					{
						PartitionId.TryParse(genericSidIdentity.PartitionId, out partitionId);
					}
				}
				else
				{
					WindowsIdentity windowsIdentity = user.Identity as WindowsIdentity;
					if (windowsIdentity != null)
					{
						securityIdentifier = windowsIdentity.User;
					}
				}
				if (securityIdentifier != null)
				{
					Logger.LogVerbose(traceSrc, "User sid is {0}.", new object[]
					{
						securityIdentifier.ToString()
					});
					try
					{
						adrawEntry = UserTokenStaticHelper.GetADRawEntry(partitionId, null, securityIdentifier);
						goto IL_11E;
					}
					catch (TransientException exception)
					{
						Logger.LogError(LiveIdRedirectionModule.eventLogger, traceSrc, "Failed to map user sid to an AD-Account with the following transient error {0}.", exception, new ExEventLog.EventTuple?(TaskEventLogConstants.Tuple_LiveIdRedirection_FailedWindowsIdMapping), securityIdentifier.ToString());
						goto IL_11E;
					}
					catch (DataSourceOperationException exception2)
					{
						Logger.LogError(LiveIdRedirectionModule.eventLogger, traceSrc, "Failed to map user sid to an AD-Account with the following error {0}.", exception2, new ExEventLog.EventTuple?(TaskEventLogConstants.Tuple_LiveIdRedirection_FailedWindowsIdMapping), securityIdentifier.ToString());
						goto IL_11E;
					}
					catch (DataValidationException exception3)
					{
						Logger.LogError(LiveIdRedirectionModule.eventLogger, traceSrc, "Failed to map user sid to an AD-Account with the following error {0}.", exception3, new ExEventLog.EventTuple?(TaskEventLogConstants.Tuple_LiveIdRedirection_FailedWindowsIdMapping), securityIdentifier.ToString());
						goto IL_11E;
					}
				}
				Logger.LogWarning(traceSrc, "Identity in the context is not valid. It should be either GenericSidIdentity or WindowsIdentity.");
			}
			else
			{
				Logger.LogWarning(traceSrc, "Context.User is null.");
			}
			IL_11E:
			if (adrawEntry != null)
			{
				Logger.LogVerbose(traceSrc, "User SMTP address resolved from AD {0}.", new object[]
				{
					(SmtpAddress)adrawEntry[ADRecipientSchema.WindowsLiveID]
				});
				ADObjectId adobjectId = (ADObjectId)adrawEntry[ADObjectSchema.OrganizationalUnitRoot];
				if (adobjectId != null)
				{
					tenantName = adobjectId.Name;
					Logger.LogVerbose(traceSrc, "User Tenant Name resolved from AD {0}.", new object[]
					{
						tenantName
					});
				}
			}
			return adrawEntry != null;
		}

		private static void OnPostAuthenticateRequest(object source, EventArgs args)
		{
			HttpApplication httpApplication = (HttpApplication)source;
			HttpContext context = httpApplication.Context;
			Logger.EnterFunction(ExTraceGlobals.RedirectionTracer, "LiveIdRedirectionModule.OnPostAuthenticateRequest");
			if (!context.Request.IsAuthenticated)
			{
				Logger.LogWarning(LiveIdRedirectionModule.traceSrc, "OnPostAuthenticateRequest was called on a not Authenticated Request!");
				Logger.ExitFunction(ExTraceGlobals.RedirectionTracer, "LiveIdRedirectionModule.OnPostAuthenticateRequest");
				return;
			}
			if (!RedirectionHelper.ShouldProcessLiveIdRedirection(context))
			{
				Logger.LogVerbose(LiveIdRedirectionModule.traceSrc, "Redirection Logic skipped for user '{0}'.", new object[]
				{
					context.User.ToString()
				});
				Logger.ExitFunction(ExTraceGlobals.RedirectionTracer, "LiveIdRedirectionModule.OnPostAuthenticateRequest");
				return;
			}
			bool flag = true;
			string text = null;
			string text2 = (string)HttpContext.Current.Items["WLID-MemberName"];
			if (RedirectionHelper.IsUserTenantOnCurrentSiteCache(text2))
			{
				Logger.LogVerbose(LiveIdRedirectionModule.traceSrc, "Redirection Logic skipped, user '{0}' is present on the current site cache.", new object[]
				{
					text2
				});
				Logger.ExitFunction(ExTraceGlobals.RedirectionTracer, "LiveIdRedirectionModule.OnPostAuthenticateRequest");
				return;
			}
			if (context.Request.LogonUserIdentity.IsSystem && !(context.User.Identity is GenericSidIdentity) && !string.IsNullOrEmpty(text2))
			{
				flag = false;
				if (!SmtpAddress.IsValidSmtpAddress(text2))
				{
					Logger.LogWarning(LiveIdRedirectionModule.traceSrc, "Cannot convert memberName to SMTP address. No redirection available.");
					Logger.ExitFunction(ExTraceGlobals.RedirectionTracer, "LiveIdRedirectionModule.OnPostAuthenticateRequest");
					return;
				}
				text = SmtpAddress.Parse(text2).Domain;
			}
			else
			{
				if (!LiveIdRedirectionModule.TryResolveCurrentUserInLocalForest(context.User, LiveIdRedirectionModule.traceSrc, out text))
				{
					Logger.LogWarning(LiveIdRedirectionModule.traceSrc, "Cannot resolve the current request user account in local forest.");
					Logger.ExitFunction(ExTraceGlobals.RedirectionTracer, "LiveIdRedirectionModule.OnPostAuthenticateRequest");
					return;
				}
				if (string.IsNullOrEmpty(text))
				{
					Logger.LogVerbose(LiveIdRedirectionModule.traceSrc, "Tenant Name cannot be resolved from the Url.");
					Logger.ExitFunction(ExTraceGlobals.RedirectionTracer, "LiveIdRedirectionModule.OnPostAuthenticateRequest");
					return;
				}
			}
			TenantMonitor.LogActivity(CounterType.HomeSiteLocationAttempts, text);
			Uri originalUrl = RedirectionHelper.RemovePropertiesFromOriginalUri(context.Request.Url, RedirectionConfig.RedirectionUriFilterProperties);
			if (!flag)
			{
				Logger.LogVerbose(LiveIdRedirectionModule.traceSrc, "Current request user is external to local AD.");
				Uri redirectUrlForTenantForest = RedirectionHelper.GetRedirectUrlForTenantForest(text, RedirectionConfig.PodRedirectTemplate, originalUrl, RedirectionConfig.PodSiteStartRange, RedirectionConfig.PodSiteEndRange);
				if (null != redirectUrlForTenantForest)
				{
					Logger.LogVerbose(LiveIdRedirectionModule.traceSrc, "Redirecting user to {0}.", new object[]
					{
						redirectUrlForTenantForest
					});
					context.Response.Redirect(redirectUrlForTenantForest.ToString());
					TenantMonitor.LogActivity(CounterType.HomeSiteLocationSuccesses, text);
				}
				else
				{
					Logger.LogEvent(LiveIdRedirectionModule.eventLogger, TaskEventLogConstants.Tuple_LiveIdRedirection_FailedToResolveForestRedirection, text2, new object[]
					{
						text,
						text2
					});
				}
			}
			else
			{
				Logger.LogVerbose(LiveIdRedirectionModule.traceSrc, "Current request user is present in AD.");
				Uri uri = null;
				Exception ex = null;
				try
				{
					uri = RedirectionHelper.GetRedirectUrlForTenantSite(text, RedirectionConfig.SiteRedirectTemplate, originalUrl, LiveIdRedirectionModule.eventLogger);
					TenantMonitor.LogActivity(CounterType.HomeSiteLocationSuccesses, text);
				}
				catch (RedirectionLogicException ex2)
				{
					ex = ex2;
				}
				catch (DataSourceOperationException ex3)
				{
					ex = ex3;
				}
				catch (TransientException ex4)
				{
					ex = ex4;
				}
				catch (DataValidationException ex5)
				{
					ex = ex5;
				}
				if (ex != null)
				{
					Logger.GenerateErrorMessage(context.Response, LiveIdRedirectionModule.eventLogger, TaskEventLogConstants.Tuple_LiveIdRedirection_ServerError, ex, text);
				}
				else if (null != uri)
				{
					Logger.LogVerbose(LiveIdRedirectionModule.traceSrc, "Redirecting user to {0}.", new object[]
					{
						uri
					});
					context.Response.Redirect(uri.ToString());
				}
				else
				{
					RedirectionHelper.AddTenantToCurrentSiteCache(text);
				}
			}
			Logger.ExitFunction(ExTraceGlobals.RedirectionTracer, "LiveIdRedirectionModule.OnPostAuthenticateRequest");
		}

		private const string W3wpPerfCounterInstanceName = "RemotePS-LiveID";

		private static readonly EventHandler OnPostAuthenticateRequestHandler = new EventHandler(LiveIdRedirectionModule.OnPostAuthenticateRequest);

		private static readonly ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.RedirectionTracer.Category, "MSExchange LiveId Redirection Module");

		private static readonly TraceSource traceSrc = new TraceSource("LiveIdRedirectionModule");

		private static RemotePowershellPerformanceCountersInstance perfCounter;
	}
}
