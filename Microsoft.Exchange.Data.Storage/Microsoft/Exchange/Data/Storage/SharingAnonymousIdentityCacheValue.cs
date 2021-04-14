using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SharingAnonymousIdentityCacheValue
	{
		public SharingAnonymousIdentityCacheValue(string folderId, SecurityIdentifier sid)
		{
			this.folderId = folderId;
			this.sid = sid;
		}

		public string FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public SecurityIdentifier Sid
		{
			get
			{
				return this.sid;
			}
		}

		public bool IsAccessAllowed
		{
			get
			{
				return !string.IsNullOrEmpty(this.folderId);
			}
		}

		private readonly string folderId;

		private readonly SecurityIdentifier sid;
	}
}
