using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DataCenterServerChooser : IRedirectTargetChooser
	{
		public DataCenterServerChooser(UMDialPlan dialPlan, bool isSecuredCall, UMRecipient userContext)
		{
			this.dialPlan = dialPlan;
			this.isSecuredCall = isSecuredCall;
			this.userContext = userContext;
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
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "DataCenterServerChooser::GetTargetServer() Site = {0}", new object[]
			{
				this.userContext.MailboxServerSite.DistinguishedName
			});
			Server server = null;
			UMRecipient umrecipient = this.userContext;
			if (Utils.RunningInTestMode && umrecipient != null && !string.IsNullOrEmpty(umrecipient.ADRecipient.CustomAttribute2))
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 111, "GetTargetServer", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcore\\DataCenterServerChooser.cs");
				server = topologyConfigurationSession.FindServerByFqdn(umrecipient.ADRecipient.CustomAttribute2);
			}
			else
			{
				InternalExchangeServer internalExchangeServer = DatacenterSiteBasedServerPicker.Instance.PickNextServer(this.userContext.MailboxServerSite);
				if (internalExchangeServer != null)
				{
					server = internalExchangeServer.Server;
				}
			}
			if (server == null)
			{
				return false;
			}
			fqdn = SipRoutingHelper.GetCrossSiteRedirectTargetFqdnAndPort(server, this.isSecuredCall, out port);
			return true;
		}

		public void HandleServerNotFound()
		{
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UMServerNotFoundInSite, null, new object[]
			{
				this.SubscriberLogId,
				(this.dialPlan != null && this.dialPlan.Id != null) ? this.dialPlan.Id.ToString() : string.Empty,
				this.userContext.MailboxServerSite
			});
		}

		private readonly bool isSecuredCall;

		private UMDialPlan dialPlan;

		private UMRecipient userContext;
	}
}
