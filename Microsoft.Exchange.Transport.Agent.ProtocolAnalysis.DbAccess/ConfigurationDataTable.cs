using System;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class ConfigurationDataTable : DataTable
	{
		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, PrimaryKey = true, Required = true)]
		public const int ConfigName = 0;

		[DataColumnDefinition(typeof(string), ColumnAccess.CachedProp, Required = true)]
		public const int ConfigValue = 1;
	}
}
