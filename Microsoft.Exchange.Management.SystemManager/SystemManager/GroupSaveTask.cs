using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class GroupSaveTask : Saver
	{
		public GroupSaveTask(string commandText, string workUnitTextColumn, string workUnitIconColumn, string commandParam, string groupColumnName, string groupLambdaExpression) : base(workUnitTextColumn, workUnitIconColumn)
		{
			this.CommandText = commandText;
			this.CommandParam = commandParam;
			this.GroupColumnName = groupColumnName;
			this.GroupLambdaExpression = groupLambdaExpression;
		}

		public GroupSaveTask() : base(null, null)
		{
		}

		[DefaultValue(null)]
		public string CommandText { get; set; }

		[DefaultValue(null)]
		public string CommandParam { get; set; }

		[DefaultValue(null)]
		[DDIDataColumnExist]
		public string GroupColumnName { get; set; }

		[DDIValidLambdaExpression]
		public string GroupLambdaExpression
		{
			get
			{
				return this.groupLambdaExpression;
			}
			set
			{
				this.groupLambdaExpression = value;
				this.groupColumnExpression = ExpressionCalculator.BuildColumnExpression(value);
			}
		}

		public override void Cancel()
		{
			this.dataHandler.Cancel();
		}

		public override void Run(object interactionHandler, DataRow row, DataObjectStore store)
		{
			this.dataHandler.ProgressReport += base.OnProgressReport;
			try
			{
				this.dataHandler.Save(interactionHandler as CommandInteractionHandler);
			}
			finally
			{
				this.dataHandler.ProgressReport -= base.OnProgressReport;
			}
		}

		public override bool IsRunnable(DataRow row, DataObjectStore store)
		{
			IList values = this.GetValues(row);
			return values != null && values.Count > 0 && base.IsRunnable(row, store);
		}

		private IList GetValues(DataRow row)
		{
			IList result;
			if (!string.IsNullOrEmpty(this.GroupColumnName))
			{
				result = (row[this.GroupColumnName] as IList);
			}
			else
			{
				result = (ExpressionCalculator.CalculateLambdaExpression(this.groupColumnExpression, typeof(IList), row, null) as IList);
			}
			return result;
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

		public override void UpdateWorkUnits(DataRow row)
		{
			this.groupValues = this.GetValues(row);
			if (!string.IsNullOrEmpty(base.WorkUnitTextColumn))
			{
				this.workUnitsTextList = (row[base.WorkUnitTextColumn] as IList);
			}
			if (this.groupValues != null && this.groupValues.Count > 0)
			{
				this.dataHandler.DataHandlers.Clear();
				if (string.IsNullOrEmpty(this.CommandParam))
				{
					WorkUnit[] array = new WorkUnit[this.groupValues.Count];
					for (int i = 0; i < this.groupValues.Count; i++)
					{
						array[i] = new WorkUnit(this.GetDisplayText(i), this.GetDisplayIcon(row), this.groupValues[i]);
					}
					this.dataHandler.DataHandlers.Add(new BulkSaveDataHandler(array.DeepCopy(), this.CommandText));
				}
				else
				{
					for (int j = 0; j < this.groupValues.Count; j++)
					{
						this.dataHandler.DataHandlers.Add(this.CreateDataHandler(this.CommandText, j, this.GetDisplayIcon(row)));
					}
				}
				this.dataHandler.UpdateWorkUnits();
				foreach (WorkUnit workUnit in this.dataHandler.WorkUnits)
				{
					workUnit.Description = this.ModifiedParametersDescription;
				}
				this.dataHandler.ResetCancel();
			}
		}

		internal void UpdateConnection(MonadConnection connection)
		{
			foreach (DataHandler dataHandler in this.dataHandler.DataHandlers)
			{
				SingleTaskDataHandler singleTaskDataHandler = (SingleTaskDataHandler)dataHandler;
				singleTaskDataHandler.Command.Connection = connection;
			}
		}

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
				return this.modifiedParametersDescription;
			}
		}

		public override void BuildParameters(DataRow row, DataObjectStore store, IList<ParameterProfile> paramInfos)
		{
			foreach (DataHandler dataHandler in this.dataHandler.DataHandlers)
			{
				SingleTaskDataHandler singleTaskDataHandler = (SingleTaskDataHandler)dataHandler;
				foreach (ParameterProfile parameterProfile in paramInfos)
				{
					singleTaskDataHandler.Parameters.Remove(parameterProfile.Name);
				}
				MonadSaveTask.BuildParametersCore(row, paramInfos, singleTaskDataHandler.Parameters);
			}
			this.modifiedParametersDescription = MonadSaveTask.BuildParametersDescription(row, paramInfos);
		}

		private SingleTaskDataHandler CreateDataHandler(string commandText, int index, Icon displayIcon)
		{
			SingleTaskDataHandler singleTaskDataHandler;
			if (this.workUnits.Length > 0)
			{
				singleTaskDataHandler = new BulkSaveDataHandler(this.workUnits.DeepCopy(), commandText);
			}
			else
			{
				singleTaskDataHandler = new SingleTaskDataHandler(commandText);
				singleTaskDataHandler.WorkUnit.Text = this.GetDisplayText(index);
				singleTaskDataHandler.WorkUnit.Icon = displayIcon;
			}
			singleTaskDataHandler.Parameters.AddWithValue(this.CommandParam, this.groupValues[index]);
			return singleTaskDataHandler;
		}

		private string GetDisplayText(int index)
		{
			object obj = string.IsNullOrEmpty(base.WorkUnitTextColumn) ? this.groupValues[index] : this.workUnitsTextList[index];
			ADObjectId adobjectId = obj as ADObjectId;
			if (adobjectId != null)
			{
				return adobjectId.Name;
			}
			if (obj != null)
			{
				return obj.ToString();
			}
			return string.Empty;
		}

		private Icon GetDisplayIcon(DataRow row)
		{
			if (string.IsNullOrEmpty(base.WorkUnitIconColumn))
			{
				return null;
			}
			return WinformsHelper.GetIconFromIconLibrary(row[base.WorkUnitIconColumn].ToString());
		}

		public override Saver CreateBulkSaver(WorkUnit[] workUnits)
		{
			this.workUnits = workUnits;
			return this;
		}

		private DataHandler dataHandler = new DataHandler(false);

		private WorkUnit[] workUnits = new WorkUnit[0];

		private ColumnExpression groupColumnExpression;

		private string modifiedParametersDescription;

		private string groupLambdaExpression;

		private IList workUnitsTextList;

		private IList groupValues;
	}
}
