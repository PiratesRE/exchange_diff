using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class VirusScanInProgressException : StorageTransientException
	{
		public VirusScanInProgressException(LocalizedString message) : base(message)
		{
		}

		public VirusScanInProgressException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected VirusScanInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
