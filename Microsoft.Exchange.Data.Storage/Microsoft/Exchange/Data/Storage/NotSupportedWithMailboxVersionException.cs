using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NotSupportedWithMailboxVersionException : StoragePermanentException
	{
		public NotSupportedWithMailboxVersionException() : base(ServerStrings.NotSupportedWithMailboxVersionException)
		{
		}

		public NotSupportedWithMailboxVersionException(Exception innerException) : base(ServerStrings.NotSupportedWithMailboxVersionException, innerException)
		{
		}

		protected NotSupportedWithMailboxVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
