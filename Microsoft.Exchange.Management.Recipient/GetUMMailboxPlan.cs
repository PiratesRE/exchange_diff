using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "UMMailboxPlan", DefaultParameterSetName = "Identity")]
	public sealed class GetUMMailboxPlan : GetUMMailboxBase<MailboxPlanIdParameter>
	{
		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return GetUMMailboxPlan.AllowedRecipientTypeDetails;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return UMMailboxPlan.FromDataObject((ADRecipient)dataObject);
		}

		internal new string Anr
		{
			get
			{
				return null;
			}
		}

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.MailboxPlan
		};
	}
}
