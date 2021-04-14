using System;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	[Cmdlet("Set", "DarTaskAggregate")]
	public sealed class SetDarTaskAggregate : SetTaskBase<TaskAggregateStoreObject>
	{
		[Parameter(Mandatory = false)]
		public OrganizationIdParameter TenantId { get; set; }

		[Parameter(Mandatory = true)]
		public string TaskType { get; set; }

		[Parameter(Mandatory = false)]
		public bool IsEnabled { get; set; }

		[Parameter(Mandatory = false)]
		public int MaxRunningTasks { get; set; }

		protected override IConfigDataProvider CreateSession()
		{
			DarTaskAggregateParams darParams = new DarTaskAggregateParams
			{
				TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8),
				TaskType = this.TaskType
			};
			return new DarTaskAggregateDataProvider(darParams, this.server.Fqdn);
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.TenantId != null)
			{
				base.CurrentOrganizationId = GetDarTask.ResolveOrganizationId(this.TenantId);
				this.server = GetDarTask.ResolveServerId(base.CurrentOrganizationId);
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			TaskAggregateStoreObject taskAggregateStoreObject = base.DataSession.FindPaged<TaskAggregateStoreObject>(null, null, false, null, 0).FirstOrDefault<TaskAggregateStoreObject>();
			if (taskAggregateStoreObject == null)
			{
				taskAggregateStoreObject = new TaskAggregateStoreObject();
			}
			taskAggregateStoreObject.ScopeId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8);
			taskAggregateStoreObject.TaskType = this.TaskType;
			taskAggregateStoreObject.MaxRunningTasks = this.MaxRunningTasks;
			taskAggregateStoreObject.Enabled = this.IsEnabled;
			return taskAggregateStoreObject;
		}

		private ServerIdParameter server = new ServerIdParameter();
	}
}
