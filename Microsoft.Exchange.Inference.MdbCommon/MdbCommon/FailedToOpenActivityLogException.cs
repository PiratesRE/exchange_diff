using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Search.Core.Abstraction;

namespace Microsoft.Exchange.Inference.MdbCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FailedToOpenActivityLogException : OperationFailedException
	{
		public FailedToOpenActivityLogException() : base(Strings.FailedToOpenActivityLog)
		{
		}

		public FailedToOpenActivityLogException(Exception innerException) : base(Strings.FailedToOpenActivityLog, innerException)
		{
		}

		protected FailedToOpenActivityLogException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
