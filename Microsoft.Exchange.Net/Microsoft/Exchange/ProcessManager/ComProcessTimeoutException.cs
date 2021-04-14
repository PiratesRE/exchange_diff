using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.ProcessManager
{
	internal class ComProcessTimeoutException : ComProcessManagerException
	{
		internal ComProcessTimeoutException(string message) : base(message)
		{
		}

		internal ComProcessTimeoutException(string message, Exception inner) : base(message, inner)
		{
		}

		public ComProcessTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
