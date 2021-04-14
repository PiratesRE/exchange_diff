using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public class FileAttachment : Attachment<FileAttachmentSchema>, IFileAttachment, IAttachment, IEntity, IPropertyChangeTracker<PropertyDefinition>
	{
		public byte[] Content
		{
			get
			{
				return base.GetPropertyValueOrDefault<byte[]>(base.Schema.ContentProperty);
			}
			set
			{
				base.SetPropertyValue<byte[]>(base.Schema.ContentProperty, value);
			}
		}

		public string ContentId
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.ContentIdProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.ContentIdProperty, value);
			}
		}

		public string ContentLocation
		{
			get
			{
				return base.GetPropertyValueOrDefault<string>(base.Schema.ContentLocationProperty);
			}
			set
			{
				base.SetPropertyValue<string>(base.Schema.ContentLocationProperty, value);
			}
		}

		public new static class Accessors
		{
			public static readonly EntityPropertyAccessor<FileAttachment, byte[]> Content = new EntityPropertyAccessor<FileAttachment, byte[]>(SchematizedObject<FileAttachmentSchema>.SchemaInstance.ContentProperty, (FileAttachment fileAttachment) => fileAttachment.Content, delegate(FileAttachment fileAttachment, byte[] content)
			{
				fileAttachment.Content = content;
			});

			public static readonly EntityPropertyAccessor<FileAttachment, string> ContentId = new EntityPropertyAccessor<FileAttachment, string>(SchematizedObject<FileAttachmentSchema>.SchemaInstance.ContentIdProperty, (FileAttachment fileAttachment) => fileAttachment.ContentId, delegate(FileAttachment fileAttachment, string contentId)
			{
				fileAttachment.ContentId = contentId;
			});

			public static readonly EntityPropertyAccessor<FileAttachment, string> ContentLocation = new EntityPropertyAccessor<FileAttachment, string>(SchematizedObject<FileAttachmentSchema>.SchemaInstance.ContentLocationProperty, (FileAttachment fileAttachment) => fileAttachment.ContentLocation, delegate(FileAttachment fileAttachment, string contentLocation)
			{
				fileAttachment.ContentLocation = contentLocation;
			});
		}
	}
}
