using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class AccountDisabledException : StoragePermanentException
	{
		public AccountDisabledException(LocalizedString message) : base(message)
		{
		}

		public AccountDisabledException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AccountDisabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
