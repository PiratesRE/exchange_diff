using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.ProcessManager
{
	internal class ComProcessBeyondMemoryLimitException : ComProcessManagerException
	{
		internal ComProcessBeyondMemoryLimitException(string message) : base(message)
		{
		}

		internal ComProcessBeyondMemoryLimitException(string message, Exception inner) : base(message, inner)
		{
		}

		public ComProcessBeyondMemoryLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
