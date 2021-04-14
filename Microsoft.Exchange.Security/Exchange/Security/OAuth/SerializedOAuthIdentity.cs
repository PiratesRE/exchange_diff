using System;

namespace Microsoft.Exchange.Security.OAuth
{
	[Serializable]
	internal sealed class SerializedOAuthIdentity
	{
		public string OrganizationName { get; set; }

		public string PartnerApplicationDn { get; set; }

		public string PartnerApplicationAppId { get; set; }

		public string PartnerApplicationRealm { get; set; }

		public string UserDn { get; set; }

		public string OfficeExtensionId { get; set; }

		public string V1ProfileAppId { get; set; }

		public string Scope { get; set; }

		public string OrganizationIdBase64 { get; set; }

		public string IsFromSameOrgExchange { get; set; }
	}
}
