using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchUserNotFoundException : MultiMailboxSearchException
	{
		public SearchUserNotFoundException(MailboxInfo info) : base(Strings.SearchUserNotFound(info.DisplayName))
		{
		}

		protected SearchUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
