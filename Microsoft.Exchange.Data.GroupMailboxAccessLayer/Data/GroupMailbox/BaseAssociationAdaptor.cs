using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class BaseAssociationAdaptor : IAssociationAdaptor
	{
		public BaseAssociationAdaptor(IAssociationStore associationStore, IRecipientSession adSession, MailboxLocator masterMailboxLocator)
		{
			ArgumentValidator.ThrowIfNull("associationStore", associationStore);
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			ArgumentValidator.ThrowIfNull("masterMailboxLocator", masterMailboxLocator);
			this.associationStore = associationStore;
			this.adSession = adSession;
			this.masterMailboxLocator = masterMailboxLocator;
		}

		public event Action<IMailboxLocator> OnAfterJoin;

		public IMailboxLocator MasterLocator
		{
			get
			{
				return this.masterMailboxLocator;
			}
		}

		public IRecipientSession AdSession
		{
			get
			{
				return this.adSession;
			}
		}

		public IAssociationStore AssociationStore
		{
			get
			{
				return this.associationStore;
			}
		}

		public MasterMailboxType MasterMailboxData { get; set; }

		public bool UseAlternateLocatorLookup { get; set; }

		protected abstract Trace Tracer { get; }

		protected abstract PropertyDefinition[] PropertiesToLoad { get; }

		protected abstract string ItemClass { get; }

		public abstract MailboxLocator GetSlaveMailboxLocator(MailboxAssociation association);

		public abstract MailboxAssociation GetAssociation(VersionedId itemId);

		public void DeleteAssociation(MailboxAssociation association)
		{
			MailboxAssociationFromStore mailboxAssociationFromStore = association as MailboxAssociationFromStore;
			IMailboxAssociationBaseItem mailboxAssociationBaseItem;
			if (mailboxAssociationFromStore != null)
			{
				this.Tracer.TraceDebug<VersionedId, MailboxAssociationFromStore>((long)this.GetHashCode(), "BaseAssociationAdaptor.DeleteAssociation: Found MailboxAssociationFromStore querying store by ItemId. ItemId={0}. Association={1}.", mailboxAssociationFromStore.ItemId, mailboxAssociationFromStore);
				mailboxAssociationBaseItem = this.GetAssociationByItemId(mailboxAssociationFromStore.ItemId);
			}
			else
			{
				IMailboxLocator slaveMailboxLocator = this.GetSlaveMailboxLocator(association);
				this.Tracer.TraceDebug<IMailboxLocator, MailboxAssociation>((long)this.GetHashCode(), "BaseAssociationAdaptor.DeleteAssociation: Found in memory MailboxAssociation, querying store by slave locator. Slave Locator={0}. Association={1}.", slaveMailboxLocator, association);
				mailboxAssociationBaseItem = this.GetItemFromStore(slaveMailboxLocator);
			}
			if (mailboxAssociationBaseItem != null)
			{
				this.Tracer.TraceDebug((long)this.GetHashCode(), "BaseAssociationAdaptor.DeleteAssociation: Association item found in store, Deleting.");
				this.associationStore.DeleteAssociation(mailboxAssociationBaseItem);
				return;
			}
			this.Tracer.TraceDebug((long)this.GetHashCode(), "BaseAssociationAdaptor.DeleteAssociation: Association item not found. No action needed.");
		}

		public MailboxAssociation GetAssociation(IMailboxLocator locator)
		{
			ArgumentValidator.ThrowIfNull("locator", locator);
			this.ValidateTargetLocatorType(locator);
			MailboxAssociation mailboxAssociation;
			using (IMailboxAssociationBaseItem itemFromStore = this.GetItemFromStore(locator))
			{
				if (itemFromStore != null)
				{
					this.Tracer.TraceDebug<IMailboxLocator>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetAssociation: Creating association from store item. Locator={0}", locator);
					mailboxAssociation = this.CreateMailboxAssociationFromItem(itemFromStore, true);
				}
				else
				{
					this.Tracer.TraceDebug<IMailboxLocator>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetAssociation: Creating default association. Locator={0}", locator);
					mailboxAssociation = this.CreateMailboxAssociationWithDefaultValues(locator);
				}
			}
			this.Tracer.TraceDebug<IMailboxLocator, MailboxAssociation>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetAssociation: Returning association for Locator={0}. Association: {1}", locator, mailboxAssociation);
			return mailboxAssociation;
		}

		public IEnumerable<MailboxAssociation> GetAllAssociations()
		{
			IEnumerable<IPropertyBag> foundItems = this.associationStore.GetAllAssociations(this.ItemClass, this.PropertiesToLoad);
			foreach (IPropertyBag item in foundItems)
			{
				MailboxAssociation association = this.CreateMailboxAssociationFromItem(item, false);
				this.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetAllAssociations: Returning association: {0}", association);
				yield return association;
			}
			yield break;
		}

		public IEnumerable<MailboxAssociation> GetMembershipAssociations(int? maxItems)
		{
			IEnumerable<IPropertyBag> foundItems = this.associationStore.GetAssociationsByType(this.ItemClass, MailboxAssociationBaseSchema.IsMember, maxItems, this.PropertiesToLoad);
			foreach (IPropertyBag item in foundItems)
			{
				MailboxAssociation association = this.CreateMailboxAssociationFromItem(item, false);
				this.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetMembershipAssociations: Returning association: {0}", association);
				yield return association;
			}
			yield break;
		}

		public IEnumerable<MailboxAssociation> GetEscalatedAssociations()
		{
			IEnumerable<IPropertyBag> foundItems = this.associationStore.GetAssociationsByType(this.ItemClass, MailboxAssociationBaseSchema.ShouldEscalate, this.PropertiesToLoad);
			foreach (IPropertyBag item in foundItems)
			{
				MailboxAssociation association = this.CreateMailboxAssociationFromItem(item, false);
				this.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetEscalatedAssociations: Returning association: {0}", association);
				yield return association;
			}
			yield break;
		}

		public IEnumerable<MailboxAssociation> GetPinAssociations()
		{
			IEnumerable<IPropertyBag> foundItems = this.associationStore.GetAssociationsByType(this.ItemClass, MailboxAssociationBaseSchema.IsPin, this.PropertiesToLoad);
			foreach (IPropertyBag item in foundItems)
			{
				MailboxAssociation association = this.CreateMailboxAssociationFromItem(item, false);
				this.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetPinAssociations: Returning association: {0}", association);
				yield return association;
			}
			yield break;
		}

		public IEnumerable<MailboxAssociation> GetAssociationsWithMembershipChangedAfter(ExDateTime date)
		{
			IEnumerable<IPropertyBag> foundItems = this.associationStore.GetAssociationsWithMembershipChangedAfter(date, this.PropertiesToLoad);
			foreach (IPropertyBag item in foundItems)
			{
				MailboxAssociation association = this.CreateMailboxAssociationFromItem(item, false);
				this.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetMembershipChangedAfterAssociations: Returning association: {0}", association);
				yield return association;
			}
			yield break;
		}

		public void SaveAssociation(MailboxAssociation association, bool markForReplication)
		{
			this.SaveAssociationInternal(association, markForReplication, new Action<MailboxAssociation, IMailboxAssociationBaseItem>(this.UpdateStoreAssociationMasterData));
		}

		public void ReplicateAssociation(MailboxAssociation association)
		{
			this.SaveAssociationInternal(association, false, new Action<MailboxAssociation, IMailboxAssociationBaseItem>(this.UpdateStoreAssociationSlaveData));
		}

		public void SaveSyncState(MailboxAssociation association)
		{
			this.SaveAssociationInternal(association, false, new Action<MailboxAssociation, IMailboxAssociationBaseItem>(BaseAssociationAdaptor.UpdateStoreAssociationSyncState));
		}

		protected static void UpdateLocatorDataInStoreItem(IMailboxLocator mailboxLocator, IMailboxAssociationBaseItem item)
		{
			if (!string.IsNullOrEmpty(mailboxLocator.LegacyDn))
			{
				item.LegacyDN = mailboxLocator.LegacyDn;
			}
			if (!string.IsNullOrEmpty(mailboxLocator.ExternalId))
			{
				item.ExternalId = mailboxLocator.ExternalId;
			}
		}

		protected abstract MailboxAssociation CreateMailboxAssociationWithDefaultValues(IMailboxLocator locator);

		protected abstract MailboxAssociation CreateMailboxAssociationFromItem(IPropertyBag item, bool setExtendedProperties = false);

		protected abstract void ValidateTargetLocatorType(IMailboxLocator locator);

		protected IMailboxAssociationBaseItem ReadOrCreateMailboxItem(MailboxAssociation mailboxAssociation)
		{
			MailboxAssociationFromStore mailboxAssociationFromStore = mailboxAssociation as MailboxAssociationFromStore;
			if (mailboxAssociationFromStore != null)
			{
				this.Tracer.TraceDebug<VersionedId>((long)this.GetHashCode(), "BaseAssociationAdaptor.ReadOrCreateMailboxItem. Binding item using entry id found in property bag. Id = {0}", mailboxAssociationFromStore.ItemId);
				IMailboxAssociationBaseItem associationByItemId = this.GetAssociationByItemId(mailboxAssociationFromStore.ItemId);
				this.associationStore.OpenAssociationAsReadWrite(associationByItemId);
				return associationByItemId;
			}
			this.Tracer.TraceDebug((long)this.GetHashCode(), "BaseAssociationAdaptor.ReadOrCreateMailboxItem. MailboxAssociation was not instantiated from store item, querying store");
			MailboxLocator slaveMailboxLocator = this.GetSlaveMailboxLocator(mailboxAssociation);
			return this.ReadOrCreateMailboxItem(slaveMailboxLocator);
		}

		protected abstract IMailboxAssociationBaseItem GetAssociationByItemId(VersionedId itemId);

		protected abstract IMailboxAssociationBaseItem CreateStoreItem(MailboxLocator locator);

		protected abstract IMailboxAssociationBaseItem GetAssociationByIdProperty(PropertyDefinition idProperty, params object[] idValues);

		protected abstract void UpdateStoreAssociationMasterData(MailboxAssociation association, IMailboxAssociationBaseItem item);

		protected abstract void UpdateStoreAssociationSlaveData(MailboxAssociation association, IMailboxAssociationBaseItem item);

		private static void UpdateStoreAssociationSyncState(MailboxAssociation association, IMailboxAssociationBaseItem item)
		{
			item.SyncedVersion = association.SyncedVersion;
			item.LastSyncError = (association.LastSyncError ?? string.Empty);
			item.SyncAttempts = association.SyncAttempts;
			item.SyncedSchemaVersion = (association.SyncedSchemaVersion ?? string.Empty);
			if (association.SyncedIdentityHash != null)
			{
				item.SyncedIdentityHash = association.SyncedIdentityHash;
			}
		}

		private void SaveAssociationInternal(MailboxAssociation association, bool incrementReplicationVersion, Action<MailboxAssociation, IMailboxAssociationBaseItem> updateFunction)
		{
			ArgumentValidator.ThrowIfNull("association", association);
			ArgumentValidator.ThrowIfNull("updateFunction", updateFunction);
			using (IMailboxAssociationBaseItem mailboxAssociationBaseItem = this.ReadOrCreateMailboxItem(association))
			{
				bool isMember = mailboxAssociationBaseItem.IsMember;
				updateFunction(association, mailboxAssociationBaseItem);
				if (incrementReplicationVersion)
				{
					this.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "BaseAssociationAdaptor.SaveAssociationInternal. Incrementing CurrentVersion of the association item. Association = {0}", association);
					mailboxAssociationBaseItem.CurrentVersion++;
					association.CurrentVersion++;
				}
				else
				{
					this.Tracer.TraceDebug<MailboxAssociation>((long)this.GetHashCode(), "BaseAssociationAdaptor.SaveAssociationInternal. Saving association without affecting CurrentVersion of the item. Association = {0}", association);
				}
				this.associationStore.SaveAssociation(mailboxAssociationBaseItem);
				if (!isMember && association.IsMember && this.OnAfterJoin != null)
				{
					this.OnAfterJoin(this.GetSlaveMailboxLocator(association));
				}
			}
		}

		private IMailboxAssociationBaseItem ReadOrCreateMailboxItem(MailboxLocator mailboxLocator)
		{
			IMailboxAssociationBaseItem mailboxAssociationBaseItem = this.GetItemFromStore(mailboxLocator);
			if (mailboxAssociationBaseItem != null)
			{
				this.Tracer.TraceDebug<MailboxLocator>((long)this.GetHashCode(), "GroupAssociationAdaptor.ReadOrCreateMailboxItem: Association item found in store, opening for read/write. Locator={0}.", mailboxLocator);
				this.associationStore.OpenAssociationAsReadWrite(mailboxAssociationBaseItem);
			}
			else
			{
				this.Tracer.TraceDebug<MailboxLocator>((long)this.GetHashCode(), "GroupAssociationAdaptor.ReadOrCreateMailboxItem: Association item not found in store, creating new item. Locator={0}.", mailboxLocator);
				mailboxAssociationBaseItem = this.CreateStoreItem(mailboxLocator);
			}
			return mailboxAssociationBaseItem;
		}

		private IMailboxAssociationBaseItem GetItemFromStore(IMailboxLocator locator)
		{
			IMailboxAssociationBaseItem mailboxAssociationBaseItem = null;
			if (!string.IsNullOrEmpty(locator.ExternalId))
			{
				this.Tracer.TraceDebug<IMailboxLocator>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetItemFromStore: Querying item in store by ExternalDirectoryObjectId. Locator={0}.", locator);
				mailboxAssociationBaseItem = this.GetAssociationByIdProperty(MailboxAssociationBaseSchema.ExternalId, new object[]
				{
					locator.ExternalId
				});
			}
			if (string.IsNullOrEmpty(locator.ExternalId) || (mailboxAssociationBaseItem == null && this.UseAlternateLocatorLookup))
			{
				this.Tracer.TraceDebug<IMailboxLocator>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetItemFromStore: Querying item in store by LegacyExchangeDN. Locator={0}.", locator);
				mailboxAssociationBaseItem = this.GetAssociationByIdProperty(MailboxAssociationBaseSchema.LegacyDN, new object[]
				{
					locator.LegacyDn
				});
				if (mailboxAssociationBaseItem == null)
				{
					this.Tracer.TraceDebug<IMailboxLocator>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetItemFromStore: Querying item in store by Alternate LegacyExchangeDN. Locator={0}.", locator);
					try
					{
						string[] idValues = locator.FindAlternateLegacyDNs();
						mailboxAssociationBaseItem = this.GetAssociationByIdProperty(MailboxAssociationBaseSchema.LegacyDN, idValues);
					}
					catch (MailboxNotFoundException arg)
					{
						this.Tracer.TraceDebug<IMailboxLocator, MailboxNotFoundException>((long)this.GetHashCode(), "BaseAssociationAdaptor.GetItemFromStore: Couldn't find Alternate Legacy DNs for the locator as the ADObject was not found. Returning NULL item. Locator={0}. Exception={1}", locator, arg);
					}
				}
			}
			return mailboxAssociationBaseItem;
		}

		private readonly IAssociationStore associationStore;

		private readonly IRecipientSession adSession;

		private readonly MailboxLocator masterMailboxLocator;
	}
}
