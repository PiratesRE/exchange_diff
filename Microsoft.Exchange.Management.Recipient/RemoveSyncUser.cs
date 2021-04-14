using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "SyncUser", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSyncUser : RemoveRecipientObjectTask<NonMailEnabledUserIdParameter, ADUser>
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

		[Parameter(Mandatory = false)]
		public SwitchParameter KeepWindowsLiveID
		{
			get
			{
				return (SwitchParameter)(base.Fields["KeepWindowsLiveID"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["KeepWindowsLiveID"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveUser(this.Identity.ToString());
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

		protected override void InternalProcessRecord()
		{
			if (Globals.IsMicrosoftHostedOnly)
			{
				ADUser dataObject = base.DataObject;
				if (this.ShouldSoftDeleteObject())
				{
					SoftDeletedTaskHelper.UpdateRecipientForSoftDelete(base.DataSession as IRecipientSession, dataObject, this.ForReconciliation);
				}
				else
				{
					dataObject.propertyBag.SetField(ADRecipientSchema.RecipientSoftDeletedStatus, 0);
				}
			}
			base.InternalProcessRecord();
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncUser.FromDataObject((ADUser)dataObject);
		}

		private const string ParameterPermanent = "Permanent";

		private const string ParameterKeepWindowsLiveID = "KeepWindowsLiveID";
	}
}
