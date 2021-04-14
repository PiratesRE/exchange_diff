using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class AttachmentSchema : EntitySchema
	{
		public new static AttachmentSchema SchemaInstance
		{
			get
			{
				return AttachmentSchema.AttachmentSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return Attachment.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return AttachmentSchema.DeclaredAttachmentProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return AttachmentSchema.AllAttachmentProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return AttachmentSchema.DefaultAttachmentProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> MandatoryCreationProperties
		{
			get
			{
				return AttachmentSchema.MandatoryAttachmentCreationProperties;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static AttachmentSchema()
		{
			PropertyDefinition propertyDefinition = new PropertyDefinition("Name", typeof(string));
			propertyDefinition.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate);
			PropertyDefinition propertyDefinition2 = propertyDefinition;
			GenericPropertyProvider<AttachmentType> genericPropertyProvider = new GenericPropertyProvider<AttachmentType>();
			genericPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, AttachmentType a)
			{
				e[ep] = a.Name;
			};
			genericPropertyProvider.Setter = delegate(Entity e, PropertyDefinition ep, AttachmentType a)
			{
				a.Name = (string)e[ep];
			};
			propertyDefinition2.EwsPropertyProvider = genericPropertyProvider;
			AttachmentSchema.Name = propertyDefinition;
			PropertyDefinition propertyDefinition3 = new PropertyDefinition("ContentType", typeof(string));
			propertyDefinition3.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition3.Flags = PropertyDefinitionFlags.CanFilter;
			PropertyDefinition propertyDefinition4 = propertyDefinition3;
			GenericPropertyProvider<AttachmentType> genericPropertyProvider2 = new GenericPropertyProvider<AttachmentType>();
			genericPropertyProvider2.Getter = delegate(Entity e, PropertyDefinition ep, AttachmentType a)
			{
				e[ep] = a.ContentType;
			};
			genericPropertyProvider2.Setter = delegate(Entity e, PropertyDefinition ep, AttachmentType a)
			{
				a.ContentType = (string)e[ep];
			};
			propertyDefinition4.EwsPropertyProvider = genericPropertyProvider2;
			AttachmentSchema.ContentType = propertyDefinition3;
			PropertyDefinition propertyDefinition5 = new PropertyDefinition("Size", typeof(int));
			propertyDefinition5.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate);
			propertyDefinition5.EdmType = EdmCoreModel.Instance.GetInt32(false);
			PropertyDefinition propertyDefinition6 = propertyDefinition5;
			GenericPropertyProvider<AttachmentType> genericPropertyProvider3 = new GenericPropertyProvider<AttachmentType>();
			genericPropertyProvider3.Getter = delegate(Entity e, PropertyDefinition ep, AttachmentType a)
			{
				e[ep] = a.Size;
			};
			genericPropertyProvider3.Setter = delegate(Entity e, PropertyDefinition ep, AttachmentType a)
			{
				a.Size = (int)e[ep];
			};
			propertyDefinition6.EwsPropertyProvider = genericPropertyProvider3;
			AttachmentSchema.Size = propertyDefinition5;
			PropertyDefinition propertyDefinition7 = new PropertyDefinition("IsInline", typeof(bool));
			propertyDefinition7.EdmType = EdmCoreModel.Instance.GetBoolean(false);
			propertyDefinition7.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate);
			PropertyDefinition propertyDefinition8 = propertyDefinition7;
			GenericPropertyProvider<AttachmentType> genericPropertyProvider4 = new GenericPropertyProvider<AttachmentType>();
			genericPropertyProvider4.Getter = delegate(Entity e, PropertyDefinition ep, AttachmentType a)
			{
				e[ep] = a.IsInline;
			};
			genericPropertyProvider4.Setter = delegate(Entity e, PropertyDefinition ep, AttachmentType a)
			{
				a.IsInline = (bool)e[ep];
			};
			propertyDefinition8.EwsPropertyProvider = genericPropertyProvider4;
			AttachmentSchema.IsInline = propertyDefinition7;
			PropertyDefinition propertyDefinition9 = new PropertyDefinition("LastModifiedTime", typeof(DateTimeOffset));
			propertyDefinition9.EdmType = EdmCoreModel.Instance.GetDateTimeOffset(true);
			propertyDefinition9.Flags = PropertyDefinitionFlags.CanFilter;
			PropertyDefinition propertyDefinition10 = propertyDefinition9;
			GenericPropertyProvider<AttachmentType> genericPropertyProvider5 = new GenericPropertyProvider<AttachmentType>();
			genericPropertyProvider5.Getter = delegate(Entity e, PropertyDefinition ep, AttachmentType a)
			{
				e[ep] = new DateTimeOffset(DateTime.Parse(a.LastModifiedTime));
			};
			propertyDefinition10.EwsPropertyProvider = genericPropertyProvider5;
			AttachmentSchema.LastModifiedTime = propertyDefinition9;
			AttachmentSchema.DeclaredAttachmentProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				AttachmentSchema.Name,
				AttachmentSchema.ContentType,
				AttachmentSchema.Size,
				AttachmentSchema.IsInline,
				AttachmentSchema.LastModifiedTime
			});
			AttachmentSchema.AllAttachmentProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.AllEntityProperties.Union(AttachmentSchema.DeclaredAttachmentProperties)));
			AttachmentSchema.DefaultAttachmentProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.DefaultEntityProperties)
			{
				AttachmentSchema.Name,
				AttachmentSchema.ContentType,
				AttachmentSchema.Size,
				AttachmentSchema.IsInline,
				AttachmentSchema.LastModifiedTime
			});
			AttachmentSchema.MandatoryAttachmentCreationProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				AttachmentSchema.Name
			});
			AttachmentSchema.AttachmentSchemaInstance = new LazyMember<AttachmentSchema>(() => new AttachmentSchema());
		}

		public static readonly PropertyDefinition Name;

		public static readonly PropertyDefinition ContentType;

		public static readonly PropertyDefinition Size;

		public static readonly PropertyDefinition IsInline;

		public static readonly PropertyDefinition LastModifiedTime;

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredAttachmentProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> AllAttachmentProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultAttachmentProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> MandatoryAttachmentCreationProperties;

		private static readonly LazyMember<AttachmentSchema> AttachmentSchemaInstance;
	}
}
