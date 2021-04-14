using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchMailboxTaskException : MultiMailboxSearchException
	{
		public SearchMailboxTaskException(LocalizedString message) : base(message)
		{
		}

		public SearchMailboxTaskException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SearchMailboxTaskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
