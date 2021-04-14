using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.ScorecardMetrics")]
	[DataServiceKey("Date")]
	[Serializable]
	public class ScorecardMetricsReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "DATETIME")]
		public DateTime Date { get; set; }

		[Column(Name = "TENANTGUID")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "SERVICE")]
		public string Service { get; set; }

		[Column(Name = "TYPE")]
		public string Type { get; set; }

		[Column(Name = "METRICNAME")]
		public string MetricName { get; set; }

		[Column(Name = "THRESHOLD")]
		public double? Threshold { get; set; }

		[Column(Name = "METRICVALUE")]
		public double MetricValue { get; set; }
	}
}
