using System;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public enum LogicalIndexType
	{
		Messages,
		Conversations,
		SearchFolderBaseView,
		SearchFolderMessages,
		CategoryHeaders,
		ConversationDeleteHistory
	}
}
