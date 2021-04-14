using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DefaultCalendarNodeCreationException : StorageTransientException
	{
		public DefaultCalendarNodeCreationException() : base(ServerStrings.idUnableToAddDefaultCalendarToDefaultCalendarGroup)
		{
		}

		public DefaultCalendarNodeCreationException(Exception innerException) : base(ServerStrings.idUnableToAddDefaultCalendarToDefaultCalendarGroup, innerException)
		{
		}

		protected DefaultCalendarNodeCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
