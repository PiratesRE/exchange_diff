using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class EntitySchema : Schema
	{
		public static EntitySchema SchemaInstance
		{
			get
			{
				return EntitySchema.EntitySchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return Entity.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return EntitySchema.DeclaredEntityProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return EntitySchema.AllEntityProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return EntitySchema.DefaultEntityProperties;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static EntitySchema()
		{
			PropertyDefinition propertyDefinition = new PropertyDefinition("Id", typeof(string));
			propertyDefinition.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition.EwsPropertyProvider = new IdPropertyProvider();
			PropertyDefinition propertyDefinition2 = propertyDefinition;
			DataEntityPropertyProvider<IEntity> dataEntityPropertyProvider = new DataEntityPropertyProvider<IEntity>("Id");
			dataEntityPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, IEntity d)
			{
				e.Id = EwsIdConverter.EwsIdToODataId(d.Id);
			};
			dataEntityPropertyProvider.Setter = delegate(Entity e, PropertyDefinition ep, IEntity d)
			{
				d.Id = EwsIdConverter.ODataIdToEwsId(e.Id);
			};
			dataEntityPropertyProvider.QueryConstantBuilder = ((object o) => Expression.Constant(EwsIdConverter.ODataIdToEwsId((string)o)));
			propertyDefinition2.DataEntityPropertyProvider = dataEntityPropertyProvider;
			PropertyDefinition propertyDefinition3 = propertyDefinition;
			SimpleDirectoryPropertyProvider simpleDirectoryPropertyProvider = new SimpleDirectoryPropertyProvider(ADRecipientSchema.PrimarySmtpAddress);
			simpleDirectoryPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, ADRawEntry d, ADPropertyDefinition dp)
			{
				e.Id = d[dp].ToString();
			};
			propertyDefinition3.ADDriverPropertyProvider = simpleDirectoryPropertyProvider;
			EntitySchema.Id = propertyDefinition;
			EntitySchema.EntitySchemaInstance = new LazyMember<EntitySchema>(() => new EntitySchema());
			EntitySchema.DeclaredEntityProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				EntitySchema.Id
			});
			EntitySchema.AllEntityProperties = EntitySchema.DeclaredEntityProperties;
			EntitySchema.DefaultEntityProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				EntitySchema.Id
			});
		}

		public static readonly PropertyDefinition Id;

		private static readonly LazyMember<EntitySchema> EntitySchemaInstance;

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredEntityProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> AllEntityProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultEntityProperties;
	}
}
