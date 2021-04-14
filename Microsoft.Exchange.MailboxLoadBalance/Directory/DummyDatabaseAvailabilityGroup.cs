using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class DummyDatabaseAvailabilityGroup : DirectoryDatabaseAvailabilityGroup
	{
		public DummyDatabaseAvailabilityGroup(IDirectoryProvider directory) : base(directory, new DirectoryIdentity(DirectoryObjectType.DatabaseAvailabilityGroup, Guid.Empty, string.Empty, Guid.Empty))
		{
		}

		protected override IEnumerable<DirectoryObject> FetchChildren()
		{
			return base.Directory.GetServers();
		}
	}
}
