using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class DatabaseLocationUnavailableException : StoragePermanentException
	{
		public DatabaseLocationUnavailableException(LocalizedString message) : base(message)
		{
		}

		public DatabaseLocationUnavailableException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected DatabaseLocationUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
