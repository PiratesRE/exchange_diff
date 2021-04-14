using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationPreexistingBatchCsvSchema : MigrationBatchCsvSchema
	{
		public MigrationPreexistingBatchCsvSchema() : base(int.MaxValue, MigrationPreexistingBatchCsvSchema.requiredColumns, null, null)
		{
		}

		public void WriteHeader(StreamWriter streamWriter)
		{
			streamWriter.WriteCsvLine(MigrationPreexistingBatchCsvSchema.requiredColumns.Keys);
		}

		public void Write(StreamWriter streamWriter, Guid userId)
		{
			streamWriter.WriteCsvLine(new List<string>(1)
			{
				userId.ToString()
			});
		}

		public const string JobItemGuidColumnName = "JobItemGuid";

		internal static readonly ProviderPropertyDefinition JobItemGuid = new SimpleProviderPropertyDefinition("JobItemGuid", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.TaskPopulated, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		private static readonly Dictionary<string, ProviderPropertyDefinition> requiredColumns = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
		{
			{
				"JobItemGuid",
				MigrationPreexistingBatchCsvSchema.JobItemGuid
			}
		};
	}
}
