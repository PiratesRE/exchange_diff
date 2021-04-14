using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public interface IHtmlReader : IDisposable
	{
		void SetNormalizeHtml(bool value);

		bool ParseNext();

		TokenKind GetTokenKind();

		string GetTagName();

		int GetCurrentOffset();

		bool ParseNextAttribute();

		string GetAttributeName();

		string GetAttributeValue();
	}
}
