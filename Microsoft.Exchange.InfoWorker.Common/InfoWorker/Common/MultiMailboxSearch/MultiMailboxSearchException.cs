using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MultiMailboxSearchException : LocalizedException
	{
		public MultiMailboxSearchException(LocalizedString message) : base(message)
		{
		}

		public MultiMailboxSearchException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MultiMailboxSearchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
