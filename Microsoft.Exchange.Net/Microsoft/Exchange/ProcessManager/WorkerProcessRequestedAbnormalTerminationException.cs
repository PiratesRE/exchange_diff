using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.ProcessManager
{
	[Serializable]
	internal class WorkerProcessRequestedAbnormalTerminationException : Exception
	{
		internal WorkerProcessRequestedAbnormalTerminationException(string message) : base(message)
		{
		}

		public WorkerProcessRequestedAbnormalTerminationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
