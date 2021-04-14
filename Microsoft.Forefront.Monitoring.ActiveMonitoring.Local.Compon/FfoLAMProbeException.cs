using System;
using System.Runtime.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	[Serializable]
	public class FfoLAMProbeException : Exception
	{
		public FfoLAMProbeException()
		{
		}

		public FfoLAMProbeException(string message) : base(message)
		{
		}

		public FfoLAMProbeException(string message, Exception inner) : base(message, inner)
		{
		}

		protected FfoLAMProbeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
