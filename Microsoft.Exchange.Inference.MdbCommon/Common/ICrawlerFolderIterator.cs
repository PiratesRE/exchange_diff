using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Inference.Common
{
	internal interface ICrawlerFolderIterator
	{
		IEnumerable<StoreObjectId> GetFolders(MailboxSession session);
	}
}
