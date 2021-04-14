using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.CsDeviceSummaryMonthly")]
	[DataServiceKey("Date")]
	[Serializable]
	public class CsClientDeviceReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "ID")]
		public long ID { get; set; }

		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "WindowsCount")]
		public long WindowsUsers { get; set; }

		[Column(Name = "WindowsPhoneCount")]
		public long WindowsPhoneUsers { get; set; }

		[Column(Name = "AndroidCount")]
		public long AndroidUsers { get; set; }

		[Column(Name = "iPhoneCount")]
		public long iPhoneUsers { get; set; }

		[Column(Name = "iPadCount")]
		public long iPadUsers { get; set; }
	}
}
