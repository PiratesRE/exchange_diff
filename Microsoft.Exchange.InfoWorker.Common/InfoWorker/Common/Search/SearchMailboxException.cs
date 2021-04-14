using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SearchMailboxException : LocalizedException
	{
		public SearchMailboxException(LocalizedString message) : base(message)
		{
		}

		public SearchMailboxException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SearchMailboxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
