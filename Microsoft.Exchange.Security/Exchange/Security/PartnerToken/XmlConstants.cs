using System;

namespace Microsoft.Exchange.Security.PartnerToken
{
	internal static class XmlConstants
	{
		public const string XmlNamespace = "http://schemas.microsoft.com/exchange/services/2006/partnertoken";

		public const string LinkedPartnerOrganizationIdClaimType = "http://schemas.microsoft.com/exchange/services/2006/partnertoken/linkedpartnerorganizationid";

		public const string LinkedPartnerGroupId = "linkedpartnergroupid";

		public const string LinkedPartnerGroupIdClaimType = "http://schemas.microsoft.com/exchange/services/2006/partnertoken/linkedpartnergroupid";

		public const string TargetTenantClaimType = "http://schemas.microsoft.com/exchange/services/2006/partnertoken/targettenant";

		public const string Issuer = "http://schemas.microsoft.com/exchange/2010/autodiscover/getusersettings";
	}
}
