using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class NoExternalOwaAvailableException : StoragePermanentException
	{
		public NoExternalOwaAvailableException() : base(ServerStrings.NoExternalOwaAvailableException)
		{
		}

		public NoExternalOwaAvailableException(Exception innerException) : base(ServerStrings.NoExternalOwaAvailableException, innerException)
		{
		}

		protected NoExternalOwaAvailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
