using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.ExchangeSystem
{
	[Serializable]
	internal class EnumArgumentException : ExArgumentException
	{
		public EnumArgumentException()
		{
		}

		public EnumArgumentException(string message) : base(message)
		{
		}

		public EnumArgumentException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public EnumArgumentException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
		{
		}

		public EnumArgumentException(string message, string paramName) : base(message, paramName)
		{
		}

		protected EnumArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
