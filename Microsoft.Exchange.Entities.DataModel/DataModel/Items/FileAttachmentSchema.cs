using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public sealed class FileAttachmentSchema : AttachmentSchema
	{
		public FileAttachmentSchema()
		{
			base.RegisterPropertyDefinition(FileAttachmentSchema.StaticContentProperty);
			base.RegisterPropertyDefinition(FileAttachmentSchema.StaticContentIdProperty);
			base.RegisterPropertyDefinition(FileAttachmentSchema.StaticContentLocationProperty);
		}

		public TypedPropertyDefinition<byte[]> ContentProperty
		{
			get
			{
				return FileAttachmentSchema.StaticContentProperty;
			}
		}

		public TypedPropertyDefinition<string> ContentIdProperty
		{
			get
			{
				return FileAttachmentSchema.StaticContentIdProperty;
			}
		}

		public TypedPropertyDefinition<string> ContentLocationProperty
		{
			get
			{
				return FileAttachmentSchema.StaticContentLocationProperty;
			}
		}

		private static readonly TypedPropertyDefinition<byte[]> StaticContentProperty = new TypedPropertyDefinition<byte[]>("FileAttachment.Content", null, false);

		private static readonly TypedPropertyDefinition<string> StaticContentIdProperty = new TypedPropertyDefinition<string>("FileAttachment.ContentId", null, true);

		private static readonly TypedPropertyDefinition<string> StaticContentLocationProperty = new TypedPropertyDefinition<string>("FileAttachment.ContentLocation", null, true);
	}
}
