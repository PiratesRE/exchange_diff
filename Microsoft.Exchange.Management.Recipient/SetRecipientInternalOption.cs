using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Set", "RecipientInternalOption", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetRecipientInternalOption : SetRecipientObjectTask<RecipientIdParameter, ADRecipient, ADRecipient>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter InternalOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields[ADRecipientSchema.InternalOnly] ?? false);
			}
			set
			{
				base.Fields[ADRecipientSchema.InternalOnly] = value;
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			if (base.Fields.IsModified(ADRecipientSchema.InternalOnly))
			{
				ADRecipient adrecipient = (ADRecipient)dataObject;
				adrecipient.InternalOnly = this.InternalOnly;
			}
			base.StampChangesOn(dataObject);
		}
	}
}
