using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FileAttachmentSchema : AttachmentSchema
	{
		public new static FileAttachmentSchema SchemaInstance
		{
			get
			{
				return FileAttachmentSchema.FileAttachmentSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return FileAttachment.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return FileAttachmentSchema.DeclaredFileAttachmentProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return FileAttachmentSchema.AllFileAttachmentProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return FileAttachmentSchema.DefaultFileAttachmentProperties;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static FileAttachmentSchema()
		{
			PropertyDefinition propertyDefinition = new PropertyDefinition("ContentId", typeof(string));
			propertyDefinition.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate);
			propertyDefinition.EdmType = EdmCoreModel.Instance.GetString(true);
			PropertyDefinition propertyDefinition2 = propertyDefinition;
			GenericPropertyProvider<FileAttachmentType> genericPropertyProvider = new GenericPropertyProvider<FileAttachmentType>();
			genericPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, FileAttachmentType a)
			{
				e[ep] = a.ContentId;
			};
			genericPropertyProvider.Setter = delegate(Entity e, PropertyDefinition ep, FileAttachmentType a)
			{
				a.ContentId = (string)e[ep];
			};
			propertyDefinition2.EwsPropertyProvider = genericPropertyProvider;
			FileAttachmentSchema.ContentId = propertyDefinition;
			PropertyDefinition propertyDefinition3 = new PropertyDefinition("ContentLocation", typeof(string));
			propertyDefinition3.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate);
			propertyDefinition3.EdmType = EdmCoreModel.Instance.GetString(true);
			PropertyDefinition propertyDefinition4 = propertyDefinition3;
			GenericPropertyProvider<FileAttachmentType> genericPropertyProvider2 = new GenericPropertyProvider<FileAttachmentType>();
			genericPropertyProvider2.Getter = delegate(Entity e, PropertyDefinition ep, FileAttachmentType a)
			{
				e[ep] = a.ContentLocation;
			};
			genericPropertyProvider2.Setter = delegate(Entity e, PropertyDefinition ep, FileAttachmentType a)
			{
				a.ContentLocation = (string)e[ep];
			};
			propertyDefinition4.EwsPropertyProvider = genericPropertyProvider2;
			FileAttachmentSchema.ContentLocation = propertyDefinition3;
			PropertyDefinition propertyDefinition5 = new PropertyDefinition("ContentBytes", typeof(byte[]));
			propertyDefinition5.Flags = PropertyDefinitionFlags.CanCreate;
			propertyDefinition5.EdmType = EdmCoreModel.Instance.GetBinary(true);
			PropertyDefinition propertyDefinition6 = propertyDefinition5;
			GenericPropertyProvider<FileAttachmentType> genericPropertyProvider3 = new GenericPropertyProvider<FileAttachmentType>();
			genericPropertyProvider3.Getter = delegate(Entity e, PropertyDefinition ep, FileAttachmentType a)
			{
				e[ep] = a.Content;
			};
			genericPropertyProvider3.Setter = delegate(Entity e, PropertyDefinition ep, FileAttachmentType a)
			{
				a.Content = (byte[])e[ep];
			};
			propertyDefinition6.EwsPropertyProvider = genericPropertyProvider3;
			FileAttachmentSchema.ContentBytes = propertyDefinition5;
			PropertyDefinition propertyDefinition7 = new PropertyDefinition("IsContactPhoto", typeof(bool));
			propertyDefinition7.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate);
			propertyDefinition7.EdmType = EdmCoreModel.Instance.GetBoolean(false);
			PropertyDefinition propertyDefinition8 = propertyDefinition7;
			GenericPropertyProvider<FileAttachmentType> genericPropertyProvider4 = new GenericPropertyProvider<FileAttachmentType>();
			genericPropertyProvider4.Getter = delegate(Entity e, PropertyDefinition ep, FileAttachmentType a)
			{
				e[ep] = a.IsContactPhoto;
			};
			genericPropertyProvider4.Setter = delegate(Entity e, PropertyDefinition ep, FileAttachmentType a)
			{
				a.IsContactPhoto = (bool)e[ep];
			};
			propertyDefinition8.EwsPropertyProvider = genericPropertyProvider4;
			FileAttachmentSchema.IsContactPhoto = propertyDefinition7;
			FileAttachmentSchema.DeclaredFileAttachmentProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				FileAttachmentSchema.ContentId,
				FileAttachmentSchema.ContentLocation,
				FileAttachmentSchema.IsContactPhoto,
				FileAttachmentSchema.ContentBytes
			});
			FileAttachmentSchema.AllFileAttachmentProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(AttachmentSchema.AllAttachmentProperties.Union(FileAttachmentSchema.DeclaredFileAttachmentProperties)));
			FileAttachmentSchema.DefaultFileAttachmentProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(AttachmentSchema.DefaultAttachmentProperties)
			{
				FileAttachmentSchema.ContentId,
				FileAttachmentSchema.ContentLocation,
				FileAttachmentSchema.IsContactPhoto,
				FileAttachmentSchema.ContentBytes
			});
			FileAttachmentSchema.FileAttachmentSchemaInstance = new LazyMember<FileAttachmentSchema>(() => new FileAttachmentSchema());
		}

		public static readonly PropertyDefinition ContentId;

		public static readonly PropertyDefinition ContentLocation;

		public static readonly PropertyDefinition ContentBytes;

		public static readonly PropertyDefinition IsContactPhoto;

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredFileAttachmentProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> AllFileAttachmentProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultFileAttachmentProperties;

		private static readonly LazyMember<FileAttachmentSchema> FileAttachmentSchemaInstance;
	}
}
