using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidTenantFullSyncCookieException : LocalizedException
	{
		public InvalidTenantFullSyncCookieException() : base(DirectoryStrings.InvalidTenantFullSyncCookieException)
		{
		}

		public InvalidTenantFullSyncCookieException(Exception innerException) : base(DirectoryStrings.InvalidTenantFullSyncCookieException, innerException)
		{
		}

		protected InvalidTenantFullSyncCookieException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
