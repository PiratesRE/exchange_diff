using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("disable", "DistributionGroup", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableDistributionGroup : RecipientObjectActionTask<DistributionGroupIdParameter, ADGroup>
	{
		[Parameter]
		public SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return base.InternalIgnoreDefaultScope;
			}
			set
			{
				base.InternalIgnoreDefaultScope = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDisableDistributionGroup(this.Identity.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADGroup adgroup = (ADGroup)base.PrepareDataObject();
			if (adgroup != null && (adgroup.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox || adgroup.RecipientTypeDetails == RecipientTypeDetails.RemoteGroupMailbox))
			{
				base.WriteError(new RecipientTaskException(Strings.NotAValidDistributionGroup), ExchangeErrorCategory.Client, this.Identity.ToString());
			}
			MailboxTaskHelper.ClearExchangeProperties(adgroup, DisableDistributionGroup.PropertiesToReset);
			adgroup.SetExchangeVersion(null);
			adgroup.OverrideCorruptedValuesWithDefault();
			TaskLogger.LogExit();
			return adgroup;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return DistributionGroup.FromDataObject((ADGroup)dataObject);
		}

		internal static readonly PropertyDefinition[] PropertiesToReset = new PropertyDefinition[]
		{
			ADRecipientSchema.AcceptMessagesOnlyFrom,
			ADRecipientSchema.AcceptMessagesOnlyFromDLMembers,
			ADRecipientSchema.AddressListMembership,
			ADRecipientSchema.Alias,
			ADRecipientSchema.EmailAddresses,
			ADGroupSchema.ExpansionServer,
			ADRecipientSchema.GrantSendOnBehalfTo,
			ADRecipientSchema.HiddenFromAddressListsEnabled,
			ADRecipientSchema.HomeMTA,
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.MaxReceiveSize,
			ADRecipientSchema.MaxSendSize,
			ADRecipientSchema.PoliciesExcluded,
			ADRecipientSchema.PoliciesIncluded,
			ADRecipientSchema.RecipientDisplayType,
			ADRecipientSchema.RecipientTypeDetails,
			ADRecipientSchema.RejectMessagesFrom,
			ADRecipientSchema.RejectMessagesFromDLMembers,
			ADRecipientSchema.RequireAllSendersAreAuthenticated,
			ADGroupSchema.ReportToManagerEnabled,
			ADGroupSchema.ReportToOriginatorEnabled,
			ADGroupSchema.SendDeliveryReportsTo,
			ADGroupSchema.SendOofMessageToOriginatorEnabled,
			ADRecipientSchema.SimpleDisplayName,
			ADRecipientSchema.TextEncodedORAddress,
			ADRecipientSchema.WindowsEmailAddress,
			ADRecipientSchema.CustomAttribute1,
			ADRecipientSchema.CustomAttribute2,
			ADRecipientSchema.CustomAttribute3,
			ADRecipientSchema.CustomAttribute4,
			ADRecipientSchema.CustomAttribute5,
			ADRecipientSchema.CustomAttribute6,
			ADRecipientSchema.CustomAttribute7,
			ADRecipientSchema.CustomAttribute8,
			ADRecipientSchema.CustomAttribute9,
			ADRecipientSchema.CustomAttribute10,
			ADRecipientSchema.CustomAttribute11,
			ADRecipientSchema.CustomAttribute12,
			ADRecipientSchema.CustomAttribute13,
			ADRecipientSchema.CustomAttribute14,
			ADRecipientSchema.CustomAttribute15,
			ADRecipientSchema.ExtensionCustomAttribute1,
			ADRecipientSchema.ExtensionCustomAttribute2,
			ADRecipientSchema.ExtensionCustomAttribute3,
			ADRecipientSchema.ExtensionCustomAttribute4,
			ADRecipientSchema.ExtensionCustomAttribute5
		};
	}
}
