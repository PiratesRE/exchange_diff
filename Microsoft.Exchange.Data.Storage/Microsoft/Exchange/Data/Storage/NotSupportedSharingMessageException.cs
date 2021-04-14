using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NotSupportedSharingMessageException : StoragePermanentException
	{
		public NotSupportedSharingMessageException() : base(ServerStrings.NotSupportedSharingMessageException)
		{
		}

		public NotSupportedSharingMessageException(Exception innerException) : base(ServerStrings.NotSupportedSharingMessageException, innerException)
		{
		}

		protected NotSupportedSharingMessageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
