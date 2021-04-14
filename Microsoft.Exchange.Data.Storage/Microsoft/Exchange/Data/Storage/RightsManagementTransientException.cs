using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class RightsManagementTransientException : StorageTransientException
	{
		public RightsManagementTransientException(LocalizedString message) : base(message)
		{
		}

		public RightsManagementTransientException(LocalizedString message, LocalizedException innerException) : base(message, innerException)
		{
		}

		protected RightsManagementTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
