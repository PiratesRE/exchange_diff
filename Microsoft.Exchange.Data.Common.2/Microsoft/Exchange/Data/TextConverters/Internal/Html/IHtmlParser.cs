using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal interface IHtmlParser
	{
		HtmlToken Token { get; }

		HtmlTokenId Parse();

		int CurrentOffset { get; }

		void SetRestartConsumer(IRestartable restartConsumer);
	}
}
