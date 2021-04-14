using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NullDbCopyException : TransientException
	{
		public NullDbCopyException() : base(ReplayStrings.NullDbCopyException)
		{
		}

		public NullDbCopyException(Exception innerException) : base(ReplayStrings.NullDbCopyException, innerException)
		{
		}

		protected NullDbCopyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
