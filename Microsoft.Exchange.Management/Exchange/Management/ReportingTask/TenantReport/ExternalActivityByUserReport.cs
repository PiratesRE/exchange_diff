using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.ExternalActivityByUser")]
	[DataServiceKey("Date")]
	[Serializable]
	public class ExternalActivityByUserReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "SenderAddress")]
		public string SenderAddress { get; set; }

		[Column(Name = "Count")]
		public long? Count { get; set; }

		[Column(Name = "Size")]
		public long? Size { get; set; }
	}
}
