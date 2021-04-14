using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class TextSendingPackage
	{
		public TextSendingPackage(BookmarkRetriever bookmarkRetriever, ICollection<MobileRecipient> recipients)
		{
			this.BookmarkRetriever = bookmarkRetriever;
			this.Recipients = new List<MobileRecipient>(recipients).AsReadOnly();
		}

		public BookmarkRetriever BookmarkRetriever { get; private set; }

		public IList<MobileRecipient> Recipients { get; private set; }
	}
}
