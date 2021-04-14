using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SyncConflictException : TransientException
	{
		public SyncConflictException() : base(Strings.SyncConflictException)
		{
		}

		public SyncConflictException(Exception innerException) : base(Strings.SyncConflictException, innerException)
		{
		}

		protected SyncConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
