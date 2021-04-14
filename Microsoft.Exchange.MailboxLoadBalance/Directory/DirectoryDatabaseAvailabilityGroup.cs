using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[DataContract]
	internal class DirectoryDatabaseAvailabilityGroup : DirectoryContainerParent
	{
		public DirectoryDatabaseAvailabilityGroup(IDirectoryProvider directory, DirectoryIdentity identity) : base(directory, identity)
		{
		}

		public IEnumerable<DirectoryServer> Servers
		{
			get
			{
				return base.Children.Cast<DirectoryServer>();
			}
		}

		protected override IEnumerable<DirectoryObject> FetchChildren()
		{
			return base.Directory.GetServers(base.Identity);
		}
	}
}
