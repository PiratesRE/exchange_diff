using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey("Date")]
	[Table(Name = "dbo.PartnerCustomerUser")]
	[Serializable]
	public class PartnerCustomerUserReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "CustomerTenantID")]
		public Guid CustomerTenantID { get; set; }

		[Column(Name = "CompanyName")]
		public string CompanyName { get; set; }

		[Column(Name = "UserPrincipalName")]
		public string UserPrincipalName { get; set; }

		[Column(Name = "DisplayName")]
		public string DisplayName { get; set; }

		[Column(Name = "CreateDate")]
		public DateTime CreateDate { get; set; }

		[Column(Name = "PasswordLastResetDate")]
		public DateTime PasswordLastResetDate { get; set; }

		[Column(Name = "LastAccessDate")]
		public DateTime LastAccessDate { get; set; }

		[Column(Name = "UsageLocation")]
		public string UsageLocation { get; set; }

		[Column(Name = "AssignedLicense")]
		public string AssignedLicense { get; set; }
	}
}
