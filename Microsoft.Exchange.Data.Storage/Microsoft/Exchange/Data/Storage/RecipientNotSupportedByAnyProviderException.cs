using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RecipientNotSupportedByAnyProviderException : StoragePermanentException
	{
		public RecipientNotSupportedByAnyProviderException() : base(ServerStrings.RecipientNotSupportedByAnyProviderException)
		{
		}

		public RecipientNotSupportedByAnyProviderException(Exception innerException) : base(ServerStrings.RecipientNotSupportedByAnyProviderException, innerException)
		{
		}

		protected RecipientNotSupportedByAnyProviderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
