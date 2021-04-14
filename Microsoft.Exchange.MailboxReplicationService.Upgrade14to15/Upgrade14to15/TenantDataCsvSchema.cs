using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	internal class TenantDataCsvSchema : CsvSchema
	{
		private TenantDataCsvSchema() : base(int.MaxValue, TenantDataCsvSchema.requiredColumns, null, null)
		{
		}

		public static TenantDataCsvSchema TenantDataCsvSchemaInstance
		{
			get
			{
				return TenantDataCsvSchema.tenantDataCsvSchemaInstance;
			}
		}

		internal static ProviderPropertyDefinition GetDefaultPropertyDefinition(string propertyName, PropertyDefinitionConstraint[] constraints)
		{
			if (constraints == null)
			{
				constraints = PropertyDefinitionConstraint.None;
			}
			return new SimpleProviderPropertyDefinition(propertyName, ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.None, null, constraints, constraints);
		}

		public const string TenantColumn = "TenantName";

		public const string PrimaryMBXCountColumn = "PrimaryMBXNum";

		public const string PrimaryMBXSizeColumn = "PrimaryMBXSize";

		public const string ArchiveMBXCountColumn = "ArchiveMBXNum";

		public const string ArchiveMBXSizeColumn = "ArchiveMBXSize";

		private const int InternalMaximumRowCount = 2147483647;

		private static readonly TenantDataCsvSchema tenantDataCsvSchemaInstance = new TenantDataCsvSchema();

		private static Dictionary<string, ProviderPropertyDefinition> requiredColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"TenantName",
				TenantDataCsvSchema.GetDefaultPropertyDefinition("TenantName", null)
			},
			{
				"PrimaryMBXNum",
				TenantDataCsvSchema.GetDefaultPropertyDefinition("PrimaryMBXNum", null)
			},
			{
				"PrimaryMBXSize",
				TenantDataCsvSchema.GetDefaultPropertyDefinition("PrimaryMBXSize", null)
			},
			{
				"ArchiveMBXNum",
				TenantDataCsvSchema.GetDefaultPropertyDefinition("ArchiveMBXNum", null)
			},
			{
				"ArchiveMBXSize",
				TenantDataCsvSchema.GetDefaultPropertyDefinition("ArchiveMBXSize", null)
			}
		};
	}
}
