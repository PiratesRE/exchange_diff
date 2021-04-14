using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidSavedStateException : TransientException
	{
		public InvalidSavedStateException() : base(ReplayStrings.InvalidSavedStateException)
		{
		}

		public InvalidSavedStateException(Exception innerException) : base(ReplayStrings.InvalidSavedStateException, innerException)
		{
		}

		protected InvalidSavedStateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
