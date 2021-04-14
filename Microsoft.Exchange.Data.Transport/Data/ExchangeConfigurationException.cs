using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class ExchangeConfigurationException : LocalizedException
	{
		public ExchangeConfigurationException(string message) : base(new LocalizedString(message))
		{
		}

		public ExchangeConfigurationException(LocalizedString message) : base(message)
		{
		}

		public ExchangeConfigurationException(string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
		}

		public ExchangeConfigurationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ExchangeConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
