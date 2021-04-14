using System;
using System.Runtime.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	[Serializable]
	public class ForwardSyncProbeException : Exception
	{
		public ForwardSyncProbeException()
		{
		}

		public ForwardSyncProbeException(string message) : base(message)
		{
		}

		public ForwardSyncProbeException(string message, Exception inner) : base(message, inner)
		{
		}

		protected ForwardSyncProbeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
