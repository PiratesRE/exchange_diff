using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetSocialNetworksOAuthInfoRequest : BaseJsonRequest
	{
		[IgnoreDataMember]
		public ConnectSubscriptionType ConnectSubscriptionType { get; set; }

		[DataMember(Name = "ConnectSubscriptionType", IsRequired = true, EmitDefaultValue = false)]
		public string ConnectSubscriptionTypeString
		{
			get
			{
				return EnumUtilities.ToString<ConnectSubscriptionType>(this.ConnectSubscriptionType);
			}
			set
			{
				this.ConnectSubscriptionType = EnumUtilities.Parse<ConnectSubscriptionType>(value);
			}
		}

		[DataMember(IsRequired = true)]
		public string RedirectUri { get; set; }

		public override string ToString()
		{
			return string.Format("GetSocialNetworksOAuthInfoRequest: {0}", this.ConnectSubscriptionType);
		}
	}
}
