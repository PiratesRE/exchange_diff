using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchTaskCancelled : MultiMailboxSearchException
	{
		public DiscoverySearchTaskCancelled(MailboxInfoList mailboxes, Guid databaseId) : base(Strings.SearchTaskCancelled(mailboxes.ToString(), databaseId.ToString()))
		{
		}

		protected DiscoverySearchTaskCancelled(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
