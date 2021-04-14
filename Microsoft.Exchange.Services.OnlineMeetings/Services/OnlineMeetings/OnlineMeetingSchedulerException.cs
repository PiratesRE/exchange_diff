using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal abstract class OnlineMeetingSchedulerException : Exception
	{
		protected OnlineMeetingSchedulerException()
		{
		}

		protected OnlineMeetingSchedulerException(string message) : this(message, null)
		{
		}

		protected OnlineMeetingSchedulerException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected OnlineMeetingSchedulerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
