using System;
using System.IO;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	public class HtmlReaderWrapper : IHtmlReader, IDisposable
	{
		internal HtmlReaderWrapper(string html)
		{
			StringReader input = new StringReader(html);
			this.reader = new HtmlReader(input);
		}

		public void SetNormalizeHtml(bool value)
		{
			this.reader.NormalizeHtml = value;
		}

		public bool ParseNext()
		{
			return this.reader.ReadNextToken();
		}

		public TokenKind GetTokenKind()
		{
			switch (this.reader.TokenKind)
			{
			case HtmlTokenKind.Text:
				return TokenKind.Text;
			case HtmlTokenKind.StartTag:
				return TokenKind.StartTag;
			case HtmlTokenKind.EndTag:
				return TokenKind.EndTag;
			case HtmlTokenKind.EmptyElementTag:
				return TokenKind.EmptyTag;
			case HtmlTokenKind.OverlappedClose:
				return TokenKind.OverlappedClose;
			case HtmlTokenKind.OverlappedReopen:
				return TokenKind.OverlappedReopen;
			}
			return TokenKind.IgnorableTag;
		}

		public string GetTagName()
		{
			return this.reader.ReadTagName();
		}

		public int GetCurrentOffset()
		{
			return this.reader.CurrentOffset;
		}

		public bool ParseNextAttribute()
		{
			return this.reader.AttributeReader.ReadNext();
		}

		public string GetAttributeName()
		{
			return this.reader.AttributeReader.ReadName();
		}

		public string GetAttributeValue()
		{
			return this.reader.AttributeReader.ReadValue();
		}

		public void Dispose()
		{
			this.reader.Dispose();
		}

		private HtmlReader reader;
	}
}
