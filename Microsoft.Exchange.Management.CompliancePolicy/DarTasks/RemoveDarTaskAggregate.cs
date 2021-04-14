using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	[Cmdlet("Remove", "DarTaskAggregate")]
	public sealed class RemoveDarTaskAggregate : Task
	{
		[Parameter(Mandatory = false)]
		public OrganizationIdParameter TenantId { get; set; }

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public string TaskType { get; set; }

		protected override void InternalProcessRecord()
		{
			if (this.TenantId != null)
			{
				base.CurrentOrganizationId = GetDarTask.ResolveOrganizationId(this.TenantId);
			}
			string fqdn = GetDarTask.ResolveServerId(base.CurrentOrganizationId).Fqdn;
			DarTaskAggregateParams darTaskAggregateParams = new DarTaskAggregateParams
			{
				TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8),
				TaskType = this.TaskType
			};
			using (HostRpcClient hostRpcClient = new HostRpcClient(fqdn))
			{
				hostRpcClient.RemoveDarTaskAggregate(darTaskAggregateParams);
			}
		}
	}
}
