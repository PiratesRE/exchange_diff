using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Serializable]
	public class InvalidLogCollectionException : ApplicationException
	{
		public InvalidLogCollectionException()
		{
		}

		public InvalidLogCollectionException(string message) : base(message)
		{
		}

		public InvalidLogCollectionException(string message, Exception inner) : base(message, inner)
		{
		}

		protected InvalidLogCollectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
