using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	[Cmdlet("New", "DarTask")]
	public sealed class NewDarTask : NewTaskBase<TaskStoreObject>
	{
		[Parameter(Mandatory = false)]
		public OrganizationIdParameter TenantId { get; set; }

		[Parameter(Mandatory = true)]
		public string TaskType { get; set; }

		[Parameter(Mandatory = false)]
		public int Priority { get; set; }

		[Parameter(Mandatory = false)]
		public string SerializedData { get; set; }

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.TenantId != null)
			{
				base.CurrentOrganizationId = GetDarTask.ResolveOrganizationId(this.TenantId);
				this.server = GetDarTask.ResolveServerId(base.CurrentOrganizationId);
			}
			this.DataObject.Id = Guid.NewGuid().ToString();
			this.DataObject.TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8);
			this.DataObject.TaskType = this.TaskType;
			this.DataObject.Priority = this.Priority;
			this.DataObject.SerializedTaskData = this.SerializedData;
		}

		protected override IConfigDataProvider CreateSession()
		{
			DarTaskParams darParams = new DarTaskParams
			{
				TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8),
				TaskId = this.DataObject.Id
			};
			return new DarTaskDataProvider(darParams, this.server.Fqdn);
		}

		private ServerIdParameter server = new ServerIdParameter();
	}
}
