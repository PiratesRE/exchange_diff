using System;
using System.Runtime.Serialization;

namespace <CrtImplementationDetails>
{
	[Serializable]
	internal class Exception : Exception
	{
		protected Exception(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public Exception(string message, Exception innerException) : base(message, innerException)
		{
		}

		public Exception(string message) : base(message)
		{
		}
	}
}
