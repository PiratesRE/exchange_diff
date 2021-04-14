using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.ContentTypes.Tnef
{
	[Serializable]
	public class TnefException : ExchangeDataException
	{
		public TnefException(string message) : base(message)
		{
		}

		public TnefException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected TnefException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
