using System;
using System.Text;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal sealed class ShiftJisCoder : CoderBase
	{
		public override CodingScheme CodingScheme
		{
			get
			{
				return CodingScheme.ShiftJis;
			}
		}

		public override int GetCodedRadixCount(char ch)
		{
			int result;
			try
			{
				result = ShiftJisCoder.encoding.GetByteCount(new char[]
				{
					ch
				});
			}
			catch (EncoderFallbackException)
			{
				result = 0;
			}
			return result;
		}

		private static Encoding encoding = Encoding.GetEncoding("shift-jis", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
	}
}
