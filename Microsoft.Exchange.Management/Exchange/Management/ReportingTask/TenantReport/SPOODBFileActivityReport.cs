using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.SPOODBFileActivity")]
	[DataServiceKey("Date")]
	[Serializable]
	public class SPOODBFileActivityReport : ScaledReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "UserPuid")]
		public string UserPuid { get; set; }

		[Column(Name = "DocumentId")]
		public Guid DocumentId { get; set; }

		[Column(Name = "EventName")]
		public string EventName { get; set; }

		[Column(Name = "UserDisplayName")]
		public string UserDisplayName { get; set; }

		[Column(Name = "EmailAddress")]
		public string EmailAddress { get; set; }

		[Column(Name = "IpAddress")]
		public string IpAddress { get; set; }

		[Column(Name = "FileName")]
		public string DocumentName { get; set; }

		[Column(Name = "ParentFolderPath")]
		public string ParentFolderPath { get; set; }

		[Column(Name = "ClientDevice")]
		public string ClientDevice { get; set; }

		[Column(Name = "ClientOs")]
		public string ClientOs { get; set; }

		[Column(Name = "ClientApplication")]
		public string ClientApplication { get; set; }

		[Column(Name = "Details")]
		public string Details { get; set; }
	}
}
