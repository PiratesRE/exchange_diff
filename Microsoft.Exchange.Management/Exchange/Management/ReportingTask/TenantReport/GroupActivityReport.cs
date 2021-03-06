using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey("Date")]
	[Table(Name = "dbo.GroupActivityDaily")]
	[Serializable]
	public class GroupActivityReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "CreatedCount")]
		public long? GroupCreated { get; set; }

		[Column(Name = "DeletedCount")]
		public long? GroupDeleted { get; set; }
	}
}
