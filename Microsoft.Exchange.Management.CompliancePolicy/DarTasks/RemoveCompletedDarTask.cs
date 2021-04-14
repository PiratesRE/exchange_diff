using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	[Cmdlet("Remove", "CompletedDarTask")]
	public sealed class RemoveCompletedDarTask : Task
	{
		[Parameter(Mandatory = false)]
		public OrganizationIdParameter TenantId { get; set; }

		[Parameter(Mandatory = true)]
		[ValidateNotNullOrEmpty]
		public DateTime MaxCompletionTime { get; set; }

		[Parameter(Mandatory = false)]
		public string TaskType { get; set; }

		protected override void InternalProcessRecord()
		{
			if (this.TenantId != null)
			{
				base.CurrentOrganizationId = GetDarTask.ResolveOrganizationId(this.TenantId);
			}
			string fqdn = GetDarTask.ResolveServerId(base.CurrentOrganizationId).Fqdn;
			DarTaskParams darParams = new DarTaskParams
			{
				TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8),
				MaxCompletionTime = this.MaxCompletionTime,
				TaskType = this.TaskType
			};
			using (HostRpcClient hostRpcClient = new HostRpcClient(fqdn))
			{
				hostRpcClient.RemoveCompletedDarTasks(darParams);
			}
		}
	}
}
