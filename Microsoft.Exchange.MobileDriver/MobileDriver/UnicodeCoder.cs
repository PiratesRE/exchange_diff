using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal sealed class UnicodeCoder : CoderBase
	{
		public override CodingScheme CodingScheme
		{
			get
			{
				return CodingScheme.Unicode;
			}
		}

		public override int GetCodedRadixCount(char ch)
		{
			return 1;
		}
	}
}
