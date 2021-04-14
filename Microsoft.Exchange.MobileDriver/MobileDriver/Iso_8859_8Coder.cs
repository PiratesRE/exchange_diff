using System;
using System.Text;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal sealed class Iso_8859_8Coder : CoderBase
	{
		public override CodingScheme CodingScheme
		{
			get
			{
				return CodingScheme.Iso_8859_8;
			}
		}

		public override int GetCodedRadixCount(char ch)
		{
			int result;
			try
			{
				result = Iso_8859_8Coder.encoding.GetByteCount(new char[]
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

		private static Encoding encoding = Encoding.GetEncoding("iso-8859-8", EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
	}
}
