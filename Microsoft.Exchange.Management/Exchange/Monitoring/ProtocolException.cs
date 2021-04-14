using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Monitoring
{
	[Serializable]
	public class ProtocolException : Exception
	{
		public ProtocolException()
		{
		}

		public ProtocolException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public ProtocolException(string message) : base(message)
		{
		}

		protected ProtocolException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
		{
		}
	}
}
