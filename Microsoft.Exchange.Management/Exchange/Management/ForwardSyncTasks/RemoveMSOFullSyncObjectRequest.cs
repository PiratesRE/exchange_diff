using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Remove", "MSOFullSyncObjectRequest", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMSOFullSyncObjectRequest : DataAccessTask<FullSyncObjectRequest>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public SyncObjectId Identity
		{
			get
			{
				return (SyncObjectId)base.Fields["SyncObjectIdParameter"];
			}
			set
			{
				base.Fields["SyncObjectIdParameter"] = value;
			}
		}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public ServiceInstanceId ServiceInstanceId
		{
			get
			{
				return (ServiceInstanceId)base.Fields["ServiceInstanceIdParameter"];
			}
			set
			{
				base.Fields["ServiceInstanceIdParameter"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMSOFullSyncRequest(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new FullSyncObjectRequestDataProvider(false, this.ServiceInstanceId.InstanceId);
		}

		protected override void InternalProcessRecord()
		{
			base.DataSession.Delete(this.request);
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			this.request = base.DataSession.Read<FullSyncObjectRequest>(this.Identity);
			if (this.request == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorManagementObjectNotFound(this.Identity.ToString())), ExchangeErrorCategory.Client, this.Identity);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is RidMasterConfigException;
		}

		private const string SyncObjectIdParameter = "SyncObjectIdParameter";

		private const string ServiceInstanceIdParameter = "ServiceInstanceIdParameter";

		private IConfigurable request;
	}
}
