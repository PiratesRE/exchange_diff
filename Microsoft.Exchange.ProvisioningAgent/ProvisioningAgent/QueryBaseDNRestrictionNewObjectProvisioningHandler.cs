using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal class QueryBaseDNRestrictionNewObjectProvisioningHandler : ProvisioningHandlerBase
	{
		public override ProvisioningValidationError[] Validate(IConfigurable readOnlyIConfigurable)
		{
			ADObject adobject;
			if (readOnlyIConfigurable is ADPresentationObject)
			{
				adobject = ((ADPresentationObject)readOnlyIConfigurable).DataObject;
			}
			else
			{
				adobject = (ADObject)readOnlyIConfigurable;
			}
			ADUser aduser = adobject as ADUser;
			if (aduser == null || !aduser.QueryBaseDNRestrictionEnabled || ADObjectId.Equals(aduser.QueryBaseDN, aduser.Id))
			{
				return null;
			}
			this.savedAdUser = aduser;
			return null;
		}

		public override void OnComplete(bool succeeded, Exception e)
		{
			if (!succeeded)
			{
				return;
			}
			if (this.savedAdUser == null)
			{
				return;
			}
			string domainController = base.UserSpecifiedParameters["DomainController"] as string;
			IRecipientSession recipientSession = this.GetRecipientSession(domainController, this.savedAdUser.OrganizationId);
			ADUser aduser = (ADUser)recipientSession.Read<ADUser>(this.savedAdUser.Id);
			if (aduser != null)
			{
				aduser.QueryBaseDN = aduser.Id;
				recipientSession.Save(aduser);
			}
		}

		private IRecipientSession GetRecipientSession(string domainController, OrganizationId organizationId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, null, false);
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, false, ConsistencyMode.FullyConsistent, sessionSettings, 119, "GetRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\QueryBaseDNRestrictionNewObjectProvisioningHandler.cs");
		}

		private ADUser savedAdUser;
	}
}
