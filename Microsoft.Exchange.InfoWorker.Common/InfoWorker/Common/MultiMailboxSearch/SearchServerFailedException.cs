using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchServerFailedException : MultiMailboxSearchException
	{
		public SearchServerFailedException(MailboxInfo info, int responseCode, string exceptionMessage) : base(Strings.SearchServerFailed(info.DisplayName, responseCode, exceptionMessage))
		{
		}

		protected SearchServerFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
