using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	[DataContract]
	internal class DirectoryServer : DirectoryContainerParent
	{
		public DirectoryServer(IDirectoryProvider directory, DirectoryIdentity identity, string fqdn) : base(directory, identity)
		{
			this.Fqdn = fqdn;
		}

		[DataMember]
		public string Fqdn { get; private set; }

		public IEnumerable<DirectoryDatabase> Databases
		{
			get
			{
				return base.Children.Cast<DirectoryDatabase>();
			}
		}

		protected override IEnumerable<DirectoryObject> FetchChildren()
		{
			return base.Directory.GetDatabasesOwnedByServer(this);
		}
	}
}
