using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.EntitySets.AttachmentCommands;

namespace Microsoft.Exchange.Entities.EntitySets
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class Attachments : StorageEntitySet<Attachments, IAttachment, IStoreSession>, IAttachments, IEntitySet<IAttachment>
	{
		protected internal Attachments(IStorageEntitySetScope<IStoreSession> parentScope, IEntityReference<IItem> parentItem, AttachmentDataProvider attachmentDataProvider, IEntityCommandFactory<Attachments, IAttachment> commandFactory = null) : base(parentScope, "Attachments", commandFactory ?? EntityCommandFactory<Attachments, IAttachment, CreateAttachment, DeleteAttachment, FindAttachments, ReadAttachment, UpdateAttachment>.Instance)
		{
			this.ParentItem = parentItem;
			this.AttachmentDataProvider = attachmentDataProvider;
		}

		public IEntityReference<IItem> ParentItem { get; private set; }

		public AttachmentDataProvider AttachmentDataProvider { get; private set; }
	}
}
