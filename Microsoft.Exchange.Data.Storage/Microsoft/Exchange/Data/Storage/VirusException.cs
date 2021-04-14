using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public abstract class VirusException : StoragePermanentException
	{
		public VirusException(LocalizedString message) : base(message)
		{
		}

		public VirusException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected VirusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
