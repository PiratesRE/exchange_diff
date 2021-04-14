using System;
using System.Management.Automation;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	[Cmdlet("Test", "DarTask")]
	public sealed class TestDarTask : Task
	{
		public TestDarTask()
		{
			this.Retries = 10;
		}

		[Parameter(Mandatory = false)]
		public OrganizationIdParameter TenantId { get; set; }

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public int Retries { get; set; }

		protected override void InternalProcessRecord()
		{
			base.WriteObject("Sending no op task");
			OrganizationId organizationId = (this.TenantId != null) ? GetDarTask.ResolveOrganizationId(this.TenantId) : OrganizationId.ForestWideOrgId;
			string fqdn = GetDarTask.ResolveServerId(base.CurrentOrganizationId).Fqdn;
			using (HostRpcClient hostRpcClient = new HostRpcClient(fqdn))
			{
				string text = Guid.NewGuid().ToString();
				TaskStoreObject darTask = new TaskStoreObject
				{
					Id = text,
					TaskType = "Common.NoOp",
					TenantId = organizationId.GetBytes(Encoding.UTF8)
				};
				hostRpcClient.SetDarTask(darTask);
				base.WriteObject("Task enqueued, waiting for completed status. ID:" + text);
				for (int i = 0; i < this.Retries; i++)
				{
					TaskStoreObject[] darTask2 = hostRpcClient.GetDarTask(new DarTaskParams
					{
						TaskId = text
					});
					if (darTask2.Length > 1)
					{
						base.WriteError(new Exception("Unexected number of tasks returned by GetDarTask"), ErrorCategory.InvalidResult, darTask2.Length);
						return;
					}
					if (darTask2.Length == 1)
					{
						base.WriteObject("Task state: " + darTask2[0].TaskState);
						if (darTask2[0].TaskState == DarTaskState.Completed)
						{
							return;
						}
					}
					else
					{
						base.WriteObject("No tasks found");
					}
					Thread.Sleep(1000);
				}
			}
			base.WriteError(new Exception("Operation timeout"), ErrorCategory.OperationTimeout, null);
		}
	}
}
