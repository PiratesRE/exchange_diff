using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ClientSessionInfoParseException : StoragePermanentException
	{
		public ClientSessionInfoParseException() : base(ServerStrings.idClientSessionInfoParseException)
		{
		}

		public ClientSessionInfoParseException(Exception innerException) : base(ServerStrings.idClientSessionInfoParseException, innerException)
		{
		}

		protected ClientSessionInfoParseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
