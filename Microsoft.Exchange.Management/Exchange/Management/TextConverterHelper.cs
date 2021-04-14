using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.TextConverters.Internal;

namespace Microsoft.Exchange.Management
{
	internal class TextConverterHelper
	{
		public static string SanitizeHtml(string unsafeHtml)
		{
			if (string.IsNullOrEmpty(unsafeHtml))
			{
				return unsafeHtml;
			}
			string result;
			using (StringReader stringReader = new StringReader(unsafeHtml))
			{
				using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
				{
					HtmlToHtml htmlToHtml = new HtmlToHtml();
					TextConvertersInternalHelpers.SetPreserveDisplayNoneStyle(htmlToHtml, true);
					htmlToHtml.InputEncoding = Encoding.UTF8;
					htmlToHtml.OutputEncoding = Encoding.UTF8;
					htmlToHtml.FilterHtml = true;
					htmlToHtml.Convert(stringReader, stringWriter);
					result = stringWriter.ToString();
				}
			}
			return result;
		}

		public static string HtmlToText(string html, bool shouldUseNarrowGapForPTagHtmlToTextConversion)
		{
			if (string.IsNullOrEmpty(html))
			{
				return html;
			}
			html = TextConverterHelper.RemoveHtmlLink(html);
			string result;
			using (StringReader stringReader = new StringReader(html))
			{
				using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
				{
					HtmlToText htmlToText = new HtmlToText();
					htmlToText.InputEncoding = Encoding.UTF8;
					htmlToText.OutputEncoding = Encoding.UTF8;
					htmlToText.ShouldUseNarrowGapForPTagHtmlToTextConversion = shouldUseNarrowGapForPTagHtmlToTextConversion;
					TextConvertersInternalHelpers.SetImageRenderingCallback(htmlToText, new ImageRenderingCallback(TextConverterHelper.RemoveImageCallback));
					htmlToText.Convert(stringReader, stringWriter);
					result = stringWriter.ToString();
				}
			}
			return result;
		}

		public static string TextToHtml(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			string result;
			using (StringReader stringReader = new StringReader(text))
			{
				using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
				{
					new TextToHtml
					{
						InputEncoding = Encoding.UTF8,
						OutputEncoding = Encoding.UTF8,
						HtmlTagCallback = new HtmlTagCallback(TextConverterHelper.RemoveLinkCallback),
						OutputHtmlFragment = true
					}.Convert(stringReader, stringWriter);
					result = stringWriter.ToString();
				}
			}
			return result;
		}

		internal static string RemoveHtmlLink(string html)
		{
			string result;
			using (StringReader stringReader = new StringReader(html))
			{
				using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
				{
					new HtmlToHtml
					{
						InputEncoding = Encoding.UTF8,
						OutputEncoding = Encoding.UTF8,
						HtmlTagCallback = new HtmlTagCallback(TextConverterHelper.RemoveLinkCallback)
					}.Convert(stringReader, stringWriter);
					result = stringWriter.ToString();
				}
			}
			return result;
		}

		internal static void RemoveLinkCallback(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			if (tagContext.TagId == HtmlTagId.A)
			{
				tagContext.DeleteTag();
				return;
			}
			tagContext.WriteTag();
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				htmlTagContextAttribute.Write();
			}
		}

		internal static bool RemoveImageCallback(string imageUrl, int approximateRenderingPosition)
		{
			return true;
		}
	}
}
