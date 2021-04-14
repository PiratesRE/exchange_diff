using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class DiscoverySearchPermanentException : MultiMailboxSearchException
	{
		public DiscoverySearchPermanentException(LocalizedString message) : base(message)
		{
		}

		public DiscoverySearchPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected DiscoverySearchPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
