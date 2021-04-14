using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NonConnectedMailbox : DirectoryMailbox
	{
		public NonConnectedMailbox(IDirectoryProvider directory, DirectoryIdentity identity, IEnumerable<IPhysicalMailbox> physicalMailboxes) : base(directory, identity, physicalMailboxes, DirectoryMailboxType.Organization)
		{
		}

		public bool IsSoftDeleted
		{
			get
			{
				IPhysicalMailbox physicalMailbox = base.PhysicalMailboxes.FirstOrDefault<IPhysicalMailbox>();
				return physicalMailbox != null && physicalMailbox.IsSoftDeleted;
			}
		}

		public DateTime? DisconnectDate
		{
			get
			{
				IPhysicalMailbox physicalMailbox = base.PhysicalMailboxes.FirstOrDefault<IPhysicalMailbox>();
				if (physicalMailbox != null)
				{
					return physicalMailbox.DisconnectDate;
				}
				return null;
			}
		}
	}
}
