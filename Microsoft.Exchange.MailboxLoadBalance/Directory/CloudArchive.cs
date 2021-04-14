using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CloudArchive : DirectoryMailbox
	{
		public CloudArchive(IDirectoryProvider directory, DirectoryIdentity identity, IEnumerable<IPhysicalMailbox> physicalMailboxes) : base(directory, identity, physicalMailboxes, DirectoryMailboxType.Organization)
		{
		}

		public override bool IsArchiveOnly
		{
			get
			{
				return true;
			}
		}
	}
}
