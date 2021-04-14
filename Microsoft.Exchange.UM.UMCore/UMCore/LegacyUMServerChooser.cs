using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class LegacyUMServerChooser : IRedirectTargetChooser
	{
		internal LegacyUMServerChooser(CallContext callContext, UMRecipient userContext) : this(callContext.DialPlan, callContext.IsSecuredCall, userContext)
		{
		}

		internal LegacyUMServerChooser(UMDialPlan dp, bool securedCall, UMRecipient userContext)
		{
			this.dialplan = dp;
			this.userContext = userContext;
			this.securedCall = securedCall;
			ExAssert.RetailAssert(userContext.VersionCompatibility != VersionEnum.Compatible, "LegacyUMServerChooser should only be used for Legacy users");
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
			fqdn = null;
			port = -1;
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "LegacyServerChooser::GetTargetServer()", new object[0]);
			LegacyUMServerPicker instance = LegacyUMServerPicker.GetInstance(this.userContext.VersionCompatibility);
			InternalExchangeServer internalExchangeServer = instance.PickNextServer(this.dialplan.Id);
			if (internalExchangeServer == null || internalExchangeServer.Server == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "LegacyServerChooser::GetTargetServer() Did not find a valid {0} server to redirect the call. The call will be rejected", new object[]
				{
					this.userContext.VersionCompatibility
				});
				return false;
			}
			fqdn = Utils.TryGetRedirectTargetFqdnForServer(internalExchangeServer.Server);
			UMServer umserver = new UMServer(internalExchangeServer.Server);
			if ((bool)Server.IsE14OrLaterGetter(umserver))
			{
				port = Utils.GetRedirectPort(umserver.SipTcpListeningPort, umserver.SipTlsListeningPort, this.securedCall);
			}
			else
			{
				port = Utils.GetRedirectPort(5060, 5061, this.securedCall);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "LegacyServerChooser::GetTargetServer() returning {0}:{1}", new object[]
			{
				fqdn,
				port
			});
			return true;
		}

		public void HandleServerNotFound()
		{
			ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
			IEnumerable<Server> enabledUMServersInDialPlan = adtopologyLookup.GetEnabledUMServersInDialPlan(this.userContext.VersionCompatibility, this.dialplan.Id);
			using (IEnumerator<Server> enumerator = enabledUMServersInDialPlan.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_LegacyServerNotFoundInDialPlan, null, new object[]
					{
						this.SubscriberLogId,
						this.dialplan.DistinguishedName
					});
				}
				else
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_LegacyServerNotRunningInDialPlan, null, new object[]
					{
						this.SubscriberLogId,
						this.dialplan.DistinguishedName
					});
				}
			}
		}

		private readonly bool securedCall;

		private UMDialPlan dialplan;

		private UMRecipient userContext;
	}
}
