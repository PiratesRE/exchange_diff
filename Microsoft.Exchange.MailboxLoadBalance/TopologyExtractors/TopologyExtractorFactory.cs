using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TopologyExtractorFactory
	{
		public TopologyExtractorFactory(ILogger logger)
		{
			this.Logger = logger;
		}

		private protected ILogger Logger { protected get; private set; }

		public virtual TopologyExtractor GetExtractor(DirectoryObject directoryObject)
		{
			DirectoryMailbox directoryMailbox = directoryObject as DirectoryMailbox;
			if (directoryMailbox != null)
			{
				return this.CreateMailboxExtractor(directoryMailbox);
			}
			DirectoryDatabase directoryDatabase = directoryObject as DirectoryDatabase;
			if (directoryDatabase != null)
			{
				return this.CreateDatabaseExtractor(directoryDatabase);
			}
			DirectoryServer directoryServer = directoryObject as DirectoryServer;
			if (directoryServer != null)
			{
				return this.CreateServerExtractor(directoryServer);
			}
			DirectoryDatabaseAvailabilityGroup directoryDatabaseAvailabilityGroup = directoryObject as DirectoryDatabaseAvailabilityGroup;
			if (directoryDatabaseAvailabilityGroup != null)
			{
				return this.CreateDagExtractor(directoryDatabaseAvailabilityGroup);
			}
			DirectoryForest directoryForest = directoryObject as DirectoryForest;
			if (directoryForest != null)
			{
				return this.CreateForestExtractor(directoryForest);
			}
			DirectoryContainerParent directoryContainerParent = directoryObject as DirectoryContainerParent;
			if (directoryContainerParent != null)
			{
				return this.CreateContainerParentExtractor(directoryContainerParent);
			}
			return null;
		}

		protected virtual TopologyExtractor CreateContainerParentExtractor(DirectoryContainerParent container)
		{
			return null;
		}

		protected virtual TopologyExtractor CreateDagExtractor(DirectoryDatabaseAvailabilityGroup directoryDag)
		{
			return this.CreateContainerParentExtractor(directoryDag);
		}

		protected virtual TopologyExtractor CreateDatabaseExtractor(DirectoryDatabase database)
		{
			return null;
		}

		protected virtual TopologyExtractor CreateForestExtractor(DirectoryForest directoryForest)
		{
			return this.CreateContainerParentExtractor(directoryForest);
		}

		protected virtual TopologyExtractor CreateMailboxExtractor(DirectoryMailbox mailbox)
		{
			return null;
		}

		protected virtual TopologyExtractor CreateServerExtractor(DirectoryServer directoryServer)
		{
			return this.CreateContainerParentExtractor(directoryServer);
		}
	}
}
