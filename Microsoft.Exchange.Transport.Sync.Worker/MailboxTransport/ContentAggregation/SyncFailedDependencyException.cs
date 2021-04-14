using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SyncFailedDependencyException : Exception
	{
		public SyncFailedDependencyException(string message) : this(message, null)
		{
		}

		public SyncFailedDependencyException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public SyncFailedDependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
