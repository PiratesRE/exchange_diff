using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Transport
{
	[Serializable]
	public class AddressBookTransientException : TransientException
	{
		public AddressBookTransientException(string message) : base(new LocalizedString(message))
		{
		}

		public AddressBookTransientException(LocalizedString message) : base(message)
		{
		}

		public AddressBookTransientException(string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
		}

		public AddressBookTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AddressBookTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
