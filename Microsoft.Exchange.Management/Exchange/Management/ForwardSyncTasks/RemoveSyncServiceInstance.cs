using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Remove", "SyncServiceInstance", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSyncServiceInstance : RemoveSystemConfigurationObjectTask<ServiceInstanceIdParameter, SyncServiceInstance>
	{
		protected override ObjectId RootId
		{
			get
			{
				return SyncServiceInstance.GetMsoSyncRootContainer();
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveSyncServiceInstance(base.DataObject.Name);
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (!this.Force && !SetSyncServiceInstance.IsServiceInstanceEmpty(base.DataObject))
			{
				base.WriteError(new InvalidOperationException(Strings.CannotRemoveServiceInstanceError(base.DataObject.Name)), ErrorCategory.InvalidOperation, base.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity,
				base.DataObject
			});
			try
			{
				((ITopologyConfigurationSession)base.DataSession).DeleteTree(base.DataObject, null);
			}
			catch (DataSourceTransientException exception)
			{
				base.WriteError(exception, ExchangeErrorCategory.ServerTransient, base.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}
	}
}
