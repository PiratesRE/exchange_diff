using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	public abstract class TenantReportBase<TReportObject> : DateReportTaskBase<TReportObject> where TReportObject : ReportObject, IDateColumn, ITenantColumn
	{
		protected TenantReportBase()
		{
			this.tenantDecorator = new TenantDecorator<TReportObject>(base.TaskContext);
			this.tenantDecorator.IsPipeline = true;
			base.AddQueryDecorator(this.tenantDecorator);
		}

		[Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		protected override DataMartType DataMartType
		{
			get
			{
				return DataMartType.Tenants;
			}
		}

		protected override void ProcessPipelineParameter()
		{
			base.ProcessPipelineParameter();
			Guid? tenantExternalDirectoryId;
			this.tenantDecorator.TenantGuid = ADHelper.ResolveOrganizationGuid(this.Organization, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, out tenantExternalDirectoryId);
			this.tenantDecorator.TenantExternalDirectoryId = tenantExternalDirectoryId;
		}

		private const string OrganizationKey = "Organization";

		private readonly TenantDecorator<TReportObject> tenantDecorator;
	}
}
