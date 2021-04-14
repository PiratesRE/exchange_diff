using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey("Date")]
	[Table(Name = "dbo.MailboxActivityDaily")]
	[Serializable]
	public class MailboxActivityReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TotalActiveCount")]
		public long? TotalNumberOfActiveMailboxes { get; set; }

		[Column(Name = "CreatedCount")]
		public long? AccountCreated { get; set; }

		[Column(Name = "DeletedCount")]
		public long? AccountDeleted { get; set; }
	}
}
