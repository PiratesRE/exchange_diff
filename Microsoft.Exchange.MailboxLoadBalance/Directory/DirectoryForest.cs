using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[DataContract]
	internal class DirectoryForest : DirectoryContainerParent
	{
		public DirectoryForest(IDirectoryProvider directory, DirectoryIdentity identity) : base(directory, identity)
		{
		}

		public IEnumerable<DirectoryDatabaseAvailabilityGroup> DatabaseAvailabilityGroups
		{
			get
			{
				return base.Children.Cast<DirectoryDatabaseAvailabilityGroup>();
			}
		}

		protected override IEnumerable<DirectoryObject> FetchChildren()
		{
			return base.Directory.GetDatabaseAvailabilityGroups();
		}
	}
}
