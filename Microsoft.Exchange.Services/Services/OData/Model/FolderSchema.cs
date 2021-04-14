using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class FolderSchema : EntitySchema
	{
		public new static FolderSchema SchemaInstance
		{
			get
			{
				return FolderSchema.FolderSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return Folder.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return FolderSchema.DeclaredFolderProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return FolderSchema.AllFolderProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return FolderSchema.DefaultFolderProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> MandatoryCreationProperties
		{
			get
			{
				return FolderSchema.MandatoryFolderCreationProperties;
			}
		}

		public override void RegisterEdmModel(EdmModel model)
		{
			base.RegisterEdmModel(model);
			CustomActions.RegisterAction(model, Folder.EdmEntityType, Folder.EdmEntityType, "Copy", new Dictionary<string, IEdmTypeReference>
			{
				{
					"DestinationId",
					EdmCoreModel.Instance.GetString(true)
				}
			});
			CustomActions.RegisterAction(model, Folder.EdmEntityType, Folder.EdmEntityType, "Move", new Dictionary<string, IEdmTypeReference>
			{
				{
					"DestinationId",
					EdmCoreModel.Instance.GetString(true)
				}
			});
		}

		// Note: this type is marked as 'beforefieldinit'.
		static FolderSchema()
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
			FolderSchema.ParentFolderId = propertyDefinition;
			PropertyDefinition propertyDefinition3 = new PropertyDefinition("UnreadItemCount", typeof(int));
			propertyDefinition3.EdmType = EdmCoreModel.Instance.GetInt32(true);
			propertyDefinition3.Flags = PropertyDefinitionFlags.CanFilter;
			PropertyDefinition propertyDefinition4 = propertyDefinition3;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider2 = new SimpleEwsPropertyProvider(FolderSchema.UnreadCount);
			simpleEwsPropertyProvider2.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e[ep] = (s[sp] ?? 0);
			};
			propertyDefinition4.EwsPropertyProvider = simpleEwsPropertyProvider2;
			FolderSchema.UnreadItemCount = propertyDefinition3;
			PropertyDefinition propertyDefinition5 = new PropertyDefinition("TotalCount", typeof(int));
			propertyDefinition5.EdmType = EdmCoreModel.Instance.GetInt32(true);
			propertyDefinition5.Flags = PropertyDefinitionFlags.CanFilter;
			PropertyDefinition propertyDefinition6 = propertyDefinition5;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider3 = new SimpleEwsPropertyProvider(BaseFolderSchema.TotalCount);
			simpleEwsPropertyProvider3.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e[ep] = (s[sp] ?? 0);
			};
			propertyDefinition6.EwsPropertyProvider = simpleEwsPropertyProvider3;
			FolderSchema.TotalCount = propertyDefinition5;
			PropertyDefinition propertyDefinition7 = new PropertyDefinition("ChildFolderCount", typeof(int));
			propertyDefinition7.EdmType = EdmCoreModel.Instance.GetInt32(true);
			propertyDefinition7.Flags = PropertyDefinitionFlags.CanFilter;
			PropertyDefinition propertyDefinition8 = propertyDefinition7;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider4 = new SimpleEwsPropertyProvider(BaseFolderSchema.ChildFolderCount);
			simpleEwsPropertyProvider4.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e[ep] = (s[sp] ?? 0);
			};
			propertyDefinition8.EwsPropertyProvider = simpleEwsPropertyProvider4;
			FolderSchema.ChildFolderCount = propertyDefinition7;
			FolderSchema.ChildFolders = new PropertyDefinition("ChildFolders", typeof(IEnumerable<Folder>))
			{
				Flags = PropertyDefinitionFlags.Navigation,
				NavigationTargetEntity = Folder.EdmEntityType
			};
			FolderSchema.Messages = new PropertyDefinition("Messages", typeof(IEnumerable<Message>))
			{
				Flags = PropertyDefinitionFlags.Navigation,
				NavigationTargetEntity = Message.EdmEntityType
			};
			FolderSchema.DeclaredFolderProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				FolderSchema.ParentFolderId,
				FolderSchema.DisplayName,
				FolderSchema.ClassName,
				FolderSchema.TotalCount,
				FolderSchema.ChildFolderCount,
				FolderSchema.UnreadItemCount,
				FolderSchema.ChildFolders,
				FolderSchema.Messages
			});
			FolderSchema.AllFolderProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.AllEntityProperties.Union(FolderSchema.DeclaredFolderProperties)));
			FolderSchema.DefaultFolderProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.DefaultEntityProperties)
			{
				FolderSchema.ParentFolderId,
				FolderSchema.DisplayName,
				FolderSchema.ClassName,
				FolderSchema.TotalCount,
				FolderSchema.ChildFolderCount,
				FolderSchema.UnreadItemCount
			});
			FolderSchema.MandatoryFolderCreationProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				FolderSchema.DisplayName
			});
			FolderSchema.FolderSchemaInstance = new LazyMember<FolderSchema>(() => new FolderSchema());
		}

		public static readonly PropertyDefinition DisplayName = new PropertyDefinition("DisplayName", typeof(string))
		{
			EdmType = EdmCoreModel.Instance.GetString(true),
			Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
			EwsPropertyProvider = new SimpleEwsPropertyProvider(BaseFolderSchema.DisplayName)
			{
				SetPropertyUpdateCreator = EwsPropertyProvider.SetFolderPropertyUpdateDelegate,
				DeletePropertyUpdateCreator = EwsPropertyProvider.DeleteFolderPropertyUpdateDelegate
			}
		};

		public static readonly PropertyDefinition ClassName = new PropertyDefinition("ClassName", typeof(string))
		{
			EdmType = EdmCoreModel.Instance.GetString(true),
			Flags = PropertyDefinitionFlags.CanFilter,
			EwsPropertyProvider = new SimpleEwsPropertyProvider(BaseFolderSchema.FolderClass)
			{
				SetPropertyUpdateCreator = EwsPropertyProvider.SetFolderPropertyUpdateDelegate,
				DeletePropertyUpdateCreator = EwsPropertyProvider.DeleteFolderPropertyUpdateDelegate
			}
		};

		public static readonly PropertyDefinition ParentFolderId;

		public static readonly PropertyDefinition UnreadItemCount;

		public static readonly PropertyDefinition TotalCount;

		public static readonly PropertyDefinition ChildFolderCount;

		public static readonly PropertyDefinition ChildFolders;

		public static readonly PropertyDefinition Messages;

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredFolderProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> AllFolderProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultFolderProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> MandatoryFolderCreationProperties;

		private static readonly LazyMember<FolderSchema> FolderSchemaInstance;
	}
}
