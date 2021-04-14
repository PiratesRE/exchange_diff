using System;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Install", "EmailAddressPolicy")]
	public sealed class InstallEmailAddressPolicy : NewMultitenancyFixedNameSystemConfigurationObjectTask<EmailAddressPolicyContainer>
	{
		[Parameter(Mandatory = false)]
		public SmtpDomain DomainName
		{
			get
			{
				return (SmtpDomain)base.Fields["DomainName"];
			}
			set
			{
				base.Fields["DomainName"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "Recipient Policies");
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			ADObjectId currentOrgContainerId = base.CurrentOrgContainerId;
			EmailAddressPolicyContainer[] array = configurationSession.Find<EmailAddressPolicyContainer>(currentOrgContainerId, QueryScope.SubTree, filter, null, 0);
			EmailAddressPolicyContainer emailAddressPolicyContainer;
			if (this.isContainerExisted = (array != null && array.Length > 0))
			{
				emailAddressPolicyContainer = array[0];
			}
			else
			{
				emailAddressPolicyContainer = (EmailAddressPolicyContainer)base.PrepareDataObject();
				emailAddressPolicyContainer.SetId(currentOrgContainerId.GetChildId("Recipient Policies"));
			}
			return emailAddressPolicyContainer;
		}

		protected override void InternalProcessRecord()
		{
			if (!this.isContainerExisted)
			{
				base.InternalProcessRecord();
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, EmailAddressPolicy.DefaultName);
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			ADObjectId currentOrgContainerId = base.CurrentOrgContainerId;
			EmailAddressPolicy[] array = configurationSession.Find<EmailAddressPolicy>(currentOrgContainerId, QueryScope.SubTree, filter, null, 0);
			if (array == null || array.Length == 0)
			{
				EmailAddressPolicy emailAddressPolicy = new EmailAddressPolicy();
				emailAddressPolicy.SetId(this.DataObject.Id.GetChildId(EmailAddressPolicy.DefaultName));
				emailAddressPolicy[EmailAddressPolicySchema.Enabled] = true;
				emailAddressPolicy.Priority = EmailAddressPolicyPriority.Lowest;
				if (Datacenter.GetExchangeSku() == Datacenter.ExchangeSku.Enterprise)
				{
					emailAddressPolicy.RecipientFilterApplied = true;
				}
				emailAddressPolicy.IncludedRecipients = new WellKnownRecipientType?(WellKnownRecipientType.AllRecipients);
				if (this.DomainName == null)
				{
					emailAddressPolicy.EnabledPrimarySMTPAddressTemplate = "@" + DNConvertor.FqdnFromDomainDistinguishedName(currentOrgContainerId.DomainId.DistinguishedName);
				}
				else
				{
					emailAddressPolicy.EnabledPrimarySMTPAddressTemplate = "@" + this.DomainName.ToString();
				}
				RecipientFilterHelper.StampE2003FilterMetadata(emailAddressPolicy, emailAddressPolicy.LdapRecipientFilter, EmailAddressPolicySchema.PurportedSearchUI);
				if (base.CurrentOrganizationId != null)
				{
					emailAddressPolicy.OrganizationId = base.CurrentOrganizationId;
				}
				else
				{
					emailAddressPolicy.OrganizationId = base.ExecutingUserOrganizationId;
				}
				configurationSession.Save(emailAddressPolicy);
			}
		}

		private bool isContainerExisted;
	}
}
