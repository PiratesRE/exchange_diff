using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ItemSchema : EntitySchema
	{
		public new static Microsoft.Exchange.Services.OData.Model.ItemSchema SchemaInstance
		{
			get
			{
				return Microsoft.Exchange.Services.OData.Model.ItemSchema.ItemSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return Item.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return Microsoft.Exchange.Services.OData.Model.ItemSchema.DeclaredItemProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return Microsoft.Exchange.Services.OData.Model.ItemSchema.AllItemProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return Microsoft.Exchange.Services.OData.Model.ItemSchema.DefaultItemProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> MandatoryCreationProperties
		{
			get
			{
				return Microsoft.Exchange.Services.OData.Model.ItemSchema.MandatoryItemCreationProperties;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ItemSchema()
		{
			PropertyDefinition propertyDefinition = new PropertyDefinition("ChangeKey", typeof(string));
			propertyDefinition.EdmType = EdmCoreModel.Instance.GetString(true);
			PropertyDefinition propertyDefinition2 = propertyDefinition;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider = new SimpleEwsPropertyProvider(Microsoft.Exchange.Services.Core.DataConverter.ItemSchema.ItemId);
			simpleEwsPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e[ep] = ((ItemId)s[sp]).ChangeKey;
			};
			propertyDefinition2.EwsPropertyProvider = simpleEwsPropertyProvider;
			PropertyDefinition propertyDefinition3 = propertyDefinition;
			DataEntityPropertyProvider<IStorageEntity> dataEntityPropertyProvider = new DataEntityPropertyProvider<IStorageEntity>("ChangeKey");
			dataEntityPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, IStorageEntity d)
			{
				e[ep] = d.ChangeKey;
			};
			dataEntityPropertyProvider.Setter = delegate(Entity e, PropertyDefinition ep, IStorageEntity d)
			{
				d.ChangeKey = (string)e[ep];
			};
			propertyDefinition3.DataEntityPropertyProvider = dataEntityPropertyProvider;
			Microsoft.Exchange.Services.OData.Model.ItemSchema.ChangeKey = propertyDefinition;
			Microsoft.Exchange.Services.OData.Model.ItemSchema.ClassName = new PropertyDefinition("ClassName", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = PropertyDefinitionFlags.CanFilter,
				EwsPropertyProvider = new SimpleEwsPropertyProvider(Microsoft.Exchange.Services.Core.DataConverter.ItemSchema.ItemClass),
				DataEntityPropertyProvider = new DataEntityPropertyProvider<IItem>("ClassName")
			};
			PropertyDefinition propertyDefinition4 = new PropertyDefinition("Subject", typeof(string));
			propertyDefinition4.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition4.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			propertyDefinition4.EwsPropertyProvider = new SimpleEwsPropertyProvider(Microsoft.Exchange.Services.Core.DataConverter.ItemSchema.Subject);
			PropertyDefinition propertyDefinition5 = propertyDefinition4;
			DataEntityPropertyProvider<IItem> dataEntityPropertyProvider2 = new DataEntityPropertyProvider<IItem>("Subject");
			dataEntityPropertyProvider2.Getter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				e[ep] = d.Subject;
			};
			dataEntityPropertyProvider2.Setter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				d.Subject = (string)e[ep];
			};
			propertyDefinition5.DataEntityPropertyProvider = dataEntityPropertyProvider2;
			Microsoft.Exchange.Services.OData.Model.ItemSchema.Subject = propertyDefinition4;
			PropertyDefinition propertyDefinition6 = new PropertyDefinition("Body", typeof(ItemBody));
			propertyDefinition6.EdmType = new EdmComplexTypeReference(ItemBody.EdmComplexType.Member, true);
			propertyDefinition6.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			propertyDefinition6.EwsPropertyProvider = new BodyPropertyProvider(Microsoft.Exchange.Services.Core.DataConverter.ItemSchema.Body);
			PropertyDefinition propertyDefinition7 = propertyDefinition6;
			DataEntityPropertyProvider<IItem> dataEntityPropertyProvider3 = new DataEntityPropertyProvider<IItem>("Body");
			dataEntityPropertyProvider3.Getter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				e[ep] = d.Body.ToItemBody();
			};
			dataEntityPropertyProvider3.Setter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				d.Body = ((ItemBody)e[ep]).ToDataEntityItemBody();
			};
			propertyDefinition7.DataEntityPropertyProvider = dataEntityPropertyProvider3;
			propertyDefinition6.ODataPropertyValueConverter = new ItemBodyODataConverter();
			Microsoft.Exchange.Services.OData.Model.ItemSchema.Body = propertyDefinition6;
			PropertyDefinition propertyDefinition8 = new PropertyDefinition("BodyPreview", typeof(string));
			propertyDefinition8.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition8.EwsPropertyProvider = new SimpleEwsPropertyProvider(Microsoft.Exchange.Services.Core.DataConverter.ItemSchema.Preview);
			PropertyDefinition propertyDefinition9 = propertyDefinition8;
			DataEntityPropertyProvider<IItem> dataEntityPropertyProvider4 = new DataEntityPropertyProvider<IItem>("BodyPreview");
			dataEntityPropertyProvider4.Getter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				e[ep] = d.Preview;
			};
			dataEntityPropertyProvider4.Setter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				d.Preview = (string)e[ep];
			};
			propertyDefinition9.DataEntityPropertyProvider = dataEntityPropertyProvider4;
			Microsoft.Exchange.Services.OData.Model.ItemSchema.BodyPreview = propertyDefinition8;
			PropertyDefinition propertyDefinition10 = new PropertyDefinition("HasAttachments", typeof(bool));
			propertyDefinition10.EdmType = EdmCoreModel.Instance.GetBoolean(true);
			propertyDefinition10.Flags = PropertyDefinitionFlags.CanFilter;
			propertyDefinition10.EwsPropertyProvider = new SimpleEwsPropertyProvider(Microsoft.Exchange.Services.Core.DataConverter.ItemSchema.HasAttachments);
			PropertyDefinition propertyDefinition11 = propertyDefinition10;
			DataEntityPropertyProvider<IItem> dataEntityPropertyProvider5 = new DataEntityPropertyProvider<IItem>("HasAttachments");
			dataEntityPropertyProvider5.Getter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				e[ep] = d.HasAttachments;
			};
			dataEntityPropertyProvider5.Setter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				d.HasAttachments = (bool)e[ep];
			};
			propertyDefinition11.DataEntityPropertyProvider = dataEntityPropertyProvider5;
			Microsoft.Exchange.Services.OData.Model.ItemSchema.HasAttachments = propertyDefinition10;
			PropertyDefinition propertyDefinition12 = new PropertyDefinition("Importance", typeof(Importance));
			propertyDefinition12.EdmType = new EdmEnumTypeReference(EnumTypes.GetEdmEnumType(typeof(Importance)), true);
			propertyDefinition12.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			PropertyDefinition propertyDefinition13 = propertyDefinition12;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider2 = new SimpleEwsPropertyProvider(Microsoft.Exchange.Services.Core.DataConverter.ItemSchema.Importance);
			simpleEwsPropertyProvider2.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				e[ep] = EnumConverter.CastEnumType<Importance>(s[sp]);
			};
			simpleEwsPropertyProvider2.Setter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				s[sp] = e[ep].ToString();
			};
			propertyDefinition13.EwsPropertyProvider = simpleEwsPropertyProvider2;
			PropertyDefinition propertyDefinition14 = propertyDefinition12;
			DataEntityPropertyProvider<IItem> dataEntityPropertyProvider6 = new DataEntityPropertyProvider<IItem>("Importance");
			dataEntityPropertyProvider6.Getter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				e[ep] = EnumConverter.CastEnumType<Importance>(d.Importance);
			};
			dataEntityPropertyProvider6.Setter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				d.Importance = EnumConverter.CastEnumType<Importance>(e[ep]);
			};
			propertyDefinition14.DataEntityPropertyProvider = dataEntityPropertyProvider6;
			Microsoft.Exchange.Services.OData.Model.ItemSchema.Importance = propertyDefinition12;
			PropertyDefinition propertyDefinition15 = new PropertyDefinition("Categories", typeof(string[]));
			propertyDefinition15.EdmType = new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true)));
			propertyDefinition15.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			propertyDefinition15.EwsPropertyProvider = new SimpleEwsPropertyProvider(Microsoft.Exchange.Services.Core.DataConverter.ItemSchema.Categories);
			propertyDefinition15.ODataPropertyValueConverter = new PrimitiveArrayValueConverter<string>();
			PropertyDefinition propertyDefinition16 = propertyDefinition15;
			DataEntityPropertyProvider<IItem> dataEntityPropertyProvider7 = new DataEntityPropertyProvider<IItem>("Categories");
			dataEntityPropertyProvider7.Getter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				e[ep] = ((d.Categories == null) ? null : d.Categories.ToArray());
			};
			dataEntityPropertyProvider7.Setter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				d.Categories = ((string[])e[ep]).ToList<string>();
			};
			propertyDefinition16.DataEntityPropertyProvider = dataEntityPropertyProvider7;
			Microsoft.Exchange.Services.OData.Model.ItemSchema.Categories = propertyDefinition15;
			PropertyDefinition propertyDefinition17 = new PropertyDefinition("DateTimeCreated", typeof(DateTimeOffset));
			propertyDefinition17.EdmType = EdmCoreModel.Instance.GetDateTimeOffset(true);
			propertyDefinition17.Flags = PropertyDefinitionFlags.CanFilter;
			propertyDefinition17.EwsPropertyProvider = new DateTimePropertyProvider(Microsoft.Exchange.Services.Core.DataConverter.ItemSchema.DateTimeCreated);
			PropertyDefinition propertyDefinition18 = propertyDefinition17;
			DataEntityPropertyProvider<IItem> dataEntityPropertyProvider8 = new DataEntityPropertyProvider<IItem>("DateTimeCreated");
			dataEntityPropertyProvider8.Getter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				e[ep] = d.DateTimeCreated.ToDateTimeOffset();
			};
			dataEntityPropertyProvider8.Setter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				d.DateTimeCreated = ((DateTimeOffset)e[ep]).ToExDateTime();
			};
			propertyDefinition18.DataEntityPropertyProvider = dataEntityPropertyProvider8;
			Microsoft.Exchange.Services.OData.Model.ItemSchema.DateTimeCreated = propertyDefinition17;
			PropertyDefinition propertyDefinition19 = new PropertyDefinition("LastModifiedTime", typeof(DateTimeOffset));
			propertyDefinition19.EdmType = EdmCoreModel.Instance.GetDateTimeOffset(true);
			propertyDefinition19.Flags = PropertyDefinitionFlags.CanFilter;
			propertyDefinition19.EwsPropertyProvider = new DateTimePropertyProvider(Microsoft.Exchange.Services.Core.DataConverter.ItemSchema.LastModifiedTime);
			PropertyDefinition propertyDefinition20 = propertyDefinition19;
			DataEntityPropertyProvider<IItem> dataEntityPropertyProvider9 = new DataEntityPropertyProvider<IItem>("LastModifiedTime");
			dataEntityPropertyProvider9.Getter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				e[ep] = d.LastModifiedTime.ToDateTimeOffset();
			};
			dataEntityPropertyProvider9.Setter = delegate(Entity e, PropertyDefinition ep, IItem d)
			{
				d.LastModifiedTime = ((DateTimeOffset)e[ep]).ToExDateTime();
			};
			propertyDefinition20.DataEntityPropertyProvider = dataEntityPropertyProvider9;
			Microsoft.Exchange.Services.OData.Model.ItemSchema.LastModifiedTime = propertyDefinition19;
			Microsoft.Exchange.Services.OData.Model.ItemSchema.Attachments = new PropertyDefinition("Attachments", typeof(IEnumerable<Attachment>))
			{
				Flags = PropertyDefinitionFlags.Navigation,
				NavigationTargetEntity = Attachment.EdmEntityType
			};
			Microsoft.Exchange.Services.OData.Model.ItemSchema.DeclaredItemProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				Microsoft.Exchange.Services.OData.Model.ItemSchema.ChangeKey,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.ClassName,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.Subject,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.Body,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.BodyPreview,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.Importance,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.Categories,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.HasAttachments,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.Attachments
			});
			Microsoft.Exchange.Services.OData.Model.ItemSchema.AllItemProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.AllEntityProperties.Union(Microsoft.Exchange.Services.OData.Model.ItemSchema.DeclaredItemProperties)));
			Microsoft.Exchange.Services.OData.Model.ItemSchema.DefaultItemProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(EntitySchema.DefaultEntityProperties)
			{
				Microsoft.Exchange.Services.OData.Model.ItemSchema.ChangeKey,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.ClassName,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.Subject,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.BodyPreview,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.Body,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.Importance,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.Categories,
				Microsoft.Exchange.Services.OData.Model.ItemSchema.HasAttachments
			});
			Microsoft.Exchange.Services.OData.Model.ItemSchema.MandatoryItemCreationProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>());
			Microsoft.Exchange.Services.OData.Model.ItemSchema.ItemSchemaInstance = new LazyMember<Microsoft.Exchange.Services.OData.Model.ItemSchema>(() => new Microsoft.Exchange.Services.OData.Model.ItemSchema());
		}

		public static readonly PropertyDefinition ChangeKey;

		public static readonly PropertyDefinition ClassName;

		public static readonly PropertyDefinition Subject;

		public static readonly PropertyDefinition Body;

		public static readonly PropertyDefinition BodyPreview;

		public static readonly PropertyDefinition HasAttachments;

		public static readonly PropertyDefinition Importance;

		public static readonly PropertyDefinition Categories;

		public static readonly PropertyDefinition DateTimeCreated;

		public static readonly PropertyDefinition LastModifiedTime;

		public static readonly PropertyDefinition Attachments;

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredItemProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> AllItemProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultItemProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> MandatoryItemCreationProperties;

		private static readonly LazyMember<Microsoft.Exchange.Services.OData.Model.ItemSchema> ItemSchemaInstance;
	}
}
