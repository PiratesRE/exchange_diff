using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.ProcessManager
{
	internal class ComProcessManagerInitializationException : ComProcessManagerException
	{
		internal ComProcessManagerInitializationException(string message) : base(message)
		{
		}

		internal ComProcessManagerInitializationException(string message, Exception inner) : base(message, inner)
		{
		}

		public ComProcessManagerInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
