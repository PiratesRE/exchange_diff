using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IAttachmentProvider : IDisposable
	{
		void SetCollection(CoreAttachmentCollection collection);

		PropertyBag[] QueryAttachmentTable(NativeStorePropertyDefinition[] properties);

		PersistablePropertyBag CreateAttachment(ICollection<PropertyDefinition> prefetchProperties, CoreAttachment attachmentToCopy, IItem itemToAttach, out int attachmentNumber);

		bool SupportsCreateClone(AttachmentPropertyBag attachmentBagToClone);

		PersistablePropertyBag OpenAttachment(ICollection<PropertyDefinition> prefetchProperties, AttachmentPropertyBag attachmentBag);

		void DeleteAttachment(int attachmentNumber);

		ICoreItem OpenAttachedItem(ICollection<PropertyDefinition> prefetchProperties, AttachmentPropertyBag attachmentBag, bool isNew);

		bool ExistsInCollection(AttachmentPropertyBag attachmentBag);

		void OnAttachmentLoad(AttachmentPropertyBag attachmentBag);

		void OnBeforeAttachmentSave(AttachmentPropertyBag attachmentBag);

		void OnAfterAttachmentSave(AttachmentPropertyBag attachmentBag);

		void OnAttachmentDisconnected(AttachmentPropertyBag attachmentBag, PersistablePropertyBag persistablePropertyBag);

		void OnCollectionDisposed(AttachmentPropertyBag attachmentBag, PersistablePropertyBag persistablePropertyBag);

		NativeStorePropertyDefinition[] AttachmentTablePropertyList { get; }
	}
}
