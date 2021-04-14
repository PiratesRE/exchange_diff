using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal
{
	internal sealed class TextConvertersInternalHelpers
	{
		public static void SetImageRenderingCallback(HtmlToText conversion, ImageRenderingCallback callback)
		{
			conversion.SetImageRenderingCallback(new ImageRenderingCallbackInternal(callback.Invoke));
		}

		public static void SetImageRenderingCallback(HtmlToRtf conversion, ImageRenderingCallback callback)
		{
			conversion.SetImageRenderingCallback(new ImageRenderingCallbackInternal(callback.Invoke));
		}

		public static void SetSmallCssBlockThreshold(HtmlToHtml conversion, int threshold)
		{
			conversion.SetSmallCssBlockThreshold(threshold);
		}

		public static void SetSmallCssBlockThreshold(RtfToHtml conversion, int threshold)
		{
			conversion.SetSmallCssBlockThreshold(threshold);
		}

		public static void SetPreserveDisplayNoneStyle(HtmlToHtml conversion, bool preserve)
		{
			conversion.SetPreserveDisplayNoneStyle(preserve);
		}

		public static void SetPreserveDisplayNoneStyle(RtfToHtml conversion, bool preserve)
		{
			conversion.SetPreserveDisplayNoneStyle(preserve);
		}

		public static Stream CreateRtfPreviewStream(Stream input, int inputBufferLength)
		{
			return new RtfPreviewStream(input, inputBufferLength);
		}

		public static bool RtfHasEncapsulatedHtml(Stream previewStream)
		{
			RtfPreviewStream rtfPreviewStream = previewStream as RtfPreviewStream;
			if (rtfPreviewStream == null)
			{
				throw new ArgumentException("previewStream must be created with CreateRtfPreviewStream");
			}
			return rtfPreviewStream.Encapsulation == RtfEncapsulation.Html;
		}

		public static bool RtfHasEncapsulatedText(Stream previewStream)
		{
			RtfPreviewStream rtfPreviewStream = previewStream as RtfPreviewStream;
			if (rtfPreviewStream == null)
			{
				throw new ArgumentException("previewStream must be created with CreateRtfPreviewStream");
			}
			return rtfPreviewStream.Encapsulation == RtfEncapsulation.Text;
		}

		public static bool IsUrlSchemaSafe(string schema)
		{
			return HtmlToHtmlConverter.TestSafeUrlSchema(schema);
		}

		public static void ReuseConverter(ConverterStream converter, object newSourceOrSink)
		{
			converter.Reuse(newSourceOrSink);
		}

		public static void ReuseConverter(ConverterReader converter, object newSource)
		{
			converter.Reuse(newSource);
		}

		public static void ReuseConverter(ConverterWriter converter, object newSink)
		{
			converter.Reuse(newSink);
		}
	}
}
