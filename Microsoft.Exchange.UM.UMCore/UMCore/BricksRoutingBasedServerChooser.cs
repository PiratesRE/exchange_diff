using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class BricksRoutingBasedServerChooser : IRedirectTargetChooser
	{
		public bool IsRedirectionNeeded { get; private set; }

		public BricksRoutingBasedServerChooser(IRoutingContext currentCallContext, UMRecipient userContext, CallType callType)
		{
			ExAssert.RetailAssert(callType == 3 || callType == 4, "Incorrect CallType");
			this.userContext = userContext;
			if (userContext.ADRecipient.RecipientType == RecipientType.UserMailbox)
			{
				this.InitializeRedirectionResults(currentCallContext, userContext, callType);
				Server server = LocalServer.GetServer();
				this.IsRedirectionNeeded = !string.Equals(Utils.TryGetRedirectTargetFqdnForServer(server), this.fqdn, StringComparison.OrdinalIgnoreCase);
			}
		}

		public string SubscriberLogId
		{
			get
			{
				return this.userContext.MailAddress;
			}
		}

		public bool GetTargetServer(out string fqdn, out int port)
		{
			fqdn = this.fqdn;
			port = this.port;
			return true;
		}

		public void HandleServerNotFound()
		{
		}

		private void InitializeRedirectionResults(IRoutingContext currentCallContext, UMRecipient userContext, CallType callType)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "BricksRoutingBasedServerChooser::InitializeRedirectionResults", new object[0]);
			RedirectionTarget.ResultSet resultSet;
			if (callType == 4)
			{
				resultSet = RedirectionTarget.Instance.GetForCallAnsweringCall(userContext, currentCallContext);
			}
			else
			{
				resultSet = RedirectionTarget.Instance.GetForSubscriberAccessCall(userContext, currentCallContext);
			}
			this.fqdn = resultSet.Fqdn;
			this.port = resultSet.Port;
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "BricksRoutingBasedServerChooser::InitializeRedirectionResults() returning {0}:{1}", new object[]
			{
				this.fqdn,
				this.port
			});
		}

		private UMRecipient userContext;

		private string fqdn;

		private int port = -1;
	}
}
