using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "MailContact", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMailContact : RemoveMailContactBase
	{
		[Parameter(Mandatory = false)]
		public new SwitchParameter ForReconciliation
		{
			get
			{
				return base.ForReconciliation;
			}
			set
			{
				base.ForReconciliation = value;
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			ADContact dataObject = base.DataObject;
			if (dataObject.RecipientTypeDetails == RecipientTypeDetails.MailForestContact)
			{
				base.WriteError(new TaskInvalidOperationException(Strings.RemoveMailForestContactNotAllowed(dataObject.Name)), ExchangeErrorCategory.Client, base.DataObject.Identity);
			}
			if (base.DataObject.CatchAllRecipientBL.Count > 0)
			{
				string domain = string.Join(", ", (from r in base.DataObject.CatchAllRecipientBL
				select r.Name).ToArray<string>());
				base.WriteError(new CannotRemoveMailContactCatchAllRecipientException(domain), ExchangeErrorCategory.Client, base.DataObject.Identity);
			}
		}
	}
}
