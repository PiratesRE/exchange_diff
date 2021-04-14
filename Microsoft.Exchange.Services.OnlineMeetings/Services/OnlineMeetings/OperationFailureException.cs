using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	[Serializable]
	internal class OperationFailureException : OnlineMeetingSchedulerException
	{
		public OperationFailureException()
		{
		}

		public OperationFailureException(string message) : this(message, null)
		{
		}

		public OperationFailureException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected OperationFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
