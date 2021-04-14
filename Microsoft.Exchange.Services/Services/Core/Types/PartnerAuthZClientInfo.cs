using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Security.PartnerToken;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class PartnerAuthZClientInfo : AuthZClientInfo
	{
		private PartnerAuthZClientInfo(OrganizationId delegatedOrganizationId, string delegatedOrganizationName, string partnerUser)
		{
			this.delegatedOrganizationId = delegatedOrganizationId;
			this.delegatedOrganizationName = delegatedOrganizationName;
			this.partnerUser = partnerUser;
		}

		public static AuthZClientInfo FromPartnerIdentity(PartnerIdentity partnerIdentity)
		{
			return new PartnerAuthZClientInfo(partnerIdentity.DelegatedOrganizationId, partnerIdentity.DelegatedPrincipal.DelegatedOrganization, partnerIdentity.DelegatedPrincipal.UserId);
		}

		public static AuthZClientInfo FromPartnerAccessToken(PartnerAccessToken token)
		{
			return new PartnerAuthZClientInfo(token.OrganizationId, token.OrganizationName, token.PartnerUser);
		}

		internal override ADRecipientSessionContext GetADRecipientSessionContext()
		{
			return ADRecipientSessionContext.CreateForPartner(this.delegatedOrganizationId);
		}

		public override void ApplyManagementRole(ManagementRoleType managementRoleType, WebMethodEntry methodEntry)
		{
			if (managementRoleType == null)
			{
				return;
			}
			throw new InvalidManagementRoleHeaderException(CoreResources.IDs.MessageManagementRoleHeaderNotSupportedForPartnerIdentity);
		}

		public override AuthZBehavior GetAuthZBehavior()
		{
			return AuthZBehavior.DefaultBehavior;
		}

		public override string ToCallerString()
		{
			return string.Format("{0}\\{1}", this.delegatedOrganizationName, this.partnerUser);
		}

		private readonly OrganizationId delegatedOrganizationId;

		private readonly string delegatedOrganizationName;

		private readonly string partnerUser;
	}
}
