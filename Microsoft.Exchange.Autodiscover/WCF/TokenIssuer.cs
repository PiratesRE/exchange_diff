using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "TokenIssuer", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class TokenIssuer
	{
		public TokenIssuer()
		{
		}

		[DataMember(Name = "Uri", IsRequired = false)]
		public Uri Uri { get; set; }

		[DataMember(Name = "Endpoint", IsRequired = false)]
		public Uri Endpoint { get; set; }

		public TokenIssuer(Uri uri, Uri endpoint)
		{
			this.Uri = uri;
			this.Endpoint = endpoint;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"Uri=",
				this.Uri,
				",Endpoint=",
				this.Endpoint
			});
		}
	}
}
