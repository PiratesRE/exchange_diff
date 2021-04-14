using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetSocialNetworksOAuthInfoResponse : OptionsResponseBase
	{
		public GetSocialNetworksOAuthInfoResponse()
		{
			this.SocialNetworksOAuthInfo = new SocialNetworksOAuthInfo();
		}

		[DataMember(IsRequired = true)]
		public SocialNetworksOAuthInfo SocialNetworksOAuthInfo { get; set; }

		public override string ToString()
		{
			return string.Format("GetSocialNetworksOAuthInfoResponse: {0}", this.SocialNetworksOAuthInfo);
		}
	}
}
