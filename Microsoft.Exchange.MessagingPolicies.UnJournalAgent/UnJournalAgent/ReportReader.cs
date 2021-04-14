using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal class ReportReader : IDisposable
	{
		public ReportReader(Stream stream, bool plaintext)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.contentReader = new StreamReader(stream);
			this.plaintext = plaintext;
		}

		public string ReadLine()
		{
			string text = this.contentReader.ReadLine();
			if (!string.IsNullOrEmpty(text) && !this.plaintext)
			{
				text = ReportReader.ParseHtmlTokens(text);
			}
			return text;
		}

		public void Dispose()
		{
			this.contentReader.Dispose();
		}

		private static string ParseHtmlTokens(string line)
		{
			MatchEvaluator evaluator = new MatchEvaluator(ReportReader.ReplaceHtmlToken);
			return ReportReader.RegexHtmlToken.Replace(line, evaluator);
		}

		private static string ReplaceHtmlToken(Match match)
		{
			string text = match.Value.ToUpperInvariant();
			string a;
			if ((a = text) != null)
			{
				if (a == "&QUOT;")
				{
					return "\"";
				}
				if (a == "&LT;")
				{
					return "<";
				}
				if (a == "&GT;")
				{
					return ">";
				}
				if (a == "&NBSP;")
				{
					return " ";
				}
			}
			return string.Empty;
		}

		private static readonly Regex RegexHtmlToken = new Regex("(</?[^>]*>|&nbsp;|&quot;|&lt;|&gt;)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private readonly bool plaintext;

		private StreamReader contentReader;
	}
}
