using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.MailboxUsage")]
	[DataServiceKey("Date")]
	[Serializable]
	public class MailboxUsageReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TotalMailboxCount")]
		public long? TotalMailboxCount { get; set; }

		[Column(Name = "TotalInactiveMailboxCount")]
		public long? TotalInactiveMailboxCount { get; set; }

		[Column(Name = "MailboxesOverWarningSize")]
		public long? MailboxesOverWarningSize { get; set; }

		[Column(Name = "MailboxesUsedLessthan25Percent")]
		public long? MailboxesUsedLessthan25Percent { get; set; }
	}
}
