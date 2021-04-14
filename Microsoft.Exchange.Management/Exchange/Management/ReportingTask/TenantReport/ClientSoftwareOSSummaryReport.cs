using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey("Date")]
	[Table(Name = "dbo.ClientSoftwareOSSummary")]
	[Serializable]
	public class ClientSoftwareOSSummaryReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "TENANTGUID")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TENANTNAME")]
		public string TenantName { get; set; }

		[Column(Name = "DATETIME")]
		public DateTime Date { get; set; }

		[Column(Name = "NAME")]
		public string Name { get; set; }

		[Column(Name = "VERSION")]
		public string Version { get; set; }

		[Column(Name = "COUNT")]
		public long Count { get; set; }

		[Column(Name = "CATEGORY")]
		public string Category { get; set; }

		[Column(Name = "DISPLAYORDER")]
		public int DisplayOrder { get; set; }
	}
}
