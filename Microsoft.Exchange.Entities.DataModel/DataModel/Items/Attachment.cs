using System;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public abstract class Attachment<TSchema> : Entity<TSchema>, IAttachment, IEntity, IPropertyChangeTracker<PropertyDefinition> where TSchema : AttachmentSchema, new()
	{
		public string ContentType
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<string>(schema.ContentTypeProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<string>(schema.ContentTypeProperty, value);
			}
		}

		public bool IsInline
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<bool>(schema.IsInlineProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<bool>(schema.IsInlineProperty, value);
			}
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<ExDateTime>(schema.LastModifiedTimeProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<ExDateTime>(schema.LastModifiedTimeProperty, value);
			}
		}

		public string Name
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<string>(schema.NameProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<string>(schema.NameProperty, value);
			}
		}

		public long Size
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<long>(schema.SizeProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<long>(schema.SizeProperty, value);
			}
		}

		public new static class Accessors
		{
			// Note: this type is marked as 'beforefieldinit'.
			static Accessors()
			{
				TSchema schemaInstance = SchematizedObject<TSchema>.SchemaInstance;
				Attachment<TSchema>.Accessors.ContentType = new EntityPropertyAccessor<Attachment<TSchema>, string>(schemaInstance.ContentTypeProperty, (Attachment<TSchema> attachment) => attachment.ContentType, delegate(Attachment<TSchema> attachment, string contentType)
				{
					attachment.ContentType = contentType;
				});
				TSchema schemaInstance2 = SchematizedObject<TSchema>.SchemaInstance;
				Attachment<TSchema>.Accessors.IsInline = new EntityPropertyAccessor<Attachment<TSchema>, bool>(schemaInstance2.IsInlineProperty, (Attachment<TSchema> attachment) => attachment.IsInline, delegate(Attachment<TSchema> attachment, bool isInline)
				{
					attachment.IsInline = isInline;
				});
				TSchema schemaInstance3 = SchematizedObject<TSchema>.SchemaInstance;
				Attachment<TSchema>.Accessors.LastModifiedTime = new EntityPropertyAccessor<Attachment<TSchema>, ExDateTime>(schemaInstance3.LastModifiedTimeProperty, (Attachment<TSchema> attachment) => attachment.LastModifiedTime, delegate(Attachment<TSchema> attachment, ExDateTime time)
				{
					attachment.LastModifiedTime = time;
				});
				TSchema schemaInstance4 = SchematizedObject<TSchema>.SchemaInstance;
				Attachment<TSchema>.Accessors.Name = new EntityPropertyAccessor<Attachment<TSchema>, string>(schemaInstance4.NameProperty, (Attachment<TSchema> attachment) => attachment.Name, delegate(Attachment<TSchema> attachment, string name)
				{
					attachment.Name = name;
				});
				TSchema schemaInstance5 = SchematizedObject<TSchema>.SchemaInstance;
				Attachment<TSchema>.Accessors.Size = new EntityPropertyAccessor<Attachment<TSchema>, long>(schemaInstance5.SizeProperty, (Attachment<TSchema> attachment) => attachment.Size, delegate(Attachment<TSchema> attachment, long size)
				{
					attachment.Size = size;
				});
			}

			public static readonly EntityPropertyAccessor<Attachment<TSchema>, string> ContentType;

			public static readonly EntityPropertyAccessor<Attachment<TSchema>, bool> IsInline;

			public static readonly EntityPropertyAccessor<Attachment<TSchema>, ExDateTime> LastModifiedTime;

			public static readonly EntityPropertyAccessor<Attachment<TSchema>, string> Name;

			public static readonly EntityPropertyAccessor<Attachment<TSchema>, long> Size;
		}
	}
}
