using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NullDatabaseException : TransientException
	{
		public NullDatabaseException() : base(ReplayStrings.NullDatabaseException)
		{
		}

		public NullDatabaseException(Exception innerException) : base(ReplayStrings.NullDatabaseException, innerException)
		{
		}

		protected NullDatabaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
