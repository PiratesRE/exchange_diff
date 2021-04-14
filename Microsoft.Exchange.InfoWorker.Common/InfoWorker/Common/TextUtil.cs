using System;
using System.IO;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal sealed class TextUtil
	{
		public static string ConvertHtmlToPlainText(string message)
		{
			StringReader stringReader = null;
			StringWriter stringWriter = null;
			string result;
			try
			{
				if (message == null)
				{
					result = null;
				}
				else
				{
					int codePage = 65001;
					stringReader = new StringReader(message);
					stringWriter = new StringWriter();
					new HtmlToText
					{
						InputEncoding = Charset.GetEncoding(codePage),
						OutputEncoding = Charset.GetEncoding(codePage)
					}.Convert(stringReader, stringWriter);
					result = stringWriter.ToString();
				}
			}
			finally
			{
				if (stringReader != null)
				{
					stringReader.Dispose();
				}
				if (stringWriter != null)
				{
					stringWriter.Dispose();
				}
			}
			return result;
		}

		public static string ConvertPlainTextToHtml(string message)
		{
			StringReader stringReader = null;
			StringWriter stringWriter = null;
			string result;
			try
			{
				if (message == null)
				{
					result = null;
				}
				else
				{
					int codePage = 65001;
					stringReader = new StringReader(message);
					stringWriter = new StringWriter();
					new TextToHtml
					{
						InputEncoding = Charset.GetEncoding(codePage),
						OutputEncoding = Charset.GetEncoding(codePage)
					}.Convert(stringReader, stringWriter);
					result = stringWriter.ToString();
				}
			}
			finally
			{
				if (stringReader != null)
				{
					stringReader.Dispose();
				}
				if (stringWriter != null)
				{
					stringWriter.Dispose();
				}
			}
			return result;
		}
	}
}
