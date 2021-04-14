using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedQueueingWorkItemException : TransientException
	{
		public FailedQueueingWorkItemException() : base(Strings.FailedQueueingWorkItemException)
		{
		}

		public FailedQueueingWorkItemException(Exception innerException) : base(Strings.FailedQueueingWorkItemException, innerException)
		{
		}

		protected FailedQueueingWorkItemException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
