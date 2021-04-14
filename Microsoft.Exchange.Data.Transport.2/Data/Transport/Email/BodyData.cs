using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class BodyData
	{
		internal BodyFormat BodyFormat
		{
			[DebuggerStepThrough]
			get
			{
				return this.format;
			}
			[DebuggerStepThrough]
			set
			{
				this.format = value;
			}
		}

		internal InternalBodyFormat InternalBodyFormat
		{
			[DebuggerStepThrough]
			get
			{
				return this.internalFormat;
			}
			[DebuggerStepThrough]
			set
			{
				this.internalFormat = value;
			}
		}

		internal string CharsetName
		{
			get
			{
				if (this.charset == null || this.format == BodyFormat.None)
				{
					return string.Empty;
				}
				return this.charset.Name;
			}
		}

		internal Charset Charset
		{
			[DebuggerStepThrough]
			get
			{
				return this.charset;
			}
			[DebuggerStepThrough]
			set
			{
				this.charset = value;
			}
		}

		internal Encoding Encoding
		{
			get
			{
				Encoding result;
				if (this.charset != null && this.charset.TryGetEncoding(out result))
				{
					return result;
				}
				return Charset.DefaultMimeCharset.GetEncoding();
			}
		}

		internal bool HasContent
		{
			get
			{
				return null != this.storage;
			}
		}

		internal void Dispose()
		{
			if (this.storage != null)
			{
				this.storage.Release();
				this.storage = null;
			}
		}

		internal void SetFormat(BodyFormat format, InternalBodyFormat internalFormat, Charset charset)
		{
			this.format = format;
			this.internalFormat = internalFormat;
			this.charset = charset;
		}

		internal void SetNewCharset(Charset charset)
		{
			this.charset = charset;
		}

		internal void SetStorage(DataStorage storage, long start, long end)
		{
			if (this.storage != null)
			{
				this.storage.Release();
			}
			if (storage != null)
			{
				storage.AddRef();
			}
			this.storage = storage;
			this.start = start;
			this.end = end;
		}

		internal void ValidateCharset(bool charsetWasDefaulted, Stream contentStream)
		{
			bool strongDefault = !charsetWasDefaulted && this.charset.CodePage != 20127;
			FeInboundCharsetDetector feInboundCharsetDetector = new FeInboundCharsetDetector(this.charset.CodePage, strongDefault, true, true, true);
			byte[] byteBuffer = ScratchPad.GetByteBuffer(1024);
			int num = 0;
			int num2 = 0;
			while (num2 < 4096 && (num = contentStream.Read(byteBuffer, 0, byteBuffer.Length)) != 0)
			{
				feInboundCharsetDetector.AddBytes(byteBuffer, 0, num, false);
				num2 += num;
			}
			if (num == 0)
			{
				feInboundCharsetDetector.AddBytes(byteBuffer, 0, 0, true);
			}
			int codePageChoice = feInboundCharsetDetector.GetCodePageChoice();
			if (codePageChoice != this.charset.CodePage)
			{
				this.charset = Charset.GetCharset(codePageChoice);
			}
		}

		internal void GetStorage(InternalBodyFormat format, Charset charset, out DataStorage outStorage, out long outStart, out long outEnd)
		{
			if (format == this.internalFormat && (format == InternalBodyFormat.RtfCompressed || charset == null || charset == this.charset))
			{
				this.storage.AddRef();
				outStorage = this.storage;
				outStart = this.start;
				outEnd = this.end;
				return;
			}
			outStorage = new BodyFormatConversionStorage(this.storage, this.start, this.end, this.internalFormat, this.charset, format, charset);
			outStart = 0L;
			outEnd = long.MaxValue;
		}

		internal void ReleaseStorage()
		{
			this.SetStorage(null, 0L, 0L);
		}

		internal Stream GetReadStream()
		{
			if (this.storage == null)
			{
				return DataStorage.NewEmptyReadStream();
			}
			return this.storage.OpenReadStream(this.start, this.end);
		}

		internal Stream GetWriteStream()
		{
			TemporaryDataStorage temporaryDataStorage = new TemporaryDataStorage();
			this.SetStorage(temporaryDataStorage, 0L, long.MaxValue);
			return temporaryDataStorage.OpenWriteStream(true);
		}

		internal Stream ConvertReadStreamFormat(Stream stream)
		{
			if (this.internalFormat == (InternalBodyFormat)this.format)
			{
				return stream;
			}
			TextConverter converter = null;
			if (this.internalFormat == InternalBodyFormat.RtfCompressed)
			{
				if (!(stream is RtfPreviewStream))
				{
					stream = new ConverterStream(stream, new RtfCompressedToRtf(), ConverterStreamAccess.Read);
				}
				if (BodyFormat.Rtf == this.BodyFormat)
				{
					return stream;
				}
				if (BodyFormat.Html == this.BodyFormat)
				{
					converter = new RtfToHtml
					{
						OutputEncoding = this.Encoding
					};
				}
				else
				{
					converter = new RtfToText
					{
						OutputEncoding = this.Encoding
					};
				}
			}
			else if (this.internalFormat == InternalBodyFormat.Enriched)
			{
				converter = new EnrichedToHtml
				{
					InputEncoding = this.Encoding,
					OutputEncoding = this.Encoding
				};
			}
			return new ConverterStream(stream, converter, ConverterStreamAccess.Read);
		}

		internal Stream ConvertWriteStreamFormat(Stream stream, Charset writeStreamCharset)
		{
			if (this.internalFormat == (InternalBodyFormat)this.format && (writeStreamCharset == null || this.charset == writeStreamCharset))
			{
				return stream;
			}
			Charset charset = writeStreamCharset ?? this.charset;
			TextConverter converter;
			if (this.internalFormat == InternalBodyFormat.RtfCompressed)
			{
				stream = new ConverterStream(stream, new RtfToRtfCompressed(), ConverterStreamAccess.Write);
				if (BodyFormat.Rtf == this.format)
				{
					return stream;
				}
				converter = new TextToRtf
				{
					InputEncoding = charset.GetEncoding()
				};
			}
			else if (this.internalFormat == InternalBodyFormat.Enriched)
			{
				converter = new HtmlToEnriched
				{
					InputEncoding = charset.GetEncoding(),
					OutputEncoding = this.Encoding
				};
			}
			else if (this.internalFormat == InternalBodyFormat.Html)
			{
				converter = new HtmlToHtml
				{
					InputEncoding = charset.GetEncoding(),
					OutputEncoding = this.Encoding
				};
			}
			else
			{
				converter = new TextToText
				{
					InputEncoding = charset.GetEncoding(),
					OutputEncoding = this.Encoding
				};
			}
			return new ConverterStream(stream, converter, ConverterStreamAccess.Write);
		}

		private BodyFormat format;

		private InternalBodyFormat internalFormat;

		private Charset charset;

		private DataStorage storage;

		private long start;

		private long end;
	}
}
