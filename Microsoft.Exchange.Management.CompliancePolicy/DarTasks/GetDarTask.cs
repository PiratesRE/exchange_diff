using System;
using System.Linq;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Compliance.DAR;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.CompliancePolicy.DarTasks.Protocol;
using Microsoft.Office.CompliancePolicy.Dar;

namespace Microsoft.Exchange.Management.CompliancePolicy.DarTasks
{
	[Cmdlet("Get", "DarTask")]
	public sealed class GetDarTask : GetTaskBase<TaskStoreObject>
	{
		public GetDarTask()
		{
			this.ExecutionUnit = new ServerIdParameter();
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public ServerIdParameter ExecutionUnit { get; set; }

		[Parameter(Mandatory = false)]
		public OrganizationIdParameter TenantId { get; set; }

		[Parameter(Mandatory = false)]
		public string TaskId { get; set; }

		[Parameter(Mandatory = false)]
		public DarTaskState TaskState { get; set; }

		[Parameter(Mandatory = false)]
		public string TaskType { get; set; }

		[Parameter(Mandatory = false)]
		public DateTime MinQueuedTime { get; set; }

		[Parameter(Mandatory = false)]
		public DateTime MaxQueuedTime { get; set; }

		[Parameter(Mandatory = false)]
		public DateTime MinCompletionTime { get; set; }

		[Parameter(Mandatory = false)]
		public DateTime MaxCompletionTIme { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter ActiveInRuntime { get; set; }

		internal static OrganizationId ResolveOrganizationId(OrganizationIdParameter tenantId)
		{
			if (tenantId == null)
			{
				throw new ArgumentNullException("tenantId");
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 146, "ResolveOrganizationId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\CompliancePolicy\\DarTasks\\GetDarTask.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADOrganizationalUnit adorganizationalUnit = tenantId.GetObjects<ADOrganizationalUnit>(null, tenantOrTopologyConfigurationSession).FirstOrDefault<ADOrganizationalUnit>();
			if (adorganizationalUnit == null)
			{
				throw new ArgumentException(Strings.ErrorOrganizationNotFound(tenantId.ToString()));
			}
			return adorganizationalUnit.OrganizationId;
		}

		internal static ServerIdParameter ResolveServerId(OrganizationId orgId)
		{
			ExchangePrincipal tenantMailbox = TenantStoreDataProvider.GetTenantMailbox(orgId);
			return new ServerIdParameter(new Fqdn(tenantMailbox.MailboxInfo.Location.ServerFqdn));
		}

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
			DarTaskParams darParams = new DarTaskParams
			{
				TaskId = this.TaskId,
				TenantId = base.CurrentOrganizationId.GetBytes(Encoding.UTF8),
				TaskState = this.TaskState,
				TaskType = this.TaskType,
				MaxCompletionTime = this.MaxCompletionTIme,
				MaxQueuedTime = this.MaxQueuedTime,
				MinCompletionTime = this.MinCompletionTime,
				MinQueuedTime = this.MinQueuedTime,
				ActiveInRuntime = this.ActiveInRuntime.ToBool()
			};
			return new DarTaskDataProvider(darParams, this.ExecutionUnit.Fqdn);
		}
	}
}
