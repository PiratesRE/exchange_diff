using System;
using System.Runtime.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	[Serializable]
	public class AsyncQueueMonitorException : Exception
	{
		public AsyncQueueMonitorException()
		{
		}

		public AsyncQueueMonitorException(string message) : base(message)
		{
		}

		public AsyncQueueMonitorException(string message, Exception inner) : base(message, inner)
		{
		}

		protected AsyncQueueMonitorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
