using System;
using System.Data;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class TableDataHandler : ExchangeDataHandler
	{
		public TableDataHandler(string selectCommandText, string updateCommandText, string insertCommandText, string deleteCommandText)
		{
			this.DataTable = new DataTable();
			this.dataAdapter = new MonadDataAdapter();
			this.dataAdapter.EnforceDataSetSchema = true;
			MonadConnection connection = new MonadConnection("timeout=30", new CommandInteractionHandler(), ADServerSettingsSingleton.GetInstance().CreateRunspaceServerSettingsObject(), PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo());
			this.dataAdapter.SelectCommand = new LoggableMonadCommand(selectCommandText, connection);
			this.dataAdapter.UpdateCommand = new LoggableMonadCommand(updateCommandText, connection);
			this.dataAdapter.UpdateCommand.UpdatedRowSource = UpdateRowSource.None;
			this.dataAdapter.DeleteCommand = new LoggableMonadCommand(deleteCommandText, connection);
			this.dataAdapter.InsertCommand = new LoggableMonadCommand(insertCommandText, connection);
			this.dataAdapter.InsertCommand.UpdatedRowSource = UpdateRowSource.None;
			this.synchronizationContext = SynchronizationContext.Current;
		}

		protected DataTable DataTable
		{
			get
			{
				return this.dataTable;
			}
			set
			{
				if (this.dataTable != value)
				{
					this.dataTable = value;
					base.DataSource = this.dataTable;
				}
			}
		}

		internal MonadParameterCollection SelectCommandParameters
		{
			get
			{
				return this.dataAdapter.SelectCommand.Parameters;
			}
		}

		internal MonadParameterCollection UpdateCommandParameters
		{
			get
			{
				return this.dataAdapter.UpdateCommand.Parameters;
			}
		}

		internal MonadParameterCollection InsertCommandParameters
		{
			get
			{
				return this.dataAdapter.InsertCommand.Parameters;
			}
		}

		internal MonadParameterCollection DeleteCommandParameters
		{
			get
			{
				return this.dataAdapter.DeleteCommand.Parameters;
			}
		}

		internal override void OnReadData(CommandInteractionHandler interactionHandler, string pageName)
		{
			DataTable dataTable = this.DataTable.Clone();
			this.OnFillTable(dataTable, interactionHandler);
			this.synchronizationContext.Send(new SendOrPostCallback(this.CopyChangeFromTableForRead), dataTable);
		}

		internal virtual void OnFillTable(DataTable table, CommandInteractionHandler interactionHandler)
		{
			this.dataAdapter.SelectCommand.Connection.InteractionHandler = interactionHandler;
			this.dataAdapter.Fill(table);
		}

		internal override void OnSaveData(CommandInteractionHandler interactionHandler)
		{
			this.dataAdapter.SelectCommand.Connection.InteractionHandler = interactionHandler;
			DataTable state = this.CreateUpdateTable();
			try
			{
				this.dataAdapter.Update(state);
			}
			finally
			{
				this.synchronizationContext.Send(new SendOrPostCallback(this.CopyChangeFromTableForUpdate), state);
			}
		}

		private void CopyChangeFromTableForRead(object arg)
		{
			DataTable table = (DataTable)arg;
			if (this.DataTable.HasErrors)
			{
				DataRow[] errors = this.DataTable.GetErrors();
				table.ImportRows(errors);
			}
			this.DataTable.Clear();
			this.DataTable.Merge(table);
		}

		private void CopyChangeFromTableForUpdate(object arg)
		{
			DataTable table = (DataTable)arg;
			this.DataTable.Clear();
			this.DataTable.Merge(table);
		}

		private void FillParametersFromDataRow(MonadCommand command, DataRow row)
		{
			DataRowVersion version = (row.RowState == DataRowState.Deleted) ? DataRowVersion.Original : DataRowVersion.Current;
			foreach (object obj in command.Parameters)
			{
				MonadParameter monadParameter = (MonadParameter)obj;
				if (row.Table.Columns.Contains(monadParameter.ParameterName))
				{
					monadParameter.Value = row[monadParameter.ParameterName, version];
				}
			}
		}

		protected virtual DataTable CreateUpdateTable()
		{
			DataTable result = this.DataTable.Clone();
			DataRow[] array = this.DataTable.Select(null, this.DataTable.DefaultView.Sort, DataViewRowState.Deleted);
			DataRow[] array2 = this.DataTable.Select(null, this.DataTable.DefaultView.Sort, DataViewRowState.CurrentRows);
			DataRow[] array3 = new DataRow[array.Length + array2.Length];
			Array.Copy(array, array3, array.Length);
			Array.Copy(array2, 0, array3, array.Length, array2.Length);
			result.ImportRows(array3);
			return result;
		}

		internal override string CommandToRun
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				MonadCommand monadCommand = this.dataAdapter.InsertCommand.Clone();
				MonadCommand monadCommand2 = this.dataAdapter.DeleteCommand.Clone();
				MonadCommand monadCommand3 = this.dataAdapter.UpdateCommand.Clone();
				DataTable dataTable = this.CreateUpdateTable();
				foreach (object obj in dataTable.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					DataRowState rowState = dataRow.RowState;
					if (rowState != DataRowState.Added)
					{
						if (rowState != DataRowState.Deleted)
						{
							if (rowState == DataRowState.Modified)
							{
								this.FillParametersFromDataRow(monadCommand3, dataRow);
								stringBuilder.AppendLine(monadCommand3.ToString());
							}
						}
						else
						{
							this.FillParametersFromDataRow(monadCommand2, dataRow);
							stringBuilder.AppendLine(monadCommand2.ToString());
						}
					}
					else
					{
						this.FillParametersFromDataRow(monadCommand, dataRow);
						stringBuilder.AppendLine(monadCommand.ToString());
					}
				}
				return stringBuilder.ToString();
			}
		}

		private DataTable dataTable;

		private MonadDataAdapter dataAdapter;

		private SynchronizationContext synchronizationContext;
	}
}
