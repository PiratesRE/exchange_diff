using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DatabaseCopyLayoutTableNullException : DatabaseCopyLayoutException
	{
		public DatabaseCopyLayoutTableNullException() : base(ReplayStrings.DatabaseCopyLayoutTableNullException)
		{
		}

		public DatabaseCopyLayoutTableNullException(Exception innerException) : base(ReplayStrings.DatabaseCopyLayoutTableNullException, innerException)
		{
		}

		protected DatabaseCopyLayoutTableNullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
