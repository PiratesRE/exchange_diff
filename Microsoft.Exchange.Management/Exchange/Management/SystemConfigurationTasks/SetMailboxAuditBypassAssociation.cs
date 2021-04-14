using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "MailboxAuditBypassAssociation", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxAuditBypassAssociation : SetRecipientObjectTask<MailboxAuditBypassAssociationIdParameter, MailboxAuditBypassAssociation, ADRecipient>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageMailboxAuditBypassAssociation(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = true)]
		public bool AuditBypassEnabled
		{
			get
			{
				return (bool)base.Fields[ADRecipientSchema.AuditBypassEnabled];
			}
			set
			{
				base.Fields[ADRecipientSchema.AuditBypassEnabled] = value;
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			ADRecipient adrecipient = (ADRecipient)dataObject;
			adrecipient.BypassAudit = this.AuditBypassEnabled;
			base.StampChangesOn(adrecipient);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			ADRecipient dataObject2 = (ADRecipient)dataObject;
			return MailboxAuditBypassAssociation.FromDataObject(dataObject2);
		}

		internal new SwitchParameter IgnoreDefaultScope
		{
			get
			{
				return new SwitchParameter(false);
			}
		}
	}
}
