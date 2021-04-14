using System;
using System.Data.Linq.Mapping;
using System.Data.Services.Common;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.ReportingTask.Query;

namespace Microsoft.Exchange.Management.ReportingTask.TenantReport
{
	[DataServiceKey("Date")]
	[Table(Name = "dbo.PartnerClientExpiringSubscription")]
	[Serializable]
	public class PartnerClientExpiringSubscriptionReport : ReportObject, IDateColumn, ITenantColumn
	{
		[Column(Name = "DateTime")]
		public DateTime Date { get; set; }

		[Column(Name = "TenantGuid")]
		public Guid TenantGuid { get; set; }

		[Column(Name = "TenantName")]
		public string TenantName { get; set; }

		[Column(Name = "PartnerId")]
		public string PartnerId { get; set; }

		[Column(Name = "ManagedTenantGuid")]
		public Guid ManagedTenantGuid { get; set; }

		[Column(Name = "ManagedTenantOrganizationName")]
		public string ManagedTenantOrganizationName { get; set; }

		[Column(Name = "OfferId")]
		public Guid OfferId { get; set; }

		[Column(Name = "OfferName")]
		public string OfferName { get; set; }

		[Column(Name = "IsOfferTrial")]
		public bool IsOfferTrial { get; set; }

		[Column(Name = "SubscriptionID")]
		public Guid SubscriptionID { get; set; }

		[Column(Name = "SubscriptionEndDate")]
		public DateTime SubscriptionEndDate { get; set; }

		[Column(Name = "IsAutoRenew")]
		public bool IsAutoRenew { get; set; }

		[Column(Name = "LicenseQuantity")]
		public int? LicenseQuantity { get; set; }
	}
}
