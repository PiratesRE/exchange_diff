using System;
using System.Runtime.Serialization;

namespace System.Threading.Tasks
{
	[__DynamicallyInvokable]
	[Serializable]
	public class TaskCanceledException : OperationCanceledException
	{
		[__DynamicallyInvokable]
		public TaskCanceledException() : base(Environment.GetResourceString("TaskCanceledException_ctor_DefaultMessage"))
		{
		}

		[__DynamicallyInvokable]
		public TaskCanceledException(string message) : base(message)
		{
		}

		[__DynamicallyInvokable]
		public TaskCanceledException(string message, Exception innerException) : base(message, innerException)
		{
		}

		[__DynamicallyInvokable]
		public TaskCanceledException(Task task) : base(Environment.GetResourceString("TaskCanceledException_ctor_DefaultMessage"), (task != null) ? task.CancellationToken : default(CancellationToken))
		{
			this.m_canceledTask = task;
		}

		protected TaskCanceledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[__DynamicallyInvokable]
		public Task Task
		{
			[__DynamicallyInvokable]
			get
			{
				return this.m_canceledTask;
			}
		}

		[NonSerialized]
		private Task m_canceledTask;
	}
}
