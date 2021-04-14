using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IAssociationStore : IDisposeTrackable, IDisposable
	{
		IMailboxLocator MailboxLocator { get; }

		string ServerFullyQualifiedDomainName { get; }

		MailboxAssociationProcessingFlags AssociationProcessingFlags { get; }

		ExDateTime MailboxNextSyncTime { get; }

		IExchangePrincipal MailboxOwner { get; }

		IMailboxAssociationGroup CreateGroupAssociation();

		IMailboxAssociationUser CreateUserAssociation();

		void SaveAssociation(IMailboxAssociationBaseItem association);

		void OpenAssociationAsReadWrite(IMailboxAssociationBaseItem associationItem);

		IEnumerable<IPropertyBag> GetAllAssociations(string associationItemClass, ICollection<PropertyDefinition> propertiesToRetrieve);

		IEnumerable<IPropertyBag> GetAssociationsByType(string associationItemClass, PropertyDefinition associationTypeProperty, params PropertyDefinition[] propertiesToRetrieve);

		IEnumerable<IPropertyBag> GetAssociationsWithMembershipChangedAfter(ExDateTime date, params PropertyDefinition[] properties);

		IEnumerable<IPropertyBag> GetAssociationsByType(string associationItemClass, PropertyDefinition associationTypeProperty, int? maxItems, params PropertyDefinition[] propertiesToRetrieve);

		IMailboxAssociationGroup GetGroupAssociationByItemId(VersionedId itemId);

		IMailboxAssociationGroup GetGroupAssociationByIdProperty(PropertyDefinition idProperty, params object[] idValues);

		IMailboxAssociationUser GetUserAssociationByItemId(VersionedId itemId);

		IMailboxAssociationUser GetUserAssociationByIdProperty(PropertyDefinition idProperty, params object[] idValues);

		TValue GetValueOrDefault<TValue>(IPropertyBag propertyBag, PropertyDefinition propertyDefinition, TValue defaultValue);

		void DeleteAssociation(IMailboxAssociationBaseItem associationItem);

		void SaveMailboxAsOutOfSync();

		void SaveMailboxSyncStatus(ExDateTime nextReplicationTime);

		void SaveMailboxSyncStatus(ExDateTime nextReplicationTime, MailboxAssociationProcessingFlags mailboxAssociationProcessingFlags);
	}
}
