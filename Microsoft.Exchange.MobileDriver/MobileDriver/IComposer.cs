using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal interface IComposer
	{
		PartType PartType { get; }

		BookmarkRetriever Compose(IList<ProportionedText> proportionedTexts);
	}
}
