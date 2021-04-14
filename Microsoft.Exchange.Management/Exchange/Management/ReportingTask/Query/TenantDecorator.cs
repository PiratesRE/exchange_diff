using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.TenantReport;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask.Query
{
	internal class TenantDecorator<TReportObject> : QueryDecorator<TReportObject> where TReportObject : class, ITenantColumn
	{
		public TenantDecorator(ITaskContext taskContext) : base(taskContext)
		{
			base.IsEnforced = true;
			this.reportTypesUseExchangeTenantGuid = new ConcurrentBag<Type>
			{
				typeof(ConnectionByClientTypeDetailReport),
				typeof(ConnectionByClientTypeReport),
				typeof(GroupActivityReport),
				typeof(MailboxActivityReport),
				typeof(MailboxUsageDetailReport),
				typeof(MailboxUsageReport),
				typeof(StaleMailboxDetailReport),
				typeof(StaleMailboxReport)
			};
		}

		public Guid? TenantGuid { get; set; }

		public Guid? TenantExternalDirectoryId { get; set; }

		public override IQueryable<TReportObject> GetQuery(IQueryable<TReportObject> query)
		{
			Guid? tenantGuid;
			Guid? tenantExternalDirectoryId;
			if (base.TaskContext.IsCurrentOrganizationForestWide)
			{
				tenantGuid = this.TenantGuid;
				tenantExternalDirectoryId = this.TenantExternalDirectoryId;
			}
			else
			{
				tenantGuid = new Guid?(base.TaskContext.CurrentOrganizationGuid);
				tenantExternalDirectoryId = new Guid?(base.TaskContext.CurrentOrganizationExternalDirectoryId);
			}
			Type typeFromHandle = typeof(TReportObject);
			if (this.reportTypesUseExchangeTenantGuid.Contains(typeFromHandle))
			{
				query = from report in query
				where (Guid?)report.TenantGuid == tenantGuid
				select report;
			}
			else
			{
				query = from report in query
				where (Guid?)report.TenantGuid == tenantExternalDirectoryId
				select report;
			}
			return query;
		}

		public override void Validate()
		{
			base.Validate();
			if (base.TaskContext.IsCurrentOrganizationForestWide && this.TenantGuid == null)
			{
				base.TaskContext.WriteError(new ReportingException(Strings.OrganizationNotSpecified), ExchangeErrorCategory.Client, null);
			}
		}

		private ConcurrentBag<Type> reportTypesUseExchangeTenantGuid;
	}
}
