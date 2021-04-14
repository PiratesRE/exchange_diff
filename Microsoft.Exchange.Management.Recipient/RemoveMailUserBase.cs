using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public abstract class RemoveMailUserBase<TIdentity> : RemoveRecipientObjectTask<TIdentity, ADUser> where TIdentity : MailUserIdParameterBase
	{
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

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreLegalHold
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreLegalHold"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreLegalHold"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (!(base.DataObject.NetID != null))
				{
					TIdentity identity = this.Identity;
					return Strings.ConfirmationMessageRemoveMailUser(identity.ToString());
				}
				if (this.KeepWindowsLiveID)
				{
					TIdentity identity2 = this.Identity;
					return Strings.ConfirmationMessageRemoveMailuserAndNotLiveId(identity2.ToString(), base.DataObject.WindowsLiveID.ToString());
				}
				TIdentity identity3 = this.Identity;
				return Strings.ConfirmationMessageRemoveMailuserAndLiveId(identity3.ToString(), base.DataObject.WindowsLiveID.ToString());
			}
		}

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return MailUser.FromDataObject((ADUser)dataObject);
		}

		protected override void InternalValidate()
		{
			this.latencyContext = ProvisioningPerformanceHelper.StartLatencyDetection(this);
			base.InternalValidate();
			MailboxTaskHelper.BlockRemoveOrDisableIfLitigationHoldEnabled(base.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), false, this.IgnoreLegalHold.ToBool());
			MailboxTaskHelper.BlockRemoveOrDisableIfDiscoveryHoldEnabled(base.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), false, this.IgnoreLegalHold.ToBool());
			if (ComplianceConfigImpl.JournalArchivingHardeningEnabled)
			{
				MailboxTaskHelper.BlockRemoveOrDisableMailUserIfJournalArchiveEnabled(base.DataSession as IRecipientSession, this.ConfigurationSession, base.DataObject, new Task.ErrorLoggerDelegate(base.WriteError), false, this.isSyncOperation);
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
			}
			finally
			{
				ProvisioningPerformanceHelper.StopLatencyDetection(this.latencyContext);
			}
		}

		private const string ParameterKeepWindowsLiveID = "KeepWindowsLiveID";

		private LatencyDetectionContext latencyContext;

		protected bool isSyncOperation;
	}
}
