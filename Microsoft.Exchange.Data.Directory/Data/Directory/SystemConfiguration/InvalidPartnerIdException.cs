using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class InvalidPartnerIdException : InvalidOperationException
	{
		public InvalidPartnerIdException(string message) : base(message)
		{
		}

		public InvalidPartnerIdException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidPartnerIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
