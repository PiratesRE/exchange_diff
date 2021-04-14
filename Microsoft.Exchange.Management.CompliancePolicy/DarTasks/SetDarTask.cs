using System;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;
using Microsoft.Exchange.Management.CompliancePolicy.LocStrings;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	[Cmdlet("Set", "DarTask")]
	public sealed class SetDarTask : SetTaskBase<TaskStoreObject>
	{
		[Parameter(Mandatory = false)]
		public OrganizationIdParameter TenantId { get; set; }

		[Parameter(Mandatory = true)]
		public string TaskId { get; set; }

		[Parameter(Mandatory = false)]
		public DarTaskState TaskState { get; set; }

		[Parameter(Mandatory = false)]
		public int Priority { get; set; }

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.TenantId != null)
			{
				base.CurrentOrganizationId = GetDarTask.ResolveOrganizationId(this.TenantId);
				this.server = GetDarTask.ResolveServerId(base.CurrentOrganizationId);
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			DarTaskParams darParams = new DarTaskParams
			{
				TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8),
				TaskId = this.TaskId
			};
			return new DarTaskDataProvider(darParams, this.server.Fqdn);
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskStoreObject taskStoreObject = base.DataSession.FindPaged<TaskStoreObject>(null, null, false, null, 0).FirstOrDefault<TaskStoreObject>();
			if (taskStoreObject == null)
			{
				throw new DataSourceOperationException(new LocalizedString(Strings.TaskNotFound));
			}
			taskStoreObject.Id = this.TaskId;
			taskStoreObject.TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8);
			taskStoreObject.TaskState = this.TaskState;
			taskStoreObject.Priority = this.Priority;
			return taskStoreObject;
		}

		private ServerIdParameter server = new ServerIdParameter();
	}
}
