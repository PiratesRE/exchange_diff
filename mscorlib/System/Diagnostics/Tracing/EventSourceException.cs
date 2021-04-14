using System;
using System.Runtime.Serialization;

namespace System.Diagnostics.Tracing
{
	[__DynamicallyInvokable]
	[Serializable]
	public class EventSourceException : Exception
	{
		[__DynamicallyInvokable]
		public EventSourceException() : base(Environment.GetResourceString("EventSource_ListenerWriteFailure"))
		{
		}

		[__DynamicallyInvokable]
		public EventSourceException(string message) : base(message)
		{
		}

		[__DynamicallyInvokable]
		public EventSourceException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected EventSourceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal EventSourceException(Exception innerException) : base(Environment.GetResourceString("EventSource_ListenerWriteFailure"), innerException)
		{
		}
	}
}
