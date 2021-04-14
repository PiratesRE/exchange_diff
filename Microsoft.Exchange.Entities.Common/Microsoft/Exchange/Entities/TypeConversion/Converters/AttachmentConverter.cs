using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.TypeConversion.Converters
{
	internal class AttachmentConverter : IConverter<IAttachment, IAttachment>
	{
		public IAttachment Convert(IAttachment storageAttachment)
		{
			switch (storageAttachment.AttachmentType)
			{
			case AttachmentType.EmbeddedMessage:
				return AttachmentTranslator<ItemAttachment, ItemAttachmentSchema>.MetadataInstance.ConvertToEntity(storageAttachment);
			case AttachmentType.Reference:
				return AttachmentTranslator<ReferenceAttachment, ReferenceAttachmentSchema>.MetadataInstance.ConvertToEntity(storageAttachment);
			}
			return AttachmentTranslator<FileAttachment, FileAttachmentSchema>.MetadataInstance.ConvertToEntity(storageAttachment);
		}
	}
}
