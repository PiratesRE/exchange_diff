using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UMIPGatewaySipPeer : UMSipPeer
	{
		public UMIPGatewaySipPeer(UMIPGateway gateway, bool useMutualTLS) : base(gateway.Address, gateway.Port, gateway.OutcallsAllowed && !gateway.Simulator, useMutualTLS, gateway.IPAddressFamily)
		{
			ValidateArgument.NotNull(gateway, "gateway");
			this.gateway = gateway;
		}

		public UMIPGatewaySipPeer(UMIPGateway gateway, UMDialPlan dialPlan) : this(gateway, dialPlan.VoIPSecurity != UMVoIPSecurityType.Unsecured)
		{
		}

		public override bool IsOcs
		{
			get
			{
				bool result = false;
				MultiValuedProperty<UMHuntGroup> huntGroups = this.gateway.HuntGroups;
				if (huntGroups != null && huntGroups.Count > 0)
				{
					IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(this.gateway.OrganizationId);
					UMDialPlan dialPlanFromId = iadsystemConfigurationLookup.GetDialPlanFromId(huntGroups[0].UMDialPlan);
					result = (dialPlanFromId.URIType == UMUriType.SipName);
				}
				return result;
			}
		}

		public override string Name
		{
			get
			{
				return this.gateway.Name;
			}
		}

		public override UMIPGateway ToUMIPGateway(OrganizationId orgId)
		{
			ValidateArgument.NotNull(orgId, "orgId");
			ExAssert.RetailAssert(orgId.Equals(this.gateway.OrganizationId), "orgId='{0}' does not match this.gateway.OrganizationId='{1}'", new object[]
			{
				orgId,
				this.gateway.OrganizationId
			});
			return this.gateway;
		}

		private UMIPGateway gateway;
	}
}
