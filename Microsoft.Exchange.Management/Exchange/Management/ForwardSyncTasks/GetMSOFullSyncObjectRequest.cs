using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Get", "MSOFullSyncObjectRequest", DefaultParameterSetName = "ServiceInstanceIdParameterSet")]
	public sealed class GetMSOFullSyncObjectRequest : GetTaskBase<FullSyncObjectRequest>
	{
		[Parameter(ParameterSetName = "IdentityParameterSet", Mandatory = true)]
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

		[Parameter(ParameterSetName = "ServiceInstanceIdParameterSet", Mandatory = false)]
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

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (base.ParameterSetName == "IdentityParameterSet")
				{
					return new ComparisonFilter(ComparisonOperator.Equal, SimpleProviderObjectSchema.Identity, this.Identity);
				}
				if (this.ServiceInstanceId != null)
				{
					return new ComparisonFilter(ComparisonOperator.Equal, FullSyncObjectRequestSchema.ServiceInstanceId, this.ServiceInstanceId.InstanceId);
				}
				return null;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return new FullSyncObjectRequestDataProvider(true, (this.ServiceInstanceId != null) ? this.ServiceInstanceId.InstanceId : null);
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is RidMasterConfigException;
		}

		private const string SyncObjectIdParameter = "SyncObjectIdParameter";

		private const string ServiceInstanceIdParameter = "ServiceInstanceIdParameter";

		private const string ServiceInstanceIdParameterSet = "ServiceInstanceIdParameterSet";

		private const string IdentityParameterSet = "IdentityParameterSet";
	}
}
