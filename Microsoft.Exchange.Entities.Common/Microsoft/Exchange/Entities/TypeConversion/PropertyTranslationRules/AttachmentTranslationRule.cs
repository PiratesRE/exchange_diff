using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules
{
	internal class AttachmentTranslationRule<TStorageItem, TItem, TSchema> : ITranslationRule<TStorageItem, TItem> where TStorageItem : IItem where TItem : Item<TSchema> where TSchema : ItemSchema, new()
	{
		public AttachmentTranslationRule()
		{
			this.AttachmentConverter = new AttachmentConverter();
		}

		public IConverter<Attachment, IAttachment> AttachmentConverter { get; private set; }

		public void FromLeftToRightType(TStorageItem left, TItem right)
		{
			List<IAttachment> list = new List<IAttachment>();
			AttachmentCollection attachmentCollection = IrmUtils.GetAttachmentCollection(left);
			IList<AttachmentHandle> handles = attachmentCollection.GetHandles();
			foreach (AttachmentHandle handle in handles)
			{
				using (Attachment attachment = attachmentCollection.Open(handle))
				{
					IAttachment item = this.AttachmentConverter.Convert(attachment);
					list.Add(item);
				}
			}
			right.Attachments = list;
		}

		public void FromRightToLeftType(TStorageItem left, TItem right)
		{
		}
	}
}
