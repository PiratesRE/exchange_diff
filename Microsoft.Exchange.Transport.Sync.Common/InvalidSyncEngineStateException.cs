using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidSyncEngineStateException : LocalizedException
	{
		public InvalidSyncEngineStateException() : base(Strings.InvalidSyncEngineStateException)
		{
		}

		public InvalidSyncEngineStateException(Exception innerException) : base(Strings.InvalidSyncEngineStateException, innerException)
		{
		}

		protected InvalidSyncEngineStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
