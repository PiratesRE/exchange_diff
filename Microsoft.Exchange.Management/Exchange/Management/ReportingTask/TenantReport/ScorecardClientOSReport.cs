using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey("Date")]
	[Table(Name = "dbo.ScorecardClientOS")]
	[Serializable]
	public class ScorecardClientOSReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "DATETIME")]
		public DateTime Date { get; set; }

		[Column(Name = "TENANTGUID")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "NAME")]
		public string Name { get; set; }

		[Column(Name = "USERCOUNT")]
		public int UserCount { get; set; }

		[Column(Name = "CATEGORY")]
		public string Category { get; set; }

		[Column(Name = "DISPLAYORDER")]
		public int DisplayOrder { get; set; }
	}
}
