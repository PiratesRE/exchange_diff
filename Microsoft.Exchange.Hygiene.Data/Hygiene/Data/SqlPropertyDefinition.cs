using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class SqlPropertyDefinition : ConfigurablePropertyBag
	{
		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.PropertyId.ToString());
			}
		}

		public string EntityName
		{
			get
			{
				return (string)this[SqlPropertyDefinition.EntityNameProp];
			}
			set
			{
				this[SqlPropertyDefinition.EntityNameProp] = value;
			}
		}

		public string PropertyName
		{
			get
			{
				return (string)this[SqlPropertyDefinition.PropertyNameProp];
			}
			set
			{
				this[SqlPropertyDefinition.PropertyNameProp] = value;
			}
		}

		public int EntityId
		{
			get
			{
				return (int)this[SqlPropertyDefinition.EntityIdProp];
			}
			set
			{
				this[SqlPropertyDefinition.EntityIdProp] = value;
			}
		}

		public int PropertyId
		{
			get
			{
				return (int)this[SqlPropertyDefinition.PropertyIdProp];
			}
			set
			{
				this[SqlPropertyDefinition.PropertyIdProp] = value;
			}
		}

		public SqlPropertyDefinitionFlags Flags
		{
			get
			{
				return (SqlPropertyDefinitionFlags)this[SqlPropertyDefinition.FlagsProp];
			}
			set
			{
				this[SqlPropertyDefinition.FlagsProp] = value;
			}
		}

		public SqlPropertyTypes Type
		{
			get
			{
				return (SqlPropertyTypes)this[SqlPropertyDefinition.TypeProp];
			}
			set
			{
				this[SqlPropertyDefinition.TypeProp] = value;
			}
		}

		public static readonly HygienePropertyDefinition PropertyNameProp = new HygienePropertyDefinition("PropertyName", typeof(string));

		public static readonly HygienePropertyDefinition EntityNameProp = new HygienePropertyDefinition("EntityName", typeof(string));

		public static readonly HygienePropertyDefinition EntityIdProp = new HygienePropertyDefinition("EntityId", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition PropertyIdProp = new HygienePropertyDefinition("PropertyId", typeof(int), int.MinValue, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition FlagsProp = new HygienePropertyDefinition("Flags", typeof(SqlPropertyDefinitionFlags), SqlPropertyDefinitionFlags.None, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition TypeProp = new HygienePropertyDefinition("Type", typeof(SqlPropertyTypes), SqlPropertyTypes.String, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
