using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Diagnostics
{
	[Serializable]
	public sealed class LogException : TransientException
	{
		public LogException(string message) : base(new LocalizedString(message))
		{
		}

		public LogException(string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
		}

		private LogException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
		{
		}
	}
}
