using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[Table(Name = "dbo.SPOODBUserStatistics")]
	[DataServiceKey("Date")]
	[Serializable]
	public class SPOODBUserStatisticsReport : ScaledReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "UserPuid")]
		public string UserPuid { get; set; }

		[Column(Name = "UserDisplayName")]
		public string UserDisplayName { get; set; }

		[Column(Name = "EmailAddress")]
		public string EmailAddress { get; set; }

		[Column(Name = "FileDownloaded")]
		public int FileDownloaded { get; set; }

		[Column(Name = "FileViewed")]
		public int FileViewed { get; set; }

		[Column(Name = "FileModified")]
		public int FileModified { get; set; }

		[Column(Name = "FileUploaded")]
		public int FileUploaded { get; set; }

		[Column(Name = "FileCheckedOut")]
		public int FileCheckedOut { get; set; }

		[Column(Name = "FileCheckedIn")]
		public int FileCheckedIn { get; set; }

		[Column(Name = "FileCheckOutDiscarded")]
		public int FileCheckOutDiscarded { get; set; }

		[Column(Name = "FileMoved")]
		public int FileMoved { get; set; }

		[Column(Name = "FileCopied")]
		public int FileCopied { get; set; }
	}
}
