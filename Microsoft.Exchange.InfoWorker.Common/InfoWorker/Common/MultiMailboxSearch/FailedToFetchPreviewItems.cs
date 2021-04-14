using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class FailedToFetchPreviewItems : MultiMailboxSearchException
	{
		public FailedToFetchPreviewItems(string mailboxName) : base(Strings.FailedToFetchPreviewItems(mailboxName))
		{
		}
	}
}
