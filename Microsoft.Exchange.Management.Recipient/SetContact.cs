using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "Contact", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetContact : SetOrgPersonObjectTask<ContactIdParameter, Contact, ADContact>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetContact(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			ADContact dataObject = this.DataObject;
			if (dataObject.RecipientTypeDetails == RecipientTypeDetails.MailForestContact && this.IsObjectStateChanged())
			{
				base.WriteError(new InvalidOperationException(Strings.SetMailForestContactNotAllowed(dataObject.Name)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return Contact.FromDataObject((ADContact)dataObject);
		}
	}
}
