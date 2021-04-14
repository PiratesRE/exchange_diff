using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.ExternalActivityByDomain")]
	[DataServiceKey("Date")]
	[Serializable]
	public class ExternalActivityByDomainReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "RecipientDomain")]
		public string RecipientDomain { get; set; }

		[Column(Name = "Count")]
		public long? Count { get; set; }
	}
}
