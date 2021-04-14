using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey("Date")]
	[Table(Name = "dbo.StaleMailbox")]
	[Serializable]
	public class StaleMailboxReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "ActiveCount")]
		public long? ActiveMailboxes { get; set; }

		[Column(Name = "StaleTwoMonthCount")]
		public long? InactiveMailboxes31To60Days { get; set; }

		[Column(Name = "StaleThreeMonthCount")]
		public long? InactiveMailboxes61To90Days { get; set; }

		[Column(Name = "StaleOthersCount")]
		public long? InactiveMailboxes91To1460Days { get; set; }
	}
}
