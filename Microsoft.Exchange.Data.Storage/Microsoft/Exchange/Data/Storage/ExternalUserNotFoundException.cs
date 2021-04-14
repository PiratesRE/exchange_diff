using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExternalUserNotFoundException : StoragePermanentException
	{
		internal ExternalUserNotFoundException(SecurityIdentifier sid) : base(ServerStrings.ExternalUserNotFound((sid == null) ? string.Empty : sid.ToString()))
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			this.sid = sid;
		}

		public SecurityIdentifier UserSecurityIdentifier
		{
			get
			{
				return this.sid;
			}
		}

		private SecurityIdentifier sid;
	}
}
