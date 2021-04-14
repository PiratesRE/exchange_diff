using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.CsP2PSessionDaily")]
	[DataServiceKey("Date")]
	[Serializable]
	public class CsP2PSessionReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "ID")]
		public long ID { get; set; }

		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "TotalP2PSessions")]
		public long? TotalP2PSessions { get; set; }

		[Column(Name = "P2PIMSessions")]
		public long? P2PIMSessions { get; set; }

		[Column(Name = "P2PAudioSessions")]
		public long? P2PAudioSessions { get; set; }

		[Column(Name = "P2PVideoSessions")]
		public long? P2PVideoSessions { get; set; }

		[Column(Name = "P2PApplicationSharingSessions")]
		public long? P2PApplicationSharingSessions { get; set; }

		[Column(Name = "P2PFileTransferSessions")]
		public long? P2PFileTransferSessions { get; set; }
	}
}
