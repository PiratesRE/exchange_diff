using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal abstract class BaseMrsMonitorCsvSchema : BaseMigMonCsvSchema
	{
		protected BaseMrsMonitorCsvSchema(Dictionary<string, ProviderPropertyDefinition> requiredColumns, Dictionary<string, ProviderPropertyDefinition> optionalColumns, IEnumerable<string> prohibitedColumns) : base(requiredColumns, optionalColumns, prohibitedColumns)
		{
		}

		public const string RequestGuidColumn = "RequestGuid";
	}
}
