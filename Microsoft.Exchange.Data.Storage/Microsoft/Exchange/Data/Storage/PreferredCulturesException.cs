using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class PreferredCulturesException : StoragePermanentException
	{
		public PreferredCulturesException(LocalizedString message) : base(message)
		{
		}

		public PreferredCulturesException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected PreferredCulturesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
