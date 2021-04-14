using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey(new string[]
	{
		"TenantGuid",
		"Workload"
	})]
	[Table(Name = "dbo.LicenseVsUsageSummary")]
	[Serializable]
	public class LicenseVsUsageSummaryReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "Workload")]
		public string Workload { get; set; }

		[Column(Name = "NonTrialEntitlements")]
		public long NonTrialEntitlements { get; set; }

		[Column(Name = "TrialEntitlements")]
		public long TrialEntitlements { get; set; }

		[Column(Name = "ActiveUsers")]
		public long ActiveUsers { get; set; }
	}
}
