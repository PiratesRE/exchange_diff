using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Text
{
	internal class TextToken : Token
	{
		public TextTokenId TextTokenId
		{
			get
			{
				return (TextTokenId)base.TokenId;
			}
		}
	}
}
