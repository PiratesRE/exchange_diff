using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Conversion
{
	internal class MdbefException : ApplicationException
	{
		public MdbefException(string message) : base(message)
		{
		}

		public MdbefException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public MdbefException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
