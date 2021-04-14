using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules
{
	internal class EmbededItemTranslationRule : ITranslationRule<ItemAttachment, ItemAttachment>
	{
		public void FromLeftToRightType(ItemAttachment left, ItemAttachment right)
		{
			using (Item item = left.GetItem())
			{
				right.Item = StorageEntityFactory.CreateFromItem(item);
				right.Item.Id = null;
				this.UpdateNestedAttachmentIds(right.Item.Attachments, right.Id);
			}
		}

		public void FromRightToLeftType(ItemAttachment left, ItemAttachment right)
		{
		}

		private void UpdateNestedAttachmentIds(List<IAttachment> attachments, string embededItemAttachmentId)
		{
			if (attachments.Count > 0)
			{
				IList<AttachmentId> attachmentIds = IdConverter.GetAttachmentIds(embededItemAttachmentId);
				int count = attachmentIds.Count;
				foreach (IAttachment attachment in attachments)
				{
					IList<AttachmentId> attachmentIds2 = IdConverter.GetAttachmentIds(attachment.Id);
					attachmentIds.Add(attachmentIds2[0]);
					attachment.Id = IdConverter.GetHierarchicalAttachmentStringId(attachmentIds);
					attachmentIds.RemoveAt(count);
				}
			}
		}
	}
}
