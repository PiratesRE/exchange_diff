using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal abstract class BaseMigMonCsvSchema : CsvSchema
	{
		protected BaseMigMonCsvSchema(Dictionary<string, ProviderPropertyDefinition> requiredColumns, Dictionary<string, ProviderPropertyDefinition> optionalColumns, IEnumerable<string> prohibitedColumns) : base(int.MaxValue, requiredColumns, optionalColumns, prohibitedColumns)
		{
		}

		public virtual string TimeStampColumnName
		{
			get
			{
				return "Time";
			}
		}

		public virtual DataTable GetCsvSchemaDataTable()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(this.TimeStampColumnName, typeof(SqlDateTime));
			dataTable.Columns.Add("LoggingServerId", typeof(int));
			this.GetRequiredColumnsIds().ForEach(delegate(ColumnDefinition<int> rc)
			{
				dataTable.Columns.Add(rc.DataTableKeyColumnName, typeof(int));
			});
			foreach (IColumnDefinition columnDefinition in this.GetRequiredColumnsAsIs())
			{
				dataTable.Columns.Add(columnDefinition.ColumnName, columnDefinition.ColumnType);
			}
			this.GetOptionalColumnsIds().ForEach(delegate(ColumnDefinition<int> oc)
			{
				dataTable.Columns.Add(oc.DataTableKeyColumnName, typeof(int));
			});
			foreach (IColumnDefinition columnDefinition2 in this.GetOptionalColumnsAsIs())
			{
				dataTable.Columns.Add(columnDefinition2.ColumnName, columnDefinition2.ColumnType);
			}
			return dataTable;
		}

		public abstract List<ColumnDefinition<int>> GetRequiredColumnsIds();

		public abstract List<IColumnDefinition> GetRequiredColumnsAsIs();

		public abstract List<ColumnDefinition<int>> GetOptionalColumnsIds();

		public abstract List<IColumnDefinition> GetOptionalColumnsAsIs();

		protected static Dictionary<string, ProviderPropertyDefinition> GetRequiredColumns(List<ColumnDefinition<int>> requiredColumnsIds, List<IColumnDefinition> requiredColumnsAsIs, string timeStampColumn = "Time")
		{
			Dictionary<string, ProviderPropertyDefinition> r = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase)
			{
				{
					timeStampColumn,
					null
				}
			};
			requiredColumnsIds.ForEach(delegate(ColumnDefinition<int> rc)
			{
				if (!r.ContainsKey(rc.ColumnName))
				{
					r.Add(rc.ColumnName, null);
				}
			});
			requiredColumnsAsIs.ForEach(delegate(IColumnDefinition rc)
			{
				if (!r.ContainsKey(rc.ColumnName))
				{
					r.Add(rc.ColumnName, null);
				}
			});
			return r;
		}

		protected static Dictionary<string, ProviderPropertyDefinition> GetOptionalColumns(List<ColumnDefinition<int>> optionalColumnsIds, List<IColumnDefinition> optionalColumnsAsIs)
		{
			Dictionary<string, ProviderPropertyDefinition> o = new Dictionary<string, ProviderPropertyDefinition>(StringComparer.OrdinalIgnoreCase);
			optionalColumnsIds.ForEach(delegate(ColumnDefinition<int> oc)
			{
				o.Add(oc.ColumnName, null);
			});
			optionalColumnsAsIs.ForEach(delegate(IColumnDefinition oc)
			{
				o.Add(oc.ColumnName, null);
			});
			return o;
		}

		public const string LoggingServerIdKey = "LoggingServerId";

		public const string TimestampColumn = "Time";

		public const int InternalMaximumRowCount = 2147483647;
	}
}
