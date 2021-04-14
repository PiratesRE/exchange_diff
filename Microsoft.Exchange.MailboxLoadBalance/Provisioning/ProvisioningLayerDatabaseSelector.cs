using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Providers;

namespace Microsoft.Exchange.MailboxLoadBalance.Provisioning
{
	internal class ProvisioningLayerDatabaseSelector : DatabaseSelector
	{
		public ProvisioningLayerDatabaseSelector(IDirectoryProvider directory, ILogger logger) : base(logger)
		{
			this.directory = directory;
		}

		protected override IEnumerable<LoadContainer> GetAvailableDatabases()
		{
			return this.directory.GetCachedDatabasesForProvisioning().Select(new Func<DirectoryDatabase, LoadContainer>(this.CreateSimpleLoadContainer));
		}

		private LoadContainer CreateSimpleLoadContainer(DirectoryDatabase database)
		{
			return database.ToLoadContainer();
		}

		private readonly IDirectoryProvider directory;
	}
}
