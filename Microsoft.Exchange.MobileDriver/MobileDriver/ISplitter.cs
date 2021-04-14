using System;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal interface ISplitter
	{
		bool OnePass { get; }

		PartType PartType { get; }

		BookmarkRetriever Split(string text);
	}
}
