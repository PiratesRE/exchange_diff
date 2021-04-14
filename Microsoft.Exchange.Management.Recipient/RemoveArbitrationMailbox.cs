using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "ArbitrationMailbox", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveArbitrationMailbox : RemoveMailboxBase<MailboxIdParameter>
	{
		public RemoveArbitrationMailbox()
		{
			base.Arbitration = new SwitchParameter(true);
			base.RemoveLastArbitrationMailboxAllowed = new SwitchParameter(true);
			base.Permanent = true;
		}

		internal override bool ArbitrationMailboxUsageValidationRequired
		{
			get
			{
				return false;
			}
		}

		protected override IConfigurable ResolveDataObject()
		{
			ADRecipient adrecipient = (ADRecipient)base.ResolveDataObject();
			if (adrecipient.RecipientTypeDetails != RecipientTypeDetails.ArbitrationMailbox)
			{
				base.WriteError(new RecipientTaskException(Strings.ErrorInvalidArbitrationMbxType(adrecipient.Id.ToString())), ExchangeErrorCategory.Client, this.Identity);
			}
			return adrecipient;
		}
	}
}
