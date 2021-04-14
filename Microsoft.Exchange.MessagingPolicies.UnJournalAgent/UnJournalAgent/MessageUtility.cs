using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal static class MessageUtility
	{
		public static Stream GetMimePartReadStream(MimePart mimePart)
		{
			if (mimePart == null)
			{
				throw new ArgumentNullException("mimePart");
			}
			Stream stream = null;
			try
			{
				if (mimePart.TryGetContentReadStream(out stream))
				{
					if (stream.Length >= 0L)
					{
					}
				}
				else
				{
					stream = null;
				}
			}
			catch (ExchangeDataException)
			{
				stream = null;
			}
			finally
			{
				if (stream == null)
				{
					stream = mimePart.GetRawContentReadStream();
				}
			}
			return stream;
		}

		public static bool IsBlankLine(string line)
		{
			if (line == null)
			{
				return true;
			}
			Match match = MessageUtility.RegexBlankLine.Match(line);
			return Match.Empty != match;
		}

		public static string GetHeaderValue(Header header)
		{
			if (header == null)
			{
				throw new ArgumentNullException("header");
			}
			string text = null;
			if (!header.TryGetValue(out text))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					header.WriteTo(memoryStream);
					memoryStream.Position = 0L;
					using (StreamReader streamReader = new StreamReader(memoryStream))
					{
						text = streamReader.ReadToEnd();
						text = text.Substring(header.Name.Length + 1).Trim();
					}
				}
			}
			return text;
		}

		private static readonly Regex RegexBlankLine = new Regex("^\\s*$", RegexOptions.Compiled);
	}
}
