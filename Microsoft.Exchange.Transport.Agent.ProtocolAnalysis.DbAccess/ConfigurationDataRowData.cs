using System;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class ConfigurationDataRowData : DataRowAccessBase<ConfigurationDataTable, ConfigurationDataRowData>
	{
		[PrimaryKey]
		public string ConfigName
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[0]).Value;
			}
			private set
			{
				((ColumnCache<string>)base.Columns[0]).Value = value;
			}
		}

		public string ConfigValue
		{
			get
			{
				return ((ColumnCache<string>)base.Columns[1]).Value;
			}
			set
			{
				((ColumnCache<string>)base.Columns[1]).Value = value;
			}
		}
	}
}
