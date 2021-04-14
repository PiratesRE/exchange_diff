using System;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class OwaRequestEventInspector : RequestEventInspectorBase
	{
		internal override void Init()
		{
			this.RegisterFilterForOnBegin();
			this.RegisterFilterForOnPostAuthorize();
		}

		internal override void OnBeginRequest(object sender, EventArgs e, out bool stopExecution)
		{
			stopExecution = false;
			if (this.onBeginRequestChain != null)
			{
				stopExecution = this.onBeginRequestChain.ExecuteRequestFilterChain(sender, e, RequestEventType.BeginRequest);
			}
		}

		internal override void OnPostAuthorizeRequest(object sender, EventArgs e)
		{
			bool flag = false;
			if (this.onPostAuthorizeRequestChain != null)
			{
				flag = this.onPostAuthorizeRequestChain.ExecuteRequestFilterChain(sender, e, RequestEventType.PostAuthorizeRequest);
			}
			if (flag)
			{
				return;
			}
			HttpApplication httpApplication = (HttpApplication)sender;
			if (UrlUtilities.IsWacRequest(httpApplication.Context.Request))
			{
				return;
			}
			try
			{
				RequestDispatcher.DispatchRequest(OwaContext.Get(httpApplication.Context));
			}
			catch (ThreadAbortException)
			{
				OwaContext.Current.UnlockMinResourcesOnCriticalError();
			}
		}

		private void RegisterFilterForOnPostAuthorize()
		{
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.ExplicitLogonAuthFilter.Enabled)
			{
				this.RegisterFilterForPostAuthorizeRequest(new AlternateMailboxFilterChain());
				return;
			}
			this.RegisterFilterForPostAuthorizeRequest(new FBASingleSignOnFilterChain());
		}

		private void RegisterFilterForPostAuthorizeRequest(RequestFilterChain filter)
		{
			if (this.onPostAuthorizeRequestChain == null)
			{
				this.onPostAuthorizeRequestChain = filter;
				return;
			}
			filter.Next = this.onPostAuthorizeRequestChain;
			this.onPostAuthorizeRequestChain = filter;
		}

		private void RegisterFilterForOnBegin()
		{
			this.RegisterFilterForOnBeginRequest(new FBASingleSignOnFilterChain());
		}

		private void RegisterFilterForOnBeginRequest(RequestFilterChain filter)
		{
			if (this.onBeginRequestChain == null)
			{
				this.onBeginRequestChain = filter;
				return;
			}
			filter.Next = this.onBeginRequestChain;
			this.onBeginRequestChain = filter;
		}

		internal override void OnEndRequest(OwaContext owaContext)
		{
			try
			{
			}
			finally
			{
				UserContext userContext = owaContext.UserContext;
				bool flag = false;
				try
				{
					if (owaContext.UserContext != null && !owaContext.UserContext.LockedByCurrentThread())
					{
						if (!owaContext.IsAsyncRequest && !owaContext.HandledCriticalError && !owaContext.UserContext.LastLockRequestFailed)
						{
							ExWatson.SendReport(new InvalidOperationException("Entered OwaRequestEventInspector without the UserContext lock when we should have had it."), ReportOptions.None, null);
						}
						owaContext.UserContext.Lock();
					}
					try
					{
						try
						{
							if (userContext != null && userContext.State == UserContextState.Active)
							{
								userContext.CleanupOnEndRequest();
								owaContext.ExitLatencyDetectionContext();
								OwaPerformanceLogger.LogPerformanceStatistics(userContext);
								OwaPerformanceLogger.TracePerformance(userContext);
								this.AppendServerHeaders(owaContext);
								if (owaContext.SearchPerformanceData != null)
								{
									owaContext.SearchPerformanceData.RefreshEnd();
									owaContext.SearchPerformanceData.WriteLog();
								}
							}
						}
						finally
						{
							owaContext.DisposeObjectsOnEndRequest();
						}
					}
					catch (OwaLockTimeoutException)
					{
						flag = true;
					}
					finally
					{
						if (userContext != null && !flag)
						{
							if (owaContext.IgnoreUnlockForcefully)
							{
								userContext.Unlock();
							}
							else
							{
								userContext.UnlockForcefully();
							}
						}
					}
				}
				finally
				{
					owaContext.TryReleaseBudgetAndStopTiming();
					if (owaContext.PreFormActionData != null && owaContext.PreFormActionData is IDisposable)
					{
						((IDisposable)owaContext.PreFormActionData).Dispose();
						owaContext.PreFormActionData = null;
					}
				}
			}
		}

		private void AppendServerHeaders(OwaContext owaContext)
		{
			HttpContext httpContext = owaContext.HttpContext;
			if (httpContext == null || httpContext.Response == null || !UserAgentUtilities.IsMonitoringRequest(httpContext.Request.UserAgent))
			{
				return;
			}
			try
			{
				if (owaContext.UserContext.ExchangePrincipal != null)
				{
					string shortServerNameFromFqdn = Utilities.GetShortServerNameFromFqdn(owaContext.UserContext.ExchangePrincipal.MailboxInfo.Location.ServerFqdn);
					if (shortServerNameFromFqdn != null)
					{
						httpContext.Response.AppendHeader("X-DiagInfoMailbox", shortServerNameFromFqdn);
					}
				}
				string lastRecipientSessionDCServerName = owaContext.UserContext.LastRecipientSessionDCServerName;
				if (lastRecipientSessionDCServerName != null)
				{
					httpContext.Response.AppendHeader("X-DiagInfoDomainController", Utilities.GetShortServerNameFromFqdn(lastRecipientSessionDCServerName));
				}
			}
			catch (HttpException arg)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<HttpException>(0L, "Exception happened while trying to append server name headers. Exception will be ignored: {0}", arg);
			}
		}

		private RequestFilterChain onPostAuthorizeRequestChain;

		private RequestFilterChain onBeginRequestChain;
	}
}
