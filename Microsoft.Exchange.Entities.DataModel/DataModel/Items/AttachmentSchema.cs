using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public abstract class AttachmentSchema : EntitySchema
	{
		protected AttachmentSchema()
		{
			base.RegisterPropertyDefinition(AttachmentSchema.StaticContentTypeProperty);
			base.RegisterPropertyDefinition(AttachmentSchema.StaticIsInlineProperty);
			base.RegisterPropertyDefinition(AttachmentSchema.StaticLastModifiedTimeProperty);
			base.RegisterPropertyDefinition(AttachmentSchema.StaticNameProperty);
			base.RegisterPropertyDefinition(AttachmentSchema.StaticSizeProperty);
		}

		public TypedPropertyDefinition<string> ContentTypeProperty
		{
			get
			{
				return AttachmentSchema.StaticContentTypeProperty;
			}
		}

		public TypedPropertyDefinition<bool> IsInlineProperty
		{
			get
			{
				return AttachmentSchema.StaticIsInlineProperty;
			}
		}

		public TypedPropertyDefinition<ExDateTime> LastModifiedTimeProperty
		{
			get
			{
				return AttachmentSchema.StaticLastModifiedTimeProperty;
			}
		}

		public TypedPropertyDefinition<long> SizeProperty
		{
			get
			{
				return AttachmentSchema.StaticSizeProperty;
			}
		}

		public TypedPropertyDefinition<string> NameProperty
		{
			get
			{
				return AttachmentSchema.StaticNameProperty;
			}
		}

		private static readonly TypedPropertyDefinition<string> StaticContentTypeProperty = new TypedPropertyDefinition<string>("Attachment.ContentType", null, true);

		private static readonly TypedPropertyDefinition<bool> StaticIsInlineProperty = new TypedPropertyDefinition<bool>("Attachment.IsInline", false, true);

		private static readonly TypedPropertyDefinition<ExDateTime> StaticLastModifiedTimeProperty = new TypedPropertyDefinition<ExDateTime>("Attachment.LastModifiedTime", default(ExDateTime), true);

		private static readonly TypedPropertyDefinition<string> StaticNameProperty = new TypedPropertyDefinition<string>("Attachment.Name", null, true);

		private static readonly TypedPropertyDefinition<long> StaticSizeProperty = new TypedPropertyDefinition<long>("Attachment.Size", 0L, true);
	}
}
