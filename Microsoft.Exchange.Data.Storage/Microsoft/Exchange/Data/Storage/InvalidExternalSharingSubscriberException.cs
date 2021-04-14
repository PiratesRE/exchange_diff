using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class InvalidExternalSharingSubscriberException : InvalidSharingTargetRecipientException
	{
		public InvalidExternalSharingSubscriberException(Exception innerException) : base(innerException)
		{
		}

		public InvalidExternalSharingSubscriberException()
		{
		}
	}
}
