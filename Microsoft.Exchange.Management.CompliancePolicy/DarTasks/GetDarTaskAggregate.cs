using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	[Cmdlet("Get", "DarTaskAggregate")]
	public sealed class GetDarTaskAggregate : GetTaskBase<TaskAggregateStoreObject>
	{
		public GetDarTaskAggregate()
		{
			this.ExecutionUnit = new ServerIdParameter();
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public ServerIdParameter ExecutionUnit { get; set; }

		[Parameter(Mandatory = false)]
		public OrganizationIdParameter TenantId { get; set; }

		[Parameter(Mandatory = false)]
		public string TaskType { get; set; }

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.TenantId != null)
			{
				base.CurrentOrganizationId = GetDarTask.ResolveOrganizationId(this.TenantId);
				this.ExecutionUnit = GetDarTask.ResolveServerId(base.CurrentOrganizationId);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			DarTaskAggregateParams darParams = new DarTaskAggregateParams
			{
				TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8),
				TaskType = this.TaskType
			};
			return new DarTaskAggregateDataProvider(darParams, this.ExecutionUnit.Fqdn);
		}
	}
}
