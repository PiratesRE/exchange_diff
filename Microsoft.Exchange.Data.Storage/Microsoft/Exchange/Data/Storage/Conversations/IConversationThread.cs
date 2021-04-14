using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Conversations
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IConversationThread : IBreadcrumbsSource, IConversationData, IThreadAggregatedProperties
	{
		StoreObjectId RootMessageId { get; }

		IConversationTree Tree { get; }

		void SyncThread();
	}
}
