using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class InvalidOperationForOrganizerException : StoragePermanentException
	{
		public InvalidOperationForOrganizerException(LocalizedString message) : base(message)
		{
		}

		public InvalidOperationForOrganizerException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidOperationForOrganizerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
