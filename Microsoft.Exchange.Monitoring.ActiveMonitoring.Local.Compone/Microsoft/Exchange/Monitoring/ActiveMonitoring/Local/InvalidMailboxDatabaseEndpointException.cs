using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidMailboxDatabaseEndpointException : LocalizedException
	{
		public InvalidMailboxDatabaseEndpointException(LocalizedString message) : base(message)
		{
		}

		public InvalidMailboxDatabaseEndpointException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidMailboxDatabaseEndpointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
