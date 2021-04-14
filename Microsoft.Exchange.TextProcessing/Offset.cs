using System;

namespace Microsoft.Exchange.TextProcessing
{
	public struct Offset
	{
		public Offset(int start, int end)
		{
			this.Start = start;
			this.End = end;
		}

		public int Start;

		public int End;
	}
}
