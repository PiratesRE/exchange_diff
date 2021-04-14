using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.ClientSoftwareBrowserDetail")]
	[DataServiceKey("Date")]
	[Serializable]
	public class ClientSoftwareBrowserDetailReport : ReportObject, IDateColumn, ITenantColumn
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

		[Column(Name = "LASTACCESSTIME")]
		public DateTime LastAccessTime { get; set; }

		[Column(Name = "OBJECTID")]
		public Guid ObjectId { get; set; }

		[Column(Name = "UPN")]
		[SuppressPii(PiiDataType = PiiDataType.Smtp)]
		public string UPN { get; set; }

		[Column(Name = "DISPLAYNAME")]
		[SuppressPii(PiiDataType = PiiDataType.String)]
		public string DisplayName { get; set; }
	}
}
