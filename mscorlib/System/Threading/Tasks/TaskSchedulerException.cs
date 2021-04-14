using System;
using System.Runtime.Serialization;

namespace System.Threading.Tasks
{
	[__DynamicallyInvokable]
	[Serializable]
	public class TaskSchedulerException : Exception
	{
		[__DynamicallyInvokable]
		public TaskSchedulerException() : base(Environment.GetResourceString("TaskSchedulerException_ctor_DefaultMessage"))
		{
		}

		[__DynamicallyInvokable]
		public TaskSchedulerException(string message) : base(message)
		{
		}

		[__DynamicallyInvokable]
		public TaskSchedulerException(Exception innerException) : base(Environment.GetResourceString("TaskSchedulerException_ctor_DefaultMessage"), innerException)
		{
		}

		[__DynamicallyInvokable]
		public TaskSchedulerException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected TaskSchedulerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
