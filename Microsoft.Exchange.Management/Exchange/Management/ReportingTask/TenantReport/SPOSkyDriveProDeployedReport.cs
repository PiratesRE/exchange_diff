using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.SPOSkyDriveProDeployedWeekly")]
	[DataServiceKey("Date")]
	[Serializable]
	public class SPOSkyDriveProDeployedReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "ID")]
		public long ID { get; set; }

		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "Active")]
		public long? Active { get; set; }

		[Column(Name = "Inactive")]
		public long? Inactive { get; set; }
	}
}
