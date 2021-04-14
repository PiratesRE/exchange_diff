using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Transport
{
	[Serializable]
	internal class TransportComponentLoadFailedException : ApplicationException
	{
		public TransportComponentLoadFailedException() : base(Strings.TransportComponentLoadFailed)
		{
		}

		public TransportComponentLoadFailedException(string message) : base(message)
		{
		}

		public TransportComponentLoadFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public TransportComponentLoadFailedException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
