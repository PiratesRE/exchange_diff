using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.ProcessManager
{
	internal abstract class ComProcessManagerException : LocalizedException
	{
		internal ComProcessManagerException(string message) : base(new LocalizedString(message))
		{
		}

		internal ComProcessManagerException(string message, Exception inner) : base(new LocalizedString(message), inner)
		{
		}

		public ComProcessManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
