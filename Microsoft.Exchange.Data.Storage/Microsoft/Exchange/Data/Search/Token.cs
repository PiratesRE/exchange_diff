using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class Token
	{
		public int Start
		{
			get
			{
				return this.start;
			}
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public Token(int start, int length)
		{
			this.start = start;
			this.length = length;
		}

		private int start;

		private int length;
	}
}
