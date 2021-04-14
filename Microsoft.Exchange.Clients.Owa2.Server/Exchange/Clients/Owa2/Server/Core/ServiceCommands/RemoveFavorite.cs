using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal sealed class RemoveFavorite
	{
		internal RemoveFavorite(IXSOFactory xsoFactory, MailboxSession mailboxSession, RequestDetailsLogger requestDetailsLogger, ItemId personaId)
		{
			ArgumentValidator.ThrowIfNull("xsoFactory", xsoFactory);
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("requestDetailsLogger", requestDetailsLogger);
			ArgumentValidator.ThrowIfNull("personaId", personaId);
			this.xso = xsoFactory;
			this.session = mailboxSession;
			this.logger = requestDetailsLogger;
			this.personaId = personaId;
			this.utilities = new UnifiedContactStoreUtilities(this.session, this.xso);
			this.favoritesPdlId = this.utilities.GetSystemPdlId(UnifiedContactStoreUtilities.FavoritesPdlDisplayName, "IPM.DistList.MOC.Favorites");
			this.SetLogValue(RemoveFavoriteMetadata.PersonaId, personaId);
		}

		internal bool Execute()
		{
			StoreObjectId defaultFolderId = this.session.GetDefaultFolderId(DefaultFolderType.QuickContacts);
			PersonId personId = IdConverter.EwsIdToPersonId(this.personaId.GetId());
			AllPersonContactsEnumerator allPersonContactsEnumerator = AllPersonContactsEnumerator.Create(this.session, personId, RemoveFavorite.ContactProperties);
			ExTraceGlobals.RemoveFavoriteTracer.TraceDebug((long)this.GetHashCode(), "Processing contacts.");
			int num = 0;
			foreach (IStorePropertyBag storePropertyBag in allPersonContactsEnumerator)
			{
				byte[] valueOrDefault = storePropertyBag.GetValueOrDefault<byte[]>(StoreObjectSchema.ParentEntryId, null);
				StoreObjectId objA = StoreObjectId.FromProviderSpecificIdOrNull(valueOrDefault);
				if (object.Equals(objA, defaultFolderId))
				{
					num++;
					VersionedId valueOrDefault2 = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
					if (valueOrDefault2 != null)
					{
						StoreId objectId = valueOrDefault2.ObjectId;
						this.utilities.RemoveContactFromGroup(objectId, this.favoritesPdlId);
						this.UnsetIsFavoriteFlagIfContactExists(objectId);
					}
				}
			}
			this.SetLogValue(RemoveFavoriteMetadata.NumberOfContacts, num);
			if (num == 0)
			{
				ExTraceGlobals.RemoveFavoriteTracer.TraceDebug<ItemId>((long)this.GetHashCode(), "No contacts found with personId: {0}", this.personaId);
			}
			return true;
		}

		private void UnsetIsFavoriteFlagIfContactExists(StoreId contactId)
		{
			try
			{
				using (IContact contact = this.xso.BindToContact(this.session, contactId, null))
				{
					ExTraceGlobals.RemoveFavoriteTracer.TraceDebug((long)this.GetHashCode(), "Unsetting the IsFavorite flag on the contact.");
					contact.OpenAsReadWrite();
					contact.IsFavorite = false;
					contact.Save(SaveMode.ResolveConflicts);
				}
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.RemoveFavoriteTracer.TraceDebug((long)this.GetHashCode(), "Contact doesn't exist to unset the IsFavorite flag.");
			}
		}

		private void SetLogValue(Enum key, object value)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(this.logger, key, value);
		}

		private static readonly PropertyDefinition[] ContactProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ParentEntryId
		};

		private readonly UnifiedContactStoreUtilities utilities;

		private readonly IXSOFactory xso;

		private readonly MailboxSession session;

		private readonly RequestDetailsLogger logger;

		private readonly ItemId personaId;

		private readonly StoreObjectId favoritesPdlId;
	}
}
