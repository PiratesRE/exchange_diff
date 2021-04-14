using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal interface ICoder
	{
		CodingScheme CodingScheme { get; }

		CodedText Code(string str);

		int GetCodedRadixCount(char ch);
	}
}
