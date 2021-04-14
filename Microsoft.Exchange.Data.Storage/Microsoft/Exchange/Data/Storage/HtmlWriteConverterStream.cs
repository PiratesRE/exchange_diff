using System;
using System.IO;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class HtmlWriteConverterStream : StreamBase
	{
		internal HtmlWriteConverterStream(Stream outputStream, TextToHtml textToHtml) : this(outputStream, textToHtml, false)
		{
		}

		internal HtmlWriteConverterStream(Stream outputStream, RtfToHtml rtfToHtml) : this(outputStream, rtfToHtml, false)
		{
		}

		internal HtmlWriteConverterStream(Stream outputStream, HtmlToHtml toHtmlConverter, bool canSkipConversionOnMatchingCharset) : this(outputStream, toHtmlConverter, canSkipConversionOnMatchingCharset)
		{
		}

		private HtmlWriteConverterStream(Stream outputStream, TextConverter toHtmlConverter, bool canSkipConversionOnMatchingCharset) : base(StreamBase.Capabilities.Writable)
		{
			this.outputStream = outputStream;
			this.toHtmlConverter = toHtmlConverter;
			this.cachedContentStream = new MemoryStream();
			this.skipConversionOnMatchingCharset = canSkipConversionOnMatchingCharset;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<HtmlWriteConverterStream>(this);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.cachedContentStream != null)
			{
				this.cachedContentStream.Write(buffer, offset, count);
				return;
			}
			this.writeStream.Write(buffer, offset, count);
		}

		public void SetCharset(Charset detectedCharset)
		{
			if (this.toHtmlConverter == null)
			{
				throw new InvalidOperationException("HtmlWriteConverterStream.SetCharset() should only be called once.");
			}
			HtmlToHtml htmlToHtml = this.toHtmlConverter as HtmlToHtml;
			if (htmlToHtml != null)
			{
				if (CodePageMap.GetCodePage(htmlToHtml.InputEncoding) == detectedCharset.CodePage && this.skipConversionOnMatchingCharset)
				{
					this.writeStream = this.outputStream;
				}
				else
				{
					htmlToHtml.OutputEncoding = detectedCharset.GetEncoding();
					this.writeStream = new ConverterStream(this.outputStream, htmlToHtml, ConverterStreamAccess.Write);
				}
			}
			else
			{
				RtfToHtml rtfToHtml = this.toHtmlConverter as RtfToHtml;
				if (rtfToHtml != null)
				{
					rtfToHtml.OutputEncoding = detectedCharset.GetEncoding();
				}
				else
				{
					TextToHtml textToHtml = this.toHtmlConverter as TextToHtml;
					if (textToHtml != null)
					{
						textToHtml.OutputEncoding = detectedCharset.GetEncoding();
					}
				}
				this.writeStream = new ConverterStream(this.outputStream, this.toHtmlConverter, ConverterStreamAccess.Write);
			}
			this.toHtmlConverter = null;
			this.cachedContentStream.Position = 0L;
			Util.StreamHandler.CopyStreamData(this.cachedContentStream, this.writeStream);
			this.cachedContentStream.Dispose();
			this.cachedContentStream = null;
		}

		public override void Flush()
		{
			this.writeStream.Flush();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.writeStream != null)
				{
					this.writeStream.Dispose();
					this.writeStream = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private Stream outputStream;

		private Stream writeStream;

		private MemoryStream cachedContentStream;

		private TextConverter toHtmlConverter;

		private bool skipConversionOnMatchingCharset;
	}
}
