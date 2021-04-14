using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Diagnostics.CmdletInfra;

namespace Microsoft.Exchange.Configuration.Core
{
	internal class WinRMDataSender : WinRMDataExchanger
	{
		static WinRMDataSender()
		{
			if (WinRMDataSender.primaryObjectBehavior == null)
			{
				lock (WinRMDataSender.initializeLocker)
				{
					if (WinRMDataSender.primaryObjectBehavior == null)
					{
						WinRMDataSender.primaryObjectBehavior = new CrossAppDomainPrimaryObjectBehavior(WinRMDataExchanger.PipeName, BehaviorDirection.Out, null);
						AppDomain.CurrentDomain.DomainUnload += WinRMDataSender.CurrentDomainDomainUnload;
					}
				}
			}
		}

		public WinRMDataSender(HttpContext context, LatencyTracker latencyTracker)
		{
			WinRMDataSender <>4__this = this;
			CoreLogger.ExecuteAndLog("WinRMDataSender.Ctor", true, null, null, delegate()
			{
				if (!WinRMDataExchangeHelper.IsExchangeDataUseAuthenticationType() && !WinRMDataExchangeHelper.IsExchangeDataUseNamedPipe())
				{
					throw new InvalidFlightingException();
				}
				if (context.User == null || context.User.Identity == null)
				{
					throw new ArgumentException("context.User and context.User.Identity should not be null.");
				}
				<>4__this.httpContext = context;
				<>4__this.latencyTracker = latencyTracker;
				<>4__this.Identity = WinRMDataExchangeHelper.GetWinRMDataIdentity(<>4__this.httpContext.Request.Url.ToString(), <>4__this.httpContext.User.Identity.Name, <>4__this.httpContext.User.Identity.AuthenticationType);
				CoreLogger.TraceDebug("[WinRMDataSender.Ctor]Initialized for identity: {0}", new object[]
				{
					<>4__this.Identity
				});
			});
		}

		public void Send()
		{
			CoreLogger.ExecuteAndLog("WinRMDataSender.Send", true, this.latencyTracker, delegate(Exception ex)
			{
				HttpLogger.SafeAppendGenericError("WinRMDataSender.Send", ex.ToString(), false);
			}, delegate()
			{
				string text = WinRMDataExchangeHelper.Serialize(base.Items);
				if (WinRMDataExchangeHelper.IsExchangeDataUseAuthenticationType())
				{
					GenericIdentity identity = new GenericIdentity(this.httpContext.User.Identity.Name, WinRMDataExchangeHelper.HydrateAuthenticationType(this.httpContext.User.Identity.AuthenticationType, text));
					this.httpContext.User = new GenericPrincipal(identity, new string[0]);
					CoreLogger.TraceDebug("[WinRMDataSender.Send]Send data for identity {0} by authentication type: {1}", new object[]
					{
						base.Identity,
						text
					});
					HttpLogger.SafeAppendGenericInfo("WinRMDataSender.AuthenticationType", "Sent");
				}
				if (WinRMDataExchangeHelper.IsExchangeDataUseNamedPipe())
				{
					WinRMDataSender.primaryObjectBehavior.SendMessage(text);
					CoreLogger.TraceDebug("[WinRMDataSender.Send]Send data for identity {0} by named pipe: {1}", new object[]
					{
						base.Identity,
						text
					});
					HttpLogger.SafeAppendGenericInfo("WinRMDataSender.NamedPipe", "Sent");
				}
			});
		}

		private static void CurrentDomainDomainUnload(object sender, EventArgs e)
		{
			WinRMDataSender.primaryObjectBehavior.Dispose();
		}

		private static readonly object initializeLocker = new object();

		private static readonly CrossAppDomainPrimaryObjectBehavior primaryObjectBehavior;

		private HttpContext httpContext;

		private LatencyTracker latencyTracker;
	}
}
