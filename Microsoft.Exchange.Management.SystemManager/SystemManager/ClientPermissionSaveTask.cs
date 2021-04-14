using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ClientPermissionSaveTask : Saver
	{
		public ClientPermissionSaveTask(string commandText, string workUnitTextColumn, string workUnitIconColumn) : base(workUnitTextColumn, workUnitIconColumn)
		{
			this.CommandText = commandText;
		}

		public ClientPermissionSaveTask() : base(null, null)
		{
		}

		[DefaultValue(null)]
		public string CommandText { get; set; }

		public override string CommandToRun
		{
			get
			{
				return this.dataHandler.CommandToRun;
			}
		}

		public override string ModifiedParametersDescription
		{
			get
			{
				return this.dataHandler.ModifiedParametersDescription;
			}
		}

		public override object WorkUnits
		{
			get
			{
				return this.dataHandler.WorkUnits;
			}
		}

		public override List<object> SavedResults
		{
			get
			{
				return this.dataHandler.SavedResults;
			}
		}

		public override void Cancel()
		{
			this.dataHandler.Cancel();
		}

		public override bool IsRunnable(DataRow row, DataObjectStore store)
		{
			if (!base.IsRunnable(row, store))
			{
				return false;
			}
			DataTable values = this.GetValues(row);
			return values != null && values.Rows.Count > 0;
		}

		public override void BuildParameters(DataRow row, DataObjectStore store, IList<ParameterProfile> paramInfos)
		{
			this.paramInfos = paramInfos;
		}

		public override void Run(object interactionHandler, DataRow row, DataObjectStore store)
		{
			this.dataHandler.ProgressReport += base.OnProgressReport;
			try
			{
				this.BuildCommandScript(row, this.paramInfos);
				this.dataHandler.Save(interactionHandler as CommandInteractionHandler);
			}
			finally
			{
				this.dataHandler.ProgressReport -= base.OnProgressReport;
			}
		}

		private DataTable GetValues(DataRow row)
		{
			return (row["ClientPermissionTable"] as DataTable).Copy();
		}

		public override void UpdateWorkUnits(DataRow row)
		{
			this.CreateDataHandlers(row);
			this.dataHandler.UpdateWorkUnits();
			this.UpdateWorkUnitsInfo(row);
			this.dataHandler.ResetCancel();
		}

		internal void UpdateConnection(MonadConnection connection)
		{
			foreach (DataHandler dataHandler in this.dataHandler.DataHandlers)
			{
				SingleTaskDataHandler singleTaskDataHandler = (SingleTaskDataHandler)dataHandler;
				singleTaskDataHandler.Command.Connection = connection;
			}
		}

		private void BuildCommandScript(DataRow row, IList<ParameterProfile> paramInfos)
		{
			DataRow dataRow = this.PrepareCombineRow(row);
			foreach (KeyValuePair<DataRow, SingleTaskDataHandler> keyValuePair in this.permissionItems)
			{
				this.CopyPermission(dataRow, keyValuePair.Key);
				keyValuePair.Value.CommandText = MonadPipelineSaveTask.BuildCommandScript(this.CommandText, dataRow, paramInfos);
			}
		}

		private void CreateDataHandlers(DataRow row)
		{
			DataTable values = this.GetValues(row);
			this.permissionItems.Clear();
			this.dataHandler.DataHandlers.Clear();
			if (values != null && values.Rows.Count > 0)
			{
				foreach (object obj in values.Rows)
				{
					DataRow key = (DataRow)obj;
					SingleTaskDataHandler singleTaskDataHandler = this.CreateDataHandler();
					this.permissionItems.Add(key, singleTaskDataHandler);
					this.dataHandler.DataHandlers.Add(singleTaskDataHandler);
				}
			}
		}

		private SingleTaskDataHandler CreateDataHandler()
		{
			return new SingleTaskDataHandler
			{
				Command = 
				{
					CommandType = CommandType.Text
				}
			};
		}

		private void UpdateWorkUnitsInfo(DataRow row)
		{
			DataRow dataRow = this.PrepareCombineRow(row);
			foreach (KeyValuePair<DataRow, SingleTaskDataHandler> keyValuePair in this.permissionItems)
			{
				this.CopyPermission(dataRow, keyValuePair.Key);
				keyValuePair.Value.WorkUnit.Icon = this.GetDisplayIcon(dataRow);
				keyValuePair.Value.WorkUnit.Text = this.GetDisplayText(dataRow);
				if (this.paramInfos != null)
				{
					keyValuePair.Value.WorkUnit.Description = MonadSaveTask.BuildParametersDescription(dataRow, this.paramInfos);
				}
			}
		}

		private DataRow PrepareCombineRow(DataRow mainRow)
		{
			DataTable dataTable = mainRow.Table.Clone();
			foreach (KeyValuePair<string, string> keyValuePair in this.permissionColumnMap)
			{
				DataColumn column = new DataColumn(keyValuePair.Value, typeof(object));
				dataTable.Columns.Add(column);
			}
			DataRow dataRow = dataTable.NewRow();
			foreach (object obj in mainRow.Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				dataRow[dataColumn.ColumnName] = mainRow[dataColumn];
			}
			dataTable.Rows.Add(dataRow);
			return dataRow;
		}

		private void CopyPermission(DataRow combineRow, DataRow permissionRow)
		{
			combineRow.BeginEdit();
			foreach (KeyValuePair<string, string> keyValuePair in this.permissionColumnMap)
			{
				combineRow[keyValuePair.Value] = permissionRow[keyValuePair.Key];
			}
			combineRow.EndEdit();
		}

		private string GetDisplayText(DataRow row)
		{
			if (string.IsNullOrEmpty(base.WorkUnitTextColumn))
			{
				return null;
			}
			return row[base.WorkUnitTextColumn].ToString();
		}

		private Icon GetDisplayIcon(DataRow row)
		{
			if (string.IsNullOrEmpty(base.WorkUnitIconColumn))
			{
				return null;
			}
			return WinformsHelper.GetIconFromIconLibrary(row[base.WorkUnitIconColumn].ToString());
		}

		private const string ClientPermissionTableColumnName = "ClientPermissionTable";

		private Dictionary<string, string> permissionColumnMap = new Dictionary<string, string>
		{
			{
				"Identity",
				"UserIdentity"
			},
			{
				"Name",
				"UserName"
			},
			{
				"RecipientTypeDetails",
				"UserTypes"
			},
			{
				"AccessRights",
				"AccessRights"
			}
		};

		private DataHandler dataHandler = new DataHandler(false);

		private IDictionary<DataRow, SingleTaskDataHandler> permissionItems = new Dictionary<DataRow, SingleTaskDataHandler>();

		private IList<ParameterProfile> paramInfos;
	}
}
