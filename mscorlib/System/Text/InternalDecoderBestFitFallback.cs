using System;

namespace System.Text
{
	[Serializable]
	internal sealed class InternalDecoderBestFitFallback : DecoderFallback
	{
		internal InternalDecoderBestFitFallback(Encoding encoding)
		{
			this.encoding = encoding;
			this.bIsMicrosoftBestFitFallback = true;
		}

		public override DecoderFallbackBuffer CreateFallbackBuffer()
		{
			return new InternalDecoderBestFitFallbackBuffer(this);
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
			InternalDecoderBestFitFallback internalDecoderBestFitFallback = value as InternalDecoderBestFitFallback;
			return internalDecoderBestFitFallback != null && this.encoding.CodePage == internalDecoderBestFitFallback.encoding.CodePage;
		}

		public override int GetHashCode()
		{
			return this.encoding.CodePage;
		}

		internal Encoding encoding;

		internal char[] arrayBestFit;

		internal char cReplacement = '?';
	}
}
