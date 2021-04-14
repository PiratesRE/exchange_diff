using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Globalization
{
	[Serializable]
	public class FailedToProgressException : ExchangeDataException
	{
		public FailedToProgressException(string message) : base(message)
		{
		}

		protected FailedToProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
