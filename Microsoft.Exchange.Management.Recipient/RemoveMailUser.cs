using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.SystemConfigurationTasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "MailUser", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMailUser : RemoveMailUserOrRemoteMailboxBase<MailUserIdParameter>
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

		[Parameter]
		public bool Permanent
		{
			get
			{
				return (bool)(base.Fields["Permanent"] ?? false);
			}
			set
			{
				base.Fields["Permanent"] = value;
			}
		}

		protected override bool ShouldSoftDeleteObject()
		{
			ADRecipient dataObject = base.DataObject;
			return dataObject != null && !(dataObject.OrganizationId == null) && dataObject.OrganizationId.ConfigurationUnit != null && !this.Permanent && Globals.IsMicrosoftHostedOnly;
		}

		protected override IConfigDataProvider CreateSession()
		{
			IRecipientSession recipientSession = (IRecipientSession)base.CreateSession();
			if (this.Permanent || this.ForReconciliation)
			{
				recipientSession = SoftDeletedTaskHelper.GetSessionForSoftDeletedObjects(recipientSession, null);
			}
			return recipientSession;
		}

		protected override void InternalProcessRecord()
		{
			if (Globals.IsMicrosoftHostedOnly)
			{
				ADUser dataObject = base.DataObject;
				if (this.ShouldSoftDeleteObject())
				{
					SoftDeletedTaskHelper.UpdateRecipientForSoftDelete(base.DataSession as IRecipientSession, dataObject, this.ForReconciliation);
					SoftDeletedTaskHelper.UpdateExchangeGuidForMailEnabledUser(dataObject);
				}
				else
				{
					dataObject.RecipientSoftDeletedStatus = 0;
				}
			}
			base.InternalProcessRecord();
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.DataObject != null && base.DataObject.CatchAllRecipientBL.Count > 0)
			{
				string domain = string.Join(", ", (from r in base.DataObject.CatchAllRecipientBL
				select r.Name).ToArray<string>());
				base.WriteError(new CannotRemoveMailUserCatchAllRecipientException(domain), ExchangeErrorCategory.Client, base.DataObject.Identity);
			}
		}

		private const string ParameterPermanent = "Permanent";
	}
}
