using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[CollectionDataContract(Name = "TokenIssuers", ItemName = "TokenIssuer", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class TokenIssuerCollection : Collection<TokenIssuer>
	{
		public TokenIssuerCollection()
		{
		}

		public TokenIssuerCollection(IList<TokenIssuer> tokenIssuers) : base(tokenIssuers)
		{
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.Count * 40);
			foreach (TokenIssuer tokenIssuer in this)
			{
				stringBuilder.AppendLine(tokenIssuer.ToString());
			}
			return stringBuilder.ToString();
		}
	}
}
