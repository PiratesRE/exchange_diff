using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.Mserve
{
	[Serializable]
	internal class MserveException : Exception
	{
		public MserveException()
		{
		}

		public MserveException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public MserveException(string message) : base(message)
		{
		}

		protected MserveException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
		{
		}
	}
}
