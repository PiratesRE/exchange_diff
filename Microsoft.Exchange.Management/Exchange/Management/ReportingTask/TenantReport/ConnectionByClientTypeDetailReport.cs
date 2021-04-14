using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey("Date")]
	[Table(Name = "dbo.ConnectionByClientTypeDetail")]
	[Serializable]
	public class ConnectionByClientTypeDetailReport : ReportObject, IDateColumn, ITenantColumn
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

		[SuppressPii(PiiDataType = PiiDataType.String)]
		[Column(Name = "UserName")]
		public string UserName { get; set; }

		[Column(Name = "ClientType")]
		public string ClientType { get; set; }

		[Column(Name = "Count")]
		public long? Count { get; set; }
	}
}
