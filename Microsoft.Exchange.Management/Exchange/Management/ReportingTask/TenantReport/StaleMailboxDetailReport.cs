using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey("Date")]
	[Table(Name = "dbo.StaleMailboxDetail")]
	[Serializable]
	public class StaleMailboxDetailReport : ScaledReportObject, IDateColumn, ITenantColumn
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

		[Column(Name = "UserName")]
		[SuppressPii(PiiDataType = PiiDataType.String)]
		public string UserName { get; set; }

		[Column(Name = "LastLogin")]
		public DateTime? LastLogin { get; set; }

		[Column(Name = "DaysInactive")]
		public int? DaysInactive { get; set; }
	}
}
