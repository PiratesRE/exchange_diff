using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DefaultTaskGroupCreationException : StorageTransientException
	{
		public DefaultTaskGroupCreationException() : base(ServerStrings.idUnableToCreateDefaultTaskGroupException)
		{
		}

		public DefaultTaskGroupCreationException(Exception innerException) : base(ServerStrings.idUnableToCreateDefaultTaskGroupException, innerException)
		{
		}

		protected DefaultTaskGroupCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
