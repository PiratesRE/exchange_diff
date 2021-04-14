using System;
using System.Text;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal sealed class EucKrCoder : CoderBase
	{
		public override CodingScheme CodingScheme
		{
			get
			{
				return CodingScheme.EucKr;
			}
		}

		public override int GetCodedRadixCount(char ch)
		{
			int result;
			try
			{
				result = EucKrCoder.encoding.GetByteCount(new char[]
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

		private static Encoding encoding = Encoding.GetEncoding("euc-kr", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
	}
}
