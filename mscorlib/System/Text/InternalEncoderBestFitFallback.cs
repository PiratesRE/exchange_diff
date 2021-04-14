using System;

namespace System.Text
{
	[Serializable]
	internal class InternalEncoderBestFitFallback : EncoderFallback
	{
		internal InternalEncoderBestFitFallback(Encoding encoding)
		{
			this.encoding = encoding;
			this.bIsMicrosoftBestFitFallback = true;
		}

		public override EncoderFallbackBuffer CreateFallbackBuffer()
		{
			return new InternalEncoderBestFitFallbackBuffer(this);
		}

		public override int MaxCharCount
		{
			get
			{
				return 1;
			}
		}

		public override bool Equals(object value)
		{
			InternalEncoderBestFitFallback internalEncoderBestFitFallback = value as InternalEncoderBestFitFallback;
			return internalEncoderBestFitFallback != null && this.encoding.CodePage == internalEncoderBestFitFallback.encoding.CodePage;
		}

		public override int GetHashCode()
		{
			return this.encoding.CodePage;
		}

		internal Encoding encoding;

		internal char[] arrayBestFit;
	}
}
