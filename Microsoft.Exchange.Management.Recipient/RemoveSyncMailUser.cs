using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "SyncMailUser", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSyncMailUser : RemoveMailUserBase<MailUserIdParameter>
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter DisableWindowsLiveID { get; set; }

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
			return dataObject != null && !(dataObject.OrganizationId == null) && dataObject.OrganizationId.ConfigurationUnit != null && !this.Permanent && Globals.IsMicrosoftHostedOnly && SoftDeletedTaskHelper.MSOSyncEnabled(this.ConfigurationSession, dataObject.OrganizationId);
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

		protected override void InternalValidate()
		{
			this.isSyncOperation = true;
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			if (Globals.IsMicrosoftHostedOnly)
			{
				ADUser dataObject = base.DataObject;
				if (this.ShouldSoftDeleteObject())
				{
					SoftDeletedTaskHelper.UpdateRecipientForSoftDelete(base.DataSession as IRecipientSession, dataObject, false);
					SoftDeletedTaskHelper.UpdateExchangeGuidForMailEnabledUser(dataObject);
				}
				else
				{
					dataObject.RecipientSoftDeletedStatus = 0;
				}
			}
			base.InternalProcessRecord();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncMailUser.FromDataObject((ADUser)dataObject);
		}

		private const string ParameterPermanent = "Permanent";
	}
}
