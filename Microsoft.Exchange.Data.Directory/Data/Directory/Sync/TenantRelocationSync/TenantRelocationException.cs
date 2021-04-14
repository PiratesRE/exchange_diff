using System;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TenantRelocationException : Exception
	{
		public RelocationError RelocationErrorCode { get; private set; }

		public TenantRelocationException(RelocationError errorCode, string message, Exception innerException) : base(message, innerException)
		{
			this.RelocationErrorCode = errorCode;
		}

		public TenantRelocationException(RelocationError errorCode, string message) : base(message)
		{
			this.RelocationErrorCode = errorCode;
		}
	}
}
