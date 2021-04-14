using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.StructuredQuery;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TokenInfo
	{
		internal int FirstChar { get; set; }

		internal int Length { get; set; }

		internal bool IsValid
		{
			get
			{
				return this.FirstChar != -1;
			}
		}

		internal TokenInfo() : this(-1, 0)
		{
		}

		internal TokenInfo(int firstChar, int length)
		{
			this.FirstChar = firstChar;
			this.Length = length;
		}

		internal TokenInfo(TokenInfo tokenInfo) : this(tokenInfo.FirstChar, tokenInfo.Length)
		{
		}
	}
}
