using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.CsConferenceDaily")]
	[DataServiceKey("Date")]
	[Serializable]
	public class CsConferenceReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "ID")]
		public long ID { get; set; }

		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "TotalConferences")]
		public long? TotalConferences { get; set; }

		[Column(Name = "AVConferences")]
		public long? AVConferences { get; set; }

		[Column(Name = "IMConferences")]
		public long? IMConferences { get; set; }

		[Column(Name = "ApplicationSharingConferences")]
		public long? ApplicationSharingConferences { get; set; }

		[Column(Name = "WebConferences")]
		public long? WebConferences { get; set; }

		[Column(Name = "TelephonyConferences")]
		public long? TelephonyConferences { get; set; }
	}
}
