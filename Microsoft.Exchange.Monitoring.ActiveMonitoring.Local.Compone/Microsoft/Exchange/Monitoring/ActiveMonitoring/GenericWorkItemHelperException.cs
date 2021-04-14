using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	[Serializable]
	public class GenericWorkItemHelperException : Exception
	{
		public GenericWorkItemHelperException()
		{
		}

		public GenericWorkItemHelperException(string message) : base(message)
		{
		}

		public GenericWorkItemHelperException(string message, Exception inner) : base(message, inner)
		{
		}

		protected GenericWorkItemHelperException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
