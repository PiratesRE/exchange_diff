using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.LatencyDetection;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Remove", "SyncMailbox", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSyncMailbox : RemoveMailboxBase<MailboxIdParameter>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter DisableWindowsLiveID { get; set; }

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

		protected override IConfigurable ConvertDataObjectToPresentationObject(IConfigurable dataObject)
		{
			return SyncMailbox.FromDataObject((ADUser)dataObject);
		}

		protected override void InternalValidate()
		{
			this.latencyContext = ProvisioningPerformanceHelper.StartLatencyDetection(this);
			base.InternalValidate();
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

		protected override bool ShouldSoftDeleteObject()
		{
			ADRecipient dataObject = base.DataObject;
			return dataObject != null && !(dataObject.OrganizationId == null) && dataObject.OrganizationId.ConfigurationUnit != null && SoftDeletedTaskHelper.IsSoftDeleteSupportedRecipientTypeDetail(dataObject.RecipientTypeDetails) && !base.Disconnect && !base.Permanent && VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).CmdletInfra.SoftDeleteObject.Enabled;
		}

		private LatencyDetectionContext latencyContext;
	}
}
