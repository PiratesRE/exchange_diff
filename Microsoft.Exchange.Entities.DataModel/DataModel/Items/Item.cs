using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Items
{
	public class Item<TSchema> : StorageEntity<TSchema>, IItem, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned where TSchema : ItemSchema, new()
	{
		[NotMapped]
		public List<IAttachment> Attachments
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<List<IAttachment>>(schema.AttachmentsProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<List<IAttachment>>(schema.AttachmentsProperty, value);
			}
		}

		public ItemBody Body
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<ItemBody>(schema.BodyProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<ItemBody>(schema.BodyProperty, value);
			}
		}

		public List<string> Categories
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<List<string>>(schema.CategoriesProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<List<string>>(schema.CategoriesProperty, value);
			}
		}

		public ExDateTime DateTimeCreated
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<ExDateTime>(schema.DateTimeCreatedProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<ExDateTime>(schema.DateTimeCreatedProperty, value);
			}
		}

		public bool HasAttachments
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<bool>(schema.HasAttachmentsProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<bool>(schema.HasAttachmentsProperty, value);
			}
		}

		public Importance Importance
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<Importance>(schema.ImportanceProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<Importance>(schema.ImportanceProperty, value);
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

		public string Preview
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<string>(schema.PreviewProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<string>(schema.PreviewProperty, value);
			}
		}

		public ExDateTime ReceivedTime
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<ExDateTime>(schema.ReceivedTimeProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<ExDateTime>(schema.ReceivedTimeProperty, value);
			}
		}

		public Sensitivity Sensitivity
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<Sensitivity>(schema.SensitivityProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<Sensitivity>(schema.SensitivityProperty, value);
			}
		}

		public string Subject
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<string>(schema.SubjectProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<string>(schema.SubjectProperty, value);
			}
		}

		public new static class Accessors
		{
			// Note: this type is marked as 'beforefieldinit'.
			static Accessors()
			{
				TSchema schemaInstance = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.Attachments = new EntityPropertyAccessor<IItem, List<IAttachment>>(schemaInstance.AttachmentsProperty, (IItem item) => item.Attachments, delegate(IItem item, List<IAttachment> list)
				{
					item.Attachments = list;
				});
				TSchema schemaInstance2 = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.Body = new EntityPropertyAccessor<IItem, ItemBody>(schemaInstance2.BodyProperty, (IItem item) => item.Body, delegate(IItem item, ItemBody body)
				{
					item.Body = body;
				});
				TSchema schemaInstance3 = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.Categories = new EntityPropertyAccessor<IItem, List<string>>(schemaInstance3.CategoriesProperty, (IItem item) => item.Categories, delegate(IItem item, List<string> list)
				{
					item.Categories = list;
				});
				TSchema schemaInstance4 = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.DateTimeCreated = new EntityPropertyAccessor<IItem, ExDateTime>(schemaInstance4.DateTimeCreatedProperty, (IItem item) => item.DateTimeCreated, delegate(IItem item, ExDateTime time)
				{
					item.DateTimeCreated = time;
				});
				TSchema schemaInstance5 = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.HasAttachments = new EntityPropertyAccessor<IItem, bool>(schemaInstance5.HasAttachmentsProperty, (IItem item) => item.HasAttachments, delegate(IItem item, bool b)
				{
					item.HasAttachments = b;
				});
				TSchema schemaInstance6 = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.Importance = new EntityPropertyAccessor<IItem, Importance>(schemaInstance6.ImportanceProperty, (IItem item) => item.Importance, delegate(IItem item, Importance importance)
				{
					item.Importance = importance;
				});
				TSchema schemaInstance7 = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.LastModifiedTime = new EntityPropertyAccessor<IItem, ExDateTime>(schemaInstance7.LastModifiedTimeProperty, (IItem item) => item.LastModifiedTime, delegate(IItem item, ExDateTime time)
				{
					item.LastModifiedTime = time;
				});
				TSchema schemaInstance8 = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.Preview = new EntityPropertyAccessor<IItem, string>(schemaInstance8.PreviewProperty, (IItem item) => item.Preview, delegate(IItem item, string s)
				{
					item.Preview = s;
				});
				TSchema schemaInstance9 = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.ReceivedTime = new EntityPropertyAccessor<IItem, ExDateTime>(schemaInstance9.ReceivedTimeProperty, (IItem item) => item.ReceivedTime, delegate(IItem item, ExDateTime time)
				{
					item.ReceivedTime = time;
				});
				TSchema schemaInstance10 = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.Sensitivity = new EntityPropertyAccessor<IItem, Sensitivity>(schemaInstance10.SensitivityProperty, (IItem item) => item.Sensitivity, delegate(IItem item, Sensitivity sensitivity)
				{
					item.Sensitivity = sensitivity;
				});
				TSchema schemaInstance11 = SchematizedObject<TSchema>.SchemaInstance;
				Item<TSchema>.Accessors.Subject = new EntityPropertyAccessor<IItem, string>(schemaInstance11.SubjectProperty, (IItem item) => item.Subject, delegate(IItem item, string s)
				{
					item.Subject = s;
				});
			}

			public static readonly EntityPropertyAccessor<IItem, List<IAttachment>> Attachments;

			public static readonly EntityPropertyAccessor<IItem, ItemBody> Body;

			public static readonly EntityPropertyAccessor<IItem, List<string>> Categories;

			public static readonly EntityPropertyAccessor<IItem, ExDateTime> DateTimeCreated;

			public static readonly EntityPropertyAccessor<IItem, bool> HasAttachments;

			public static readonly EntityPropertyAccessor<IItem, Importance> Importance;

			public static readonly EntityPropertyAccessor<IItem, ExDateTime> LastModifiedTime;

			public static readonly EntityPropertyAccessor<IItem, string> Preview;

			public static readonly EntityPropertyAccessor<IItem, ExDateTime> ReceivedTime;

			public static readonly EntityPropertyAccessor<IItem, Sensitivity> Sensitivity;

			public static readonly EntityPropertyAccessor<IItem, string> Subject;
		}
	}
}
