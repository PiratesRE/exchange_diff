using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	[Serializable]
	public class E4eException : LocalizedException
	{
		public E4eException(string message) : base(new LocalizedString(message))
		{
		}

		public E4eException(LocalizedString message) : base(message)
		{
		}

		public E4eException(string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
		}

		public E4eException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected E4eException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
