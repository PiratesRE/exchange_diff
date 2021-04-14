using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.MailboxUsageDetail")]
	[DataServiceKey("Date")]
	[Serializable]
	public class MailboxUsageDetailReport : ScaledReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[SuppressPii(PiiDataType = PiiDataType.Smtp)]
		[Column(Name = "WindowsLiveID")]
		public string WindowsLiveID { get; set; }

		[SuppressPii(PiiDataType = PiiDataType.String)]
		[Column(Name = "UserName")]
		public string UserName { get; set; }

		[Column(Name = "MailboxSize")]
		public long? MailboxSize { get; set; }

		[Column(Name = "CurrentMailboxSize")]
		public long? CurrentMailboxSize { get; set; }

		[Column(Name = "PercentUsed")]
		public long? PercentUsed { get; set; }

		[Column(Name = "MailboxPlan")]
		public string MailboxPlan { get; set; }

		[Column(Name = "IsInactive")]
		public bool IsInactive { get; set; }

		[Column(Name = "IssueWarningQuota")]
		public long? IssueWarningQuota { get; set; }

		[Column(Name = "IsOverWarningQuota")]
		public bool IsOverWarningQuota { get; set; }
	}
}
