using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey("Date")]
	[Table(Name = "dbo.SPOTeamSiteStorageWeekly")]
	[Serializable]
	public class SPOTeamSiteStorageReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "ID")]
		public long ID { get; set; }

		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "Used")]
		public long? Used { get; set; }

		[Column(Name = "Allocated")]
		public long? Allocated { get; set; }
	}
}
