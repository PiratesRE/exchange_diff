using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.SPOActiveUserDaily")]
	[DataServiceKey("Date")]
	[Serializable]
	public class SPOActiveUserReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "ID")]
		public long ID { get; set; }

		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "UniqueUsers")]
		public long? UniqueUsers { get; set; }

		[Column(Name = "LicensesAssigned")]
		public long? LicensesAssigned { get; set; }

		[Column(Name = "LicensesAcquired")]
		public long? LicensesAcquired { get; set; }

		[Column(Name = "TotalUsers")]
		public long? TotalUsers { get; set; }
	}
}
