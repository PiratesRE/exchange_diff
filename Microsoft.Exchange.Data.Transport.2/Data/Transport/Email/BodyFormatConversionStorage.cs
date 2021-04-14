using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class BodyFormatConversionStorage : DataStorage
	{
		public BodyFormatConversionStorage(DataStorage originalStorage, long originalStart, long originalEnd, InternalBodyFormat originalFormat, Charset originalCharset, InternalBodyFormat targetFormat, Charset targetCharset)
		{
			originalStorage.AddRef();
			this.originalStorage = originalStorage;
			this.originalStart = originalStart;
			this.originalEnd = originalEnd;
			this.originalFormat = originalFormat;
			this.originalCharset = originalCharset;
			this.targetFormat = targetFormat;
			this.targetCharset = targetCharset;
		}

		public override Stream OpenReadStream(long start, long end)
		{
			base.ThrowIfDisposed();
			BodyFormatConversionStorage.BodyFormatConversion conversion = BodyFormatConversionStorage.GetConversion(this.originalFormat, this.targetFormat);
			TextConverter converter = null;
			Encoding inputEncoding = null;
			if (this.originalCharset != null)
			{
				inputEncoding = this.originalCharset.GetEncoding();
			}
			Encoding outputEncoding = null;
			if (this.targetCharset != null)
			{
				outputEncoding = this.targetCharset.GetEncoding();
			}
			Stream stream = this.originalStorage.OpenReadStream(this.originalStart, this.originalEnd);
			switch (conversion)
			{
			case BodyFormatConversionStorage.BodyFormatConversion.HtmlToText:
				converter = new HtmlToText
				{
					InputEncoding = inputEncoding,
					OutputEncoding = outputEncoding
				};
				break;
			case BodyFormatConversionStorage.BodyFormatConversion.HtmlToEnriched:
				converter = new HtmlToEnriched
				{
					InputEncoding = inputEncoding,
					OutputEncoding = outputEncoding
				};
				break;
			case BodyFormatConversionStorage.BodyFormatConversion.RtfCompressedToText:
				stream = new ConverterStream(stream, new RtfCompressedToRtf(), ConverterStreamAccess.Read);
				converter = new RtfToText
				{
					OutputEncoding = outputEncoding
				};
				break;
			case BodyFormatConversionStorage.BodyFormatConversion.TextToText:
				converter = new TextToText
				{
					InputEncoding = inputEncoding,
					OutputEncoding = outputEncoding
				};
				break;
			case BodyFormatConversionStorage.BodyFormatConversion.HtmlToHtml:
				converter = new HtmlToHtml
				{
					InputEncoding = inputEncoding,
					OutputEncoding = outputEncoding
				};
				break;
			case BodyFormatConversionStorage.BodyFormatConversion.EnrichedToText:
				converter = new EnrichedToText
				{
					InputEncoding = inputEncoding,
					OutputEncoding = outputEncoding
				};
				break;
			}
			return new ConverterStream(stream, converter, ConverterStreamAccess.Read);
		}

		protected override void Dispose(bool disposing)
		{
			if (!base.IsDisposed)
			{
				if (disposing && this.originalStorage != null)
				{
					this.originalStorage.Release();
					this.originalStorage = null;
				}
				this.originalCharset = null;
				this.targetCharset = null;
			}
			base.Dispose(disposing);
		}

		private static BodyFormatConversionStorage.BodyFormatConversion GetConversion(InternalBodyFormat sourceFormat, InternalBodyFormat targetFormat)
		{
			if (sourceFormat == InternalBodyFormat.Html && targetFormat == InternalBodyFormat.Text)
			{
				return BodyFormatConversionStorage.BodyFormatConversion.HtmlToText;
			}
			if (sourceFormat == InternalBodyFormat.Html && targetFormat == InternalBodyFormat.Enriched)
			{
				return BodyFormatConversionStorage.BodyFormatConversion.HtmlToEnriched;
			}
			if (sourceFormat == InternalBodyFormat.Enriched && targetFormat == InternalBodyFormat.Text)
			{
				return BodyFormatConversionStorage.BodyFormatConversion.EnrichedToText;
			}
			if (sourceFormat == InternalBodyFormat.RtfCompressed && targetFormat == InternalBodyFormat.Text)
			{
				return BodyFormatConversionStorage.BodyFormatConversion.RtfCompressedToText;
			}
			if (sourceFormat == InternalBodyFormat.Text && targetFormat == InternalBodyFormat.Text)
			{
				return BodyFormatConversionStorage.BodyFormatConversion.TextToText;
			}
			if (sourceFormat == InternalBodyFormat.Html && targetFormat == InternalBodyFormat.Html)
			{
				return BodyFormatConversionStorage.BodyFormatConversion.HtmlToHtml;
			}
			return BodyFormatConversionStorage.BodyFormatConversion.None;
		}

		private DataStorage originalStorage;

		private long originalStart;

		private long originalEnd;

		private InternalBodyFormat originalFormat;

		private Charset originalCharset;

		private InternalBodyFormat targetFormat;

		private Charset targetCharset;

		internal enum BodyFormatConversion
		{
			None,
			HtmlToText,
			HtmlToEnriched,
			RtfCompressedToText,
			TextToText,
			HtmlToHtml,
			EnrichedToText
		}
	}
}
