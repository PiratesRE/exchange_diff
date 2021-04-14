using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Configuration
{
	internal static class AppConfigSchema
	{
		public const int MaxNameLength = 255;

		public static readonly HygienePropertyDefinition ParamIdProp = new HygienePropertyDefinition("ParamId", typeof(Guid), Guid.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ParamVersionProp = new HygienePropertyDefinition("ParamVersion", typeof(long), 0L, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ParamNameProp = new HygienePropertyDefinition("ParamName", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition ParamValueProp = new HygienePropertyDefinition("ParamValue", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DescriptionProp = new HygienePropertyDefinition("Description", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition DescriptionQueryProp = new HygienePropertyDefinition("Descriptions", typeof(string), null, ADPropertyDefinitionFlags.MultiValued);

		public static readonly HygienePropertyDefinition ParamNamesTableProp = new HygienePropertyDefinition("tvp_ParamNames", typeof(DataTable));

		public static readonly HygienePropertyDefinition ItemsTableProp = new HygienePropertyDefinition("tvp_Items", typeof(DataTable));

		public static readonly HygienePropertyDefinition NameVersionsTableProp = new HygienePropertyDefinition("tvp_NameVersions", typeof(DataTable));

		public sealed class AppConfigNameVersions : ConfigurablePropertyBag
		{
			public AppConfigNameVersions(DataTable nameVersions)
			{
				this.NameVersions = nameVersions;
			}

			public override ObjectId Identity
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public DataTable NameVersions
			{
				get
				{
					return (DataTable)this[AppConfigSchema.NameVersionsTableProp];
				}
				set
				{
					this[AppConfigSchema.NameVersionsTableProp] = value;
				}
			}

			public override Type GetSchemaType()
			{
				return typeof(AppConfigSchema);
			}
		}

		public sealed class AppConfigItems : ConfigurablePropertyBag
		{
			public AppConfigItems(DataTable items)
			{
				this.Items = items;
			}

			public override ObjectId Identity
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public DataTable Items
			{
				get
				{
					return (DataTable)this[AppConfigSchema.ItemsTableProp];
				}
				set
				{
					this[AppConfigSchema.ItemsTableProp] = value;
				}
			}

			public override Type GetSchemaType()
			{
				return typeof(AppConfigSchema);
			}
		}

		public sealed class AppConfigByName : AppConfigParameter
		{
		}

		public sealed class AppConfigByDescription : AppConfigParameter
		{
		}

		public sealed class AppConfigByVersion : AppConfigParameter
		{
		}
	}
}
