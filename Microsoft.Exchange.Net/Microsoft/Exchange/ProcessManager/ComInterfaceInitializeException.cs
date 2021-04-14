using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.ProcessManager
{
	internal class ComInterfaceInitializeException : ComProcessManagerException
	{
		internal ComInterfaceInitializeException(string message) : base(message)
		{
		}

		internal ComInterfaceInitializeException(string message, Exception inner) : base(message, inner)
		{
		}

		public ComInterfaceInitializeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
