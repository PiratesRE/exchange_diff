using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class SessionDeadException : StoragePermanentException
	{
		public SessionDeadException(LocalizedString message) : base(message)
		{
		}

		public SessionDeadException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SessionDeadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
