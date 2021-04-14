using System;
using System.Text;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal sealed class Ia5Coder : CoderBase
	{
		public override CodingScheme CodingScheme
		{
			get
			{
				return CodingScheme.Ia5;
			}
		}

		public override int GetCodedRadixCount(char ch)
		{
			try
			{
				Ia5Coder.encoding.GetByteCount(new char[]
				{
					ch
				});
			}
			catch (EncoderFallbackException)
			{
				return 0;
			}
			int num = Convert.ToInt32(ch);
			if (128 <= num)
			{
				return 0;
			}
			return 1;
		}

		private static Encoding encoding = Encoding.GetEncoding("x-IA5", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
	}
}
