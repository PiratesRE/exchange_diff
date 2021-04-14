using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	[Cmdlet("New", "DarTaskAggregate")]
	public sealed class NewDarTaskAggregate : NewTaskBase<TaskAggregateStoreObject>
	{
		[Parameter(Mandatory = false)]
		public OrganizationIdParameter TenantId { get; set; }

		[Parameter(Mandatory = true)]
		public string TaskType { get; set; }

		[Parameter(Mandatory = false)]
		public bool IsEnabled { get; set; }

		[Parameter(Mandatory = false)]
		public int MaxRunningTasks { get; set; }

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (this.TenantId != null)
			{
				base.CurrentOrganizationId = GetDarTask.ResolveOrganizationId(this.TenantId);
				this.server = GetDarTask.ResolveServerId(base.CurrentOrganizationId);
			}
			this.DataObject.ScopeId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8);
			this.DataObject.TaskType = this.TaskType;
			this.DataObject.MaxRunningTasks = this.MaxRunningTasks;
			this.DataObject.Enabled = this.IsEnabled;
		}

		protected override IConfigDataProvider CreateSession()
		{
			DarTaskAggregateParams darParams = new DarTaskAggregateParams
			{
				TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8),
				TaskId = this.DataObject.Id
			};
			return new DarTaskAggregateDataProvider(darParams, this.server.Fqdn);
		}

		private ServerIdParameter server = new ServerIdParameter();
	}
}
