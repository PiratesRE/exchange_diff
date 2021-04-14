using System;
using System.Data;
using System.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Monad;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MonadDataAdapter : DbDataAdapter
	{
		public MonadDataAdapter()
		{
			ExTraceGlobals.IntegrationTracer.Information((long)this.GetHashCode(), "new MonadDataAdapter()");
		}

		public MonadDataAdapter(MonadCommand selectCommand) : this()
		{
			this.SelectCommand = selectCommand;
		}

		public MonadDataAdapter(string selectCommandText) : this(new MonadCommand(selectCommandText, new MonadConnection()))
		{
		}

		public MonadDataAdapter(string selectCommandText, string selectConnectionString) : this(new MonadCommand(selectCommandText, new MonadConnection(selectConnectionString)))
		{
		}

		public MonadDataAdapter(string selectCommandText, MonadConnection selectConnection) : this(new MonadCommand(selectCommandText, selectConnection))
		{
		}

		public bool EnforceDataSetSchema
		{
			get
			{
				return this.enforceDataSetSchema;
			}
			set
			{
				this.enforceDataSetSchema = value;
			}
		}

		public new MonadCommand UpdateCommand
		{
			get
			{
				return (MonadCommand)base.UpdateCommand;
			}
			set
			{
				base.UpdateCommand = value;
			}
		}

		public new MonadCommand SelectCommand
		{
			get
			{
				return (MonadCommand)base.SelectCommand;
			}
			set
			{
				base.SelectCommand = value;
			}
		}

		public new MonadCommand DeleteCommand
		{
			get
			{
				return (MonadCommand)base.DeleteCommand;
			}
			set
			{
				base.DeleteCommand = value;
			}
		}

		public new MonadCommand InsertCommand
		{
			get
			{
				return (MonadCommand)base.InsertCommand;
			}
			set
			{
				base.InsertCommand = value;
			}
		}

		protected override int Fill(DataSet dataSet, string srcTable, IDataReader dataReader, int startRecord, int maxRecords)
		{
			ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "-->MonadDataAdapter.Fill({0})", srcTable);
			ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "\tSelectCommand={0}", this.SelectCommand.CommandText);
			DataTable dataTable = dataSet.Tables[srcTable];
			int num;
			if (dataTable != null)
			{
				num = this.Fill(new DataTable[]
				{
					dataTable
				}, dataReader, startRecord, maxRecords);
			}
			else
			{
				if (this.enforceDataSetSchema)
				{
					throw new InvalidOperationException("EnforceDataSetSchema cannot be used if the data table is not already present in the dataset.");
				}
				num = base.Fill(dataSet, srcTable, dataReader, startRecord, maxRecords);
			}
			ExTraceGlobals.IntegrationTracer.Information<int>((long)this.GetHashCode(), "<--MonadDataAdapter.Fill(), {0}", num);
			return num;
		}

		protected override int Fill(DataTable[] dataTables, IDataReader dataReader, int startRecord, int maxRecords)
		{
			ExTraceGlobals.IntegrationTracer.Information<int, string>((long)this.GetHashCode(), "-->MonadDataAdapter.Fill({0} (first of {1}))", dataTables.Length, dataTables[0].TableName);
			ExTraceGlobals.IntegrationTracer.Information<string>((long)this.GetHashCode(), "\tSelectCommand={0}", this.SelectCommand.CommandText);
			MonadDataReader monadDataReader = dataReader as MonadDataReader;
			if (dataTables == null || dataTables[0] == null)
			{
				throw new ArgumentNullException("dataTables");
			}
			DataTable dataTable = dataTables[0];
			if (this.enforceDataSetSchema)
			{
				DataColumnMappingCollection mappings = null;
				if (base.TableMappings.Contains(dataTable.TableName))
				{
					mappings = base.TableMappings[dataTable.TableName].ColumnMappings;
				}
				monadDataReader.EnforceSchema(dataTable.Columns, mappings);
			}
			if (monadDataReader.PositionInfo != null)
			{
				dataTable.ExtendedProperties["Position"] = monadDataReader.PositionInfo.PageOffset;
				dataTable.ExtendedProperties["TotalCount"] = monadDataReader.PositionInfo.TotalCount;
			}
			int num = base.Fill(dataTables, dataReader, startRecord, maxRecords);
			if (monadDataReader.PositionInfo != null)
			{
				dataTable.ExtendedProperties["BookmarkPrevious"] = monadDataReader.FirstResult;
				dataTable.ExtendedProperties["BookmarkNext"] = monadDataReader.LastResult;
			}
			ExTraceGlobals.IntegrationTracer.Information<int>((long)this.GetHashCode(), "<--MonadDataAdapter.Fill(), {0}", num);
			return num;
		}

		public virtual object[] GetObjects()
		{
			object[] result;
			using (new OpenConnection(this.SelectCommand.Connection))
			{
				result = this.SelectCommand.Execute();
			}
			return result;
		}

		private bool enforceDataSetSchema;
	}
}
