using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class TenantAccessBlockedException : StoragePermanentException
	{
		public TenantAccessBlockedException(LocalizedString message) : base(message)
		{
		}

		public TenantAccessBlockedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected TenantAccessBlockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
