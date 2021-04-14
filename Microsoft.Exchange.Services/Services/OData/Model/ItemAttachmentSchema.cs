using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ItemAttachmentSchema : AttachmentSchema
	{
		public new static ItemAttachmentSchema SchemaInstance
		{
			get
			{
				return ItemAttachmentSchema.ItemAttachmentSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return ItemAttachment.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return ItemAttachmentSchema.DeclaredItemAttachmentProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return ItemAttachmentSchema.AllItemAttachmentProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return ItemAttachmentSchema.DefaultItemAttachmentProperties;
			}
		}

		public static readonly PropertyDefinition Item = new PropertyDefinition("Item", typeof(Item))
		{
			Flags = (PropertyDefinitionFlags.Navigation | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.ChildOnlyEntity),
			NavigationTargetEntity = Microsoft.Exchange.Services.OData.Model.Item.EdmEntityType
		};

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredItemAttachmentProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
		{
			ItemAttachmentSchema.Item
		});

		public static readonly ReadOnlyCollection<PropertyDefinition> AllItemAttachmentProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(AttachmentSchema.AllAttachmentProperties.Union(ItemAttachmentSchema.DeclaredItemAttachmentProperties)));

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultItemAttachmentProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(AttachmentSchema.DefaultAttachmentProperties)
		{
			ItemAttachmentSchema.Item
		});

		private static readonly LazyMember<ItemAttachmentSchema> ItemAttachmentSchemaInstance = new LazyMember<ItemAttachmentSchema>(() => new ItemAttachmentSchema());
	}
}
