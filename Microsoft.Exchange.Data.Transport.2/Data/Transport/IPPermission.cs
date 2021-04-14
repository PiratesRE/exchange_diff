using System;
using System.Net;

namespace Microsoft.Exchange.Data.Transport
{
	public abstract class IPPermission
	{
		internal IPPermission()
		{
		}

		public abstract void AddRestriction(IPAddress ipAddress, TimeSpan expiration, string comments);

		public abstract PermissionCheckResults Check(IPAddress ipAddress);
	}
}
