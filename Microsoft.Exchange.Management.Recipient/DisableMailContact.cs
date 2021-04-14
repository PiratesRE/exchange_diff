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
	[Cmdlet("Disable", "MailContact", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableMailContact : RecipientObjectActionTask<MailContactIdParameter, ADContact>
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
				return Strings.ConfirmationMessageDisableMailContact(this.Identity.ToString());
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			ADContact adcontact = (ADContact)base.PrepareDataObject();
			if (adcontact.RecipientTypeDetails == RecipientTypeDetails.MailForestContact)
			{
				base.WriteError(new InvalidOperationException(Strings.DisableMailForestContactNotAllowed(adcontact.Name)), ErrorCategory.InvalidOperation, adcontact.Identity);
			}
			MailboxTaskHelper.ClearExchangeProperties(adcontact, DisableMailContact.PropertiesToReset);
			adcontact.SetExchangeVersion(null);
			adcontact.OverrideCorruptedValuesWithDefault();
			TaskLogger.LogExit();
			return adcontact;
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MailContact.FromDataObject((ADContact)dataObject);
		}

		internal static readonly PropertyDefinition[] PropertiesToReset = new PropertyDefinition[]
		{
			ADRecipientSchema.AcceptMessagesOnlyFrom,
			ADRecipientSchema.AcceptMessagesOnlyFromDLMembers,
			ADRecipientSchema.AddressListMembership,
			ADRecipientSchema.Alias,
			ADContactSchema.DeliverToMailboxAndForward,
			ADRecipientSchema.EmailAddresses,
			ADRecipientSchema.ForwardingAddress,
			ADRecipientSchema.GrantSendOnBehalfTo,
			ADRecipientSchema.HiddenFromAddressListsEnabled,
			ADRecipientSchema.InternetEncoding,
			ADRecipientSchema.LegacyExchangeDN,
			ADRecipientSchema.MaxReceiveSize,
			ADRecipientSchema.MaxSendSize,
			ADRecipientSchema.MapiRecipient,
			ADRecipientSchema.PoliciesExcluded,
			ADRecipientSchema.PoliciesIncluded,
			ADRecipientSchema.RawExternalEmailAddress,
			ADRecipientSchema.RecipientDisplayType,
			ADRecipientSchema.RecipientLimits,
			ADRecipientSchema.RecipientTypeDetails,
			ADRecipientSchema.RejectMessagesFrom,
			ADRecipientSchema.RejectMessagesFromDLMembers,
			ADRecipientSchema.RequireAllSendersAreAuthenticated,
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
