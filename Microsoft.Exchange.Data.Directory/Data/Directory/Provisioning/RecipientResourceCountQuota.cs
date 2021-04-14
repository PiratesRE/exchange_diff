using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Provisioning
{
	[Serializable]
	internal sealed class RecipientResourceCountQuota : ProvisioningResourceCountQuota
	{
		public RecipientResourceCountQuota(ADPropertyDefinition countQuotaProperty, string systemAddressListName, Type[] targetObjectTypes) : base(countQuotaProperty, systemAddressListName, targetObjectTypes)
		{
		}

		public RecipientResourceCountQuota(ADPropertyDefinition countQuotaProperty, string systemAddressListName, Type[] targetObjectTypes, RecipientTypeDetails[] targetObjectRecipientTypeDetails) : this(countQuotaProperty, systemAddressListName, targetObjectTypes)
		{
			if (targetObjectRecipientTypeDetails != null)
			{
				this.targetRecipientTypeDetails = new ReadOnlyCollection<RecipientTypeDetails>(targetObjectRecipientTypeDetails);
			}
		}

		public override bool IsApplicable(IConfigurable readOnlyPresentationObject)
		{
			if (base.Context != null && base.Context.UserSpecifiedParameters != null && base.Context.UserSpecifiedParameters.Contains("OverrideRecipientQuotas") && base.Context.UserSpecifiedParameters["OverrideRecipientQuotas"] != null && base.Context.UserSpecifiedParameters["OverrideRecipientQuotas"] is SwitchParameter && (SwitchParameter)base.Context.UserSpecifiedParameters["OverrideRecipientQuotas"])
			{
				return false;
			}
			ADObject adobject = readOnlyPresentationObject as ADObject;
			if (base.IsApplicable(readOnlyPresentationObject))
			{
				return this.IsApplicableRecipientTypeDetails(adobject);
			}
			if (adobject != null && adobject.IsChanged(ADRecipientSchema.RecipientType))
			{
				RecipientType recipientType = (RecipientType)adobject[ADRecipientSchema.RecipientType];
				if (recipientType == RecipientType.DynamicDistributionGroup || recipientType == RecipientType.MailContact || recipientType == RecipientType.MailNonUniversalGroup || recipientType == RecipientType.MailUniversalDistributionGroup || recipientType == RecipientType.MailUniversalSecurityGroup || recipientType == RecipientType.MailUser || recipientType == RecipientType.UserMailbox || recipientType == RecipientType.PublicFolder)
				{
					return this.IsApplicableRecipientTypeDetails(adobject);
				}
			}
			return false;
		}

		public override ProvisioningValidationError[] Validate(ADProvisioningPolicy enforcementPolicy, IConfigurable readOnlyPresentationObject)
		{
			base.Validate(enforcementPolicy, readOnlyPresentationObject);
			ADObject adobject;
			if (readOnlyPresentationObject is ADPublicFolder)
			{
				adobject = (ADPublicFolder)readOnlyPresentationObject;
			}
			else
			{
				adobject = (MailEnabledRecipient)readOnlyPresentationObject;
			}
			Unlimited<int> fromValue = (Unlimited<int>)enforcementPolicy[base.CountQuotaProperty];
			if (!fromValue.IsUnlimited && (T)fromValue >= 0)
			{
				int num = (T)fromValue;
				bool flag;
				if (num == 0)
				{
					flag = true;
				}
				else
				{
					string domainController = null;
					if (base.Context != null && base.Context.UserSpecifiedParameters != null && base.Context.UserSpecifiedParameters.Contains("DomainController"))
					{
						object obj = base.Context.UserSpecifiedParameters["DomainController"];
						if (obj != null)
						{
							domainController = obj.ToString();
						}
					}
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), adobject.OrganizationId, null, false);
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(domainController, true, ConsistencyMode.FullyConsistent, sessionSettings, 178, "Validate", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Provisioning\\RecipientResourceCountQuota.cs");
					flag = SystemAddressListMemberCount.IsQuotaExceded(tenantOrTopologyConfigurationSession, adobject.OrganizationId, base.SystemAddressListName, num);
				}
				if (flag)
				{
					string policyId = string.Format("{0}: {1}", enforcementPolicy.Identity.ToString(), base.CountQuotaProperty.Name);
					LocalizedString description;
					if (adobject.OrganizationalUnitRoot == null)
					{
						description = DirectoryStrings.ErrorExceededHosterResourceCountQuota(policyId, (readOnlyPresentationObject.GetType() == typeof(ADPublicFolder)) ? "MailPublicFolder" : ProvisioningHelper.GetProvisioningObjectTag(readOnlyPresentationObject.GetType()), num);
					}
					else
					{
						description = DirectoryStrings.ErrorExceededMultiTenantResourceCountQuota(policyId, (readOnlyPresentationObject.GetType() == typeof(ADPublicFolder)) ? "MailPublicFolder" : ProvisioningHelper.GetProvisioningObjectTag(readOnlyPresentationObject.GetType()), adobject.OrganizationalUnitRoot.Name, num);
					}
					return new ProvisioningValidationError[]
					{
						new ProvisioningValidationError(description, ExchangeErrorCategory.ServerOperation, null)
					};
				}
			}
			return null;
		}

		private bool IsApplicableRecipientTypeDetails(ADObject recipient)
		{
			return this.targetRecipientTypeDetails == null || this.targetRecipientTypeDetails.Count == 0 || (recipient != null && this.targetRecipientTypeDetails.Contains((RecipientTypeDetails)recipient[ADRecipientSchema.RecipientTypeDetails]));
		}

		private ReadOnlyCollection<RecipientTypeDetails> targetRecipientTypeDetails;
	}
}
