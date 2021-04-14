using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidSharingTargetRecipientException : StoragePermanentException
	{
		public InvalidSharingTargetRecipientException() : base(ServerStrings.InvalidSharingTargetRecipientException)
		{
		}

		public InvalidSharingTargetRecipientException(Exception innerException) : base(ServerStrings.InvalidSharingTargetRecipientException, innerException)
		{
		}

		protected InvalidSharingTargetRecipientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
