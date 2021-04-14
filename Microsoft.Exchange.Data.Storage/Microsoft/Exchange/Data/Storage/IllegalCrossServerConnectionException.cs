using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class IllegalCrossServerConnectionException : WrongServerException
	{
		public IllegalCrossServerConnectionException(LocalizedString message) : base(message)
		{
		}

		public IllegalCrossServerConnectionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected IllegalCrossServerConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
