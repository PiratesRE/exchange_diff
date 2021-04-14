using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.ExchangeSystem
{
	[Serializable]
	internal class EnumOutOfRangeException : ExArgumentOutOfRangeException
	{
		public EnumOutOfRangeException()
		{
		}

		public EnumOutOfRangeException(string paramName, string message) : base(paramName, message)
		{
		}

		public EnumOutOfRangeException(string paramName) : base(paramName)
		{
		}

		public EnumOutOfRangeException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public EnumOutOfRangeException(string paramName, object actualValue, string message) : base(paramName, actualValue, message)
		{
		}

		protected EnumOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
