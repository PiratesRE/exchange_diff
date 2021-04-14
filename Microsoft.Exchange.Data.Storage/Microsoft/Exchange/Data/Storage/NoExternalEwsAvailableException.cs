using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NoExternalEwsAvailableException : StoragePermanentException
	{
		public NoExternalEwsAvailableException() : base(ServerStrings.NoExternalEwsAvailableException)
		{
		}

		public NoExternalEwsAvailableException(Exception innerException) : base(ServerStrings.NoExternalEwsAvailableException, innerException)
		{
		}

		protected NoExternalEwsAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
