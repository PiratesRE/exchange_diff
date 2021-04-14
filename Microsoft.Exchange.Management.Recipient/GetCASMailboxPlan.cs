using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Get", "CASMailboxPlan", DefaultParameterSetName = "Identity")]
	public sealed class GetCASMailboxPlan : GetCASMailboxBase<MailboxPlanIdParameter>
	{
		internal new string Anr
		{
			get
			{
				return null;
			}
		}

		protected override RecipientTypeDetails[] InternalRecipientTypeDetails
		{
			get
			{
				return GetCASMailboxPlan.AllowedRecipientTypeDetails;
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			if (dataObject == null)
			{
				return null;
			}
			return CASMailboxPlan.FromDataObject((ADUser)dataObject);
		}

		private static readonly RecipientTypeDetails[] AllowedRecipientTypeDetails = new RecipientTypeDetails[]
		{
			RecipientTypeDetails.MailboxPlan
		};
	}
}
