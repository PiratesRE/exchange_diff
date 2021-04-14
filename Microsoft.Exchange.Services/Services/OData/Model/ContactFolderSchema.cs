using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ContactFolderSchema : EntitySchema
	{
		public new static ContactFolderSchema SchemaInstance
		{
			get
			{
				return ContactFolderSchema.ContactFolderSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return ContactFolder.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return ContactFolderSchema.DeclaredContactFolderProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return ContactFolderSchema.AllContactFolderProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return ContactFolderSchema.DefaultContactFolderProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> MandatoryCreationProperties
		{
			get
			{
				return ContactFolderSchema.MandatoryContactFolderCreationProperties;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ContactFolderSchema()
		{
			PropertyDefinition propertyDefinition = new PropertyDefinition("ParentFolderId", typeof(string));
			propertyDefinition.EdmType = EdmCoreModel.Instance.GetString(true);
			PropertyDefinition propertyDefinition2 = propertyDefinition;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider = new SimpleEwsPropertyProvider(BaseFolderSchema.ParentFolderId);
			simpleEwsPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e[ep] = EwsIdConverter.EwsIdToODataId((s[sp] as FolderId).Id);
			};
			propertyDefinition2.EwsPropertyProvider = simpleEwsPropertyProvider;
			ContactFolderSchema.ParentFolderId = propertyDefinition;
			ContactFolderSchema.DisplayName = new PropertyDefinition("DisplayName", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(BaseFolderSchema.DisplayName)
				{
					SetPropertyUpdateCreator = EwsPropertyProvider.SetFolderPropertyUpdateDelegate,
					DeletePropertyUpdateCreator = EwsPropertyProvider.DeleteFolderPropertyUpdateDelegate
				}
			};
			ContactFolderSchema.Contacts = new PropertyDefinition("Contacts", typeof(IEnumerable<Contact>))
			{
				Flags = PropertyDefinitionFlags.Navigation,
				NavigationTargetEntity = Contact.EdmEntityType
			};
			ContactFolderSchema.ChildFolders = new PropertyDefinition("ChildFolders", typeof(IEnumerable<ContactFolder>))
			{
				Flags = PropertyDefinitionFlags.Navigation,
				NavigationTargetEntity = ContactFolder.EdmEntityType
			};
			ContactFolderSchema.DeclaredContactFolderProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				ContactFolderSchema.ParentFolderId,
				ContactFolderSchema.DisplayName,
				ContactFolderSchema.Contacts,
				ContactFolderSchema.ChildFolders
			});
			ContactFolderSchema.AllContactFolderProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.AllEntityProperties.Union(ContactFolderSchema.DeclaredContactFolderProperties)));
			ContactFolderSchema.DefaultContactFolderProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.DefaultEntityProperties)
			{
				ContactFolderSchema.ParentFolderId,
				ContactFolderSchema.DisplayName
			});
			ContactFolderSchema.MandatoryContactFolderCreationProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				FolderSchema.DisplayName
			});
			ContactFolderSchema.ContactFolderSchemaInstance = new LazyMember<ContactFolderSchema>(() => new ContactFolderSchema());
		}

		public static readonly PropertyDefinition ParentFolderId;

		public static readonly PropertyDefinition DisplayName;

		public static readonly PropertyDefinition Contacts;

		public static readonly PropertyDefinition ChildFolders;

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredContactFolderProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> AllContactFolderProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultContactFolderProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> MandatoryContactFolderCreationProperties;

		private static readonly LazyMember<ContactFolderSchema> ContactFolderSchemaInstance;
	}
}
