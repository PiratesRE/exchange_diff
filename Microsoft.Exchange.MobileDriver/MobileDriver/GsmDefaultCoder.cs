using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal sealed class GsmDefaultCoder : CoderBase
	{
		public override CodingScheme CodingScheme
		{
			get
			{
				return CodingScheme.GsmDefault;
			}
		}

		public override int GetCodedRadixCount(char ch)
		{
			return UnicodeToGsmMap.GetUnicodeToGsmRadixCount(ch, true);
		}
	}
}
