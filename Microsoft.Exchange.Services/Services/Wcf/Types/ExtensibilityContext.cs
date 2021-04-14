using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class ExtensibilityContext
	{
		public ExtensibilityContext(Extension[] extensions, string marketplaceUrl, string ewsUrl, bool isInOrgMarketplaceRole)
		{
			this.Extensions = extensions;
			this.MarketplaceUrl = marketplaceUrl;
			this.EwsUrl = ewsUrl;
			this.IsInOrgMarketplaceRole = isInOrgMarketplaceRole;
		}

		[DataMember]
		public Extension[] Extensions { get; set; }

		[DataMember]
		public string MarketplaceUrl { get; set; }

		[DataMember]
		public string EwsUrl { get; set; }

		[DataMember]
		public bool IsInOrgMarketplaceRole { get; set; }
	}
}
