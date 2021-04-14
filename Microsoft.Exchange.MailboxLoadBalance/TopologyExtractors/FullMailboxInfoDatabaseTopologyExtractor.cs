using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Constraints;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class FullMailboxInfoDatabaseTopologyExtractor : TopologyExtractor
	{
		public FullMailboxInfoDatabaseTopologyExtractor(DirectoryDatabase directoryObject, TopologyExtractorFactory extractorFactory, IList<Guid> nonMovableOrganizations) : base(directoryObject, extractorFactory)
		{
			this.nonMovableOrganizations = nonMovableOrganizations;
		}

		public override LoadContainer ExtractTopology()
		{
			LoadContainer loadContainer = this.CreateDatabaseContainer();
			this.ExtractConstraintSetHierarchy(loadContainer);
			return loadContainer;
		}

		protected virtual void AddEntityToContainer(LoadContainer databaseContainer, LoadEntity extractedEntity)
		{
			databaseContainer.ConsumedLoad += extractedEntity.ConsumedLoad;
		}

		private void ExtractConstraintSetHierarchy(LoadContainer databaseContainer)
		{
			DirectoryDatabase directoryDatabase = (DirectoryDatabase)base.DirectoryObject;
			Dictionary<string, LoadContainer> dictionary = new Dictionary<string, LoadContainer>();
			foreach (DirectoryMailbox directoryMailbox in directoryDatabase.GetMailboxes())
			{
				TopologyExtractor extractor = base.ExtractorFactory.GetExtractor(directoryMailbox);
				IMailboxProvisioningConstraints mailboxProvisioningConstraints = directoryMailbox.MailboxProvisioningConstraints;
				string text = null;
				if (mailboxProvisioningConstraints != null)
				{
					text = mailboxProvisioningConstraints.HardConstraint.Value;
				}
				text = (text ?? string.Empty);
				if (!dictionary.ContainsKey(text))
				{
					DirectoryIdentity identity = new DirectoryIdentity(DirectoryObjectType.ConstraintSet, Guid.NewGuid(), text, directoryMailbox.Identity.OrganizationId);
					DirectoryObject directoryObject = new DirectoryObject(base.DirectoryObject.Directory, identity);
					LoadContainer value = new LoadContainer(directoryObject, ContainerType.ConstraintSet);
					dictionary.Add(text, value);
					databaseContainer.AddChild(dictionary[text]);
				}
				LoadEntity extractedEntity = extractor.ExtractEntity();
				this.AddEntityToContainer(dictionary[text], extractedEntity);
			}
		}

		private LoadContainer CreateDatabaseContainer()
		{
			DirectoryDatabase directoryDatabase = (DirectoryDatabase)base.DirectoryObject;
			LoadContainer loadContainer = directoryDatabase.ToLoadContainer();
			loadContainer.Constraint = new AllAcceptConstraint(new IAllocationConstraint[]
			{
				loadContainer.Constraint,
				new SpecialMailboxPlacementConstraint(this.nonMovableOrganizations)
			});
			return loadContainer;
		}

		private readonly IList<Guid> nonMovableOrganizations;
	}
}
