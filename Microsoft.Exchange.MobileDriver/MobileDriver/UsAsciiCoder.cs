using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal sealed class UsAsciiCoder : CoderBase
	{
		public override CodingScheme CodingScheme
		{
			get
			{
				return CodingScheme.UsAscii;
			}
		}

		public override int GetCodedRadixCount(char ch)
		{
			int num = Convert.ToInt32(ch);
			if (128 <= num)
			{
				return 0;
			}
			return 1;
		}
	}
}
