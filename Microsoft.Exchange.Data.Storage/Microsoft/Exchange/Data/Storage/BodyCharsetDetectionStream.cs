using System;
using System.IO;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BodyCharsetDetectionStream : StreamBase
	{
		public BodyCharsetDetectionStream(Stream outputStream, BodyCharsetDetectionStream.DetectCharsetCallback callback, ICoreItem coreItem, BodyStreamFormat format, Charset contentCharset, Charset userCharset, BodyCharsetFlags charsetFlags, string extraData, bool trustHtmlMetaTag) : base(StreamBase.Capabilities.Writable)
		{
			this.outputStream = outputStream;
			this.callback = callback;
			this.coreItem = coreItem;
			this.userCharset = userCharset;
			this.charsetFlags = charsetFlags;
			Charset charset;
			this.isDetectingCharset = !this.coreItem.CharsetDetector.IsItemCharsetKnownWithoutDetection(charsetFlags, userCharset, out charset);
			if (this.isDetectingCharset)
			{
				this.CreateDetectionStream(format, contentCharset, extraData, trustHtmlMetaTag);
				return;
			}
			this.CalculateCharset();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.CloseDetectorConversionStream();
					if (this.outputStream != null)
					{
						this.outputStream.Dispose();
						this.outputStream = null;
					}
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<BodyCharsetDetectionStream>(this);
		}

		private void CreateDetectionStream(BodyStreamFormat format, Charset contentCharset, string extraData, bool trustHtmlMetaTag)
		{
			this.detectorCache = new PooledMemoryStream(8192);
			if (extraData != null)
			{
				byte[] bytes = ConvertUtils.UnicodeEncoding.GetBytes(extraData);
				this.detectorCache.Write(bytes, 0, bytes.Length);
			}
			switch (format)
			{
			case BodyStreamFormat.Text:
			{
				if (contentCharset.CodePage == 1200)
				{
					this.detectorConversionStream = new StreamWrapper(this.detectorCache, false);
					return;
				}
				TextToText textToText = new TextToText(TextToTextConversionMode.ConvertCodePageOnly);
				textToText.InputEncoding = contentCharset.GetEncoding();
				textToText.OutputEncoding = ConvertUtils.UnicodeEncoding;
				textToText.OutputStreamBufferSize = 1024;
				textToText.InputStreamBufferSize = 1024;
				this.detectorConversionStream = new ConverterStream(new StreamWrapper(this.detectorCache, false), textToText, ConverterStreamAccess.Write);
				return;
			}
			case BodyStreamFormat.Html:
			{
				HtmlToText htmlToText = new HtmlToText(TextExtractionMode.ExtractText);
				htmlToText.InputEncoding = contentCharset.GetEncoding();
				htmlToText.OutputEncoding = ConvertUtils.UnicodeEncoding;
				htmlToText.DetectEncodingFromMetaTag = trustHtmlMetaTag;
				htmlToText.OutputStreamBufferSize = 1024;
				htmlToText.InputStreamBufferSize = 1024;
				this.detectorConversionStream = new ConverterStream(new StreamWrapper(this.detectorCache, false), htmlToText, ConverterStreamAccess.Write);
				return;
			}
			case BodyStreamFormat.RtfCompressed:
			case BodyStreamFormat.RtfUncompressed:
			{
				RtfToText rtfToText = new RtfToText(TextExtractionMode.ExtractText);
				rtfToText.OutputEncoding = ConvertUtils.UnicodeEncoding;
				rtfToText.OutputStreamBufferSize = 1024;
				rtfToText.InputStreamBufferSize = 1024;
				this.detectorConversionStream = new ConverterStream(new StreamWrapper(this.detectorCache, false), rtfToText, ConverterStreamAccess.Write);
				if (format == BodyStreamFormat.RtfCompressed)
				{
					RtfCompressedToRtf rtfCompressedToRtf = new RtfCompressedToRtf();
					rtfCompressedToRtf.OutputStreamBufferSize = 1024;
					rtfCompressedToRtf.InputStreamBufferSize = 1024;
					this.detectorConversionStream = new ConverterStream(this.detectorConversionStream, rtfCompressedToRtf, ConverterStreamAccess.Write);
				}
				return;
			}
			default:
				return;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.isDetectingCharset)
			{
				this.detectorConversionStream.Write(buffer, offset, count);
				if (this.detectorCache.Length >= 32768L)
				{
					this.OnBufferFull();
				}
			}
			if (this.outputStream != null)
			{
				this.outputStream.Write(buffer, offset, count);
			}
		}

		public override void Flush()
		{
			this.CloseDetectorConversionStream();
			if (this.outputStream != null)
			{
				this.outputStream.Flush();
			}
		}

		private void CloseDetectorConversionStream()
		{
			if (this.isDetectingCharset)
			{
				this.OnBufferFull();
			}
			if (this.detectorConversionStream != null)
			{
				try
				{
					this.detectorConversionStream.Dispose();
					this.detectorConversionStream = null;
				}
				catch (TextConvertersException)
				{
				}
			}
			if (this.detectorCache != null)
			{
				this.detectorCache.Dispose();
				this.detectorCache = null;
			}
		}

		private void CalculateCharset()
		{
			char[] cachedBodyData = null;
			if (this.isDetectingCharset)
			{
				this.detectorCache.Position = 0L;
				cachedBodyData = ConvertUtils.UnicodeEncoding.GetChars(this.detectorCache.GetBuffer(), 0, (int)this.detectorCache.Length);
				this.isDetectingCharset = false;
			}
			Charset detectedCharset = this.coreItem.CharsetDetector.SetCachedBodyDataAndDetectCharset(cachedBodyData, this.userCharset, this.charsetFlags);
			if (this.callback != null)
			{
				this.callback(detectedCharset);
			}
		}

		private void OnBufferFull()
		{
			if (!this.isDetectingCharset)
			{
				return;
			}
			this.CalculateCharset();
			this.CloseDetectorConversionStream();
		}

		internal const int DetectionDataSize = 32768;

		private const int InitialDetectionBufferSize = 8192;

		private ICoreItem coreItem;

		private Stream detectorConversionStream;

		private PooledMemoryStream detectorCache;

		private Stream outputStream;

		private Charset userCharset;

		private BodyCharsetFlags charsetFlags;

		private BodyCharsetDetectionStream.DetectCharsetCallback callback;

		private bool isDetectingCharset;

		public delegate void DetectCharsetCallback(Charset detectedCharset);
	}
}
