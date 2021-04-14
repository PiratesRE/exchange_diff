using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Ex12RenEntryIdStrategy : LocationEntryIdStrategy
	{
		internal Ex12RenEntryIdStrategy(StorePropertyDefinition property, LocationEntryIdStrategy.GetLocationPropertyBagDelegate getLocationPropertyBag, Ex12RenEntryIdStrategy.PersistenceId persistenceId) : base(property, getLocationPropertyBag)
		{
			this.persistenceId = persistenceId;
		}

		internal override byte[] GetEntryId(DefaultFolderContext context)
		{
			Ex12ExRenEntryParser ex12ExRenEntryParser = Ex12ExRenEntryParser.FromBytes(this.GetLocationPropertyBag(context).TryGetProperty(this.Property) as byte[]);
			return ex12ExRenEntryParser.GetEntryId(this.persistenceId);
		}

		internal override void SetEntryId(DefaultFolderContext context, byte[] entryId)
		{
			Ex12ExRenEntryParser ex12ExRenEntryParser = Ex12ExRenEntryParser.FromBytes(this.GetLocationPropertyBag(context).TryGetProperty(this.Property) as byte[]);
			ex12ExRenEntryParser.Insert(this.persistenceId, entryId);
			base.SetEntryId(context, ex12ExRenEntryParser.ToBytes());
		}

		internal override FolderSaveResult UnsetEntryId(DefaultFolderContext context)
		{
			Ex12ExRenEntryParser ex12ExRenEntryParser = Ex12ExRenEntryParser.FromBytes(this.GetLocationPropertyBag(context).TryGetProperty(this.Property) as byte[]);
			ex12ExRenEntryParser.Remove(this.persistenceId);
			base.SetEntryId(context, ex12ExRenEntryParser.ToBytes());
			return FolderPropertyBag.SuccessfulSave;
		}

		private Ex12RenEntryIdStrategy.PersistenceId persistenceId;

		internal enum PersistenceId : ushort
		{
			Mask = 32768,
			RssSubscriptions,
			SendAndTrack,
			InfoMail,
			ToDoSearch,
			ToDoSearchOffline,
			ConversationActions,
			ImContactList = 32778,
			QuickContacts
		}
	}
}
