using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.ProcessManager
{
	internal class ComProcessBusyException : ComProcessManagerException
	{
		internal ComProcessBusyException(string message) : base(message)
		{
		}

		internal ComProcessBusyException(string message, Exception inner) : base(message, inner)
		{
		}

		public ComProcessBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
