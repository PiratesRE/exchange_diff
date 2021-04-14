using System;
using System.Collections.Generic;
using Microsoft.Exchange.TextMessaging.MobileDriver.Resources;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal abstract class CoderBase : ICoder
	{
		public abstract CodingScheme CodingScheme { get; }

		public CodedText Code(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				throw new ArgumentNullException("str");
			}
			if (this.CodingScheme == CodingScheme.Neutral)
			{
				throw new MobileDriverFatalErrorException(Strings.ErrorNeutralCodingScheme);
			}
			List<int> list = new List<int>(str.Length);
			int num = 0;
			while (str.Length > num)
			{
				list.Insert(num, this.GetCodedRadixCount(str[num]));
				num++;
			}
			return new CodedText(this.CodingScheme, str, list.AsReadOnly());
		}

		public abstract int GetCodedRadixCount(char ch);
	}
}
