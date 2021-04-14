using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class DatabaseStatusFiller : MonadAdapterFiller
	{
		public DatabaseStatusFiller(string commandText, ICommandBuilder builder) : base(commandText, builder)
		{
		}

		public override void BuildCommand(string searchText, object[] pipeline, DataRow row)
		{
			this.BuildCommandWithScope(searchText, pipeline, row, null);
		}

		public override void BuildCommandWithScope(string searchText, object[] pipeline, DataRow row, object scope)
		{
			this.DatabaseTable = (row["Databases"] as DataTable);
			base.BuildCommandWithScope(searchText, null, row, scope);
		}

		private IEnumerable<string> GetCommandScripts()
		{
			if (this.DatabaseTable != null)
			{
				if (this.DatabaseTable.Rows.Count == 1)
				{
					yield return string.Format("{0} -Identity '{1}' | Filter-PropertyEqualTo -Property 'ActiveCopy' -Value $true", base.GetExecutingCommandText(), this.DatabaseTable.Rows[0]["Identity"].ToQuotationEscapedString());
				}
				else
				{
					foreach (string serverName in this.GetMailboxServers())
					{
						yield return base.Command.CommandText = string.Format("{0} -Server '{1}' | Filter-PropertyEqualTo -Property 'ActiveCopy' -Value $true", base.GetExecutingCommandText(), serverName.ToQuotationEscapedString());
					}
				}
			}
			yield break;
		}

		private IEnumerable<string> GetMailboxServers()
		{
			HashSet<string> hashSet = new HashSet<string>();
			if (this.DatabaseTable != null)
			{
				foreach (object obj in this.DatabaseTable.Rows)
				{
					DataRow dataRow = (DataRow)obj;
					DatabaseCopy[] array = dataRow["DatabaseCopies"] as DatabaseCopy[];
					if (array != null)
					{
						foreach (DatabaseCopy databaseCopy in array)
						{
							if (databaseCopy != null && !string.IsNullOrEmpty(databaseCopy.HostServerName))
							{
								hashSet.Add(databaseCopy.HostServerName);
							}
						}
					}
				}
			}
			return hashSet;
		}

		private DataTable CreateDatabaseCopyEntryTable()
		{
			DataTable dataTable = new DataTable();
			dataTable.Locale = CultureInfo.InvariantCulture;
			dataTable.Columns.Add("DatabaseName", typeof(string));
			dataTable.Columns.Add("Status", typeof(CopyStatus));
			dataTable.Columns.Add("MailboxServer", typeof(string));
			dataTable.PrimaryKey = new DataColumn[]
			{
				dataTable.Columns["DatabaseName"]
			};
			return dataTable;
		}

		protected override void OnFill(DataTable table)
		{
			if (this.DatabaseTable != null)
			{
				using (DataTable databaseCopiesTable = this.CreateDatabaseCopyEntryTable())
				{
					databaseCopiesTable.RowChanged += delegate(object sender, DataRowChangeEventArgs rowChangedEvent)
					{
						if (rowChangedEvent.Action == DataRowAction.Add)
						{
							DataRow dataRow = this.FindDatabaseRowByName(rowChangedEvent.Row["DatabaseName"].ToString());
							if (dataRow != null)
							{
								bool? flag = this.ComputeDatabaseStatus(rowChangedEvent.Row);
								if (flag != null)
								{
									dataRow["Mounted"] = flag.Value;
								}
								else
								{
									dataRow["Mounted"] = DBNull.Value;
								}
								dataRow["MountedOnServer"] = rowChangedEvent.Row["MailboxServer"].ToString();
								table.Rows.Add(dataRow.ItemArray);
								this.DatabaseTable.Rows.Remove(dataRow);
							}
							databaseCopiesTable.Clear();
						}
					};
					this.FillTableWithDatabaseCopyEntries(databaseCopiesTable);
				}
			}
		}

		private DataRow FindDatabaseRowByName(string databaseName)
		{
			DataRow result = null;
			if (this.DatabaseTable != null && !string.IsNullOrEmpty(databaseName))
			{
				DataRow[] array = this.DatabaseTable.Select(string.Format("{0} = '{1}'", "Name", databaseName.ToQuotationEscapedString()));
				if (array != null && array.Length > 0)
				{
					result = array[0];
				}
			}
			return result;
		}

		private void FillTableWithDatabaseCopyEntries(DataTable table)
		{
			base.Command.CommandType = CommandType.Text;
			foreach (string commandText in this.GetCommandScripts())
			{
				base.Command.CommandText = commandText;
				using (MonadDataAdapter monadDataAdapter = new MonadDataAdapter(base.Command))
				{
					if (table.Columns.Count != 0)
					{
						monadDataAdapter.MissingSchemaAction = MissingSchemaAction.Ignore;
						monadDataAdapter.EnforceDataSetSchema = true;
					}
					monadDataAdapter.Fill(table);
				}
			}
		}

		private bool? ComputeDatabaseStatus(DataRow dataRow)
		{
			bool? result = null;
			if (!dataRow["Status"].IsNullValue())
			{
				switch ((CopyStatus)dataRow["Status"])
				{
				case CopyStatus.Mounted:
				case CopyStatus.Mounting:
					result = new bool?(true);
					break;
				case CopyStatus.Dismounted:
				case CopyStatus.Dismounting:
					result = new bool?(false);
					break;
				}
			}
			return result;
		}

		public override object Clone()
		{
			return new DatabaseStatusFiller(base.CommandText, this.CommandBuilder)
			{
				ResolveCommandText = base.ResolveCommandText,
				IsResolving = base.IsResolving
			};
		}

		private const string DatabaseName = "DatabaseName";

		private const string DatabaseCopyStatus = "Status";

		private const string DatabaseCopyMailboxServer = "MailboxServer";

		private const string CommandScriptPerServer = "{0} -Server '{1}' | Filter-PropertyEqualTo -Property 'ActiveCopy' -Value $true";

		private const string CommandScriptPerDatabase = "{0} -Identity '{1}' | Filter-PropertyEqualTo -Property 'ActiveCopy' -Value $true";

		private const string EqualFilterString = "{0} = '{1}'";

		private DataTable DatabaseTable;
	}
}
