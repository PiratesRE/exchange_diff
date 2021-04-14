using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class MonadSaveTask : Saver
	{
		internal MonadCommand Command
		{
			get
			{
				return this.DataHandler.Command;
			}
		}

		[DDIDataObjectNameExist]
		public string DataObjectName { get; set; }

		[DDIValidLambdaExpression]
		public string CommandTextLambdaExpression
		{
			get
			{
				return this.commandTextLambdaExpression;
			}
			set
			{
				this.commandTextLambdaExpression = value;
			}
		}

		[DDIValidCommandText]
		public string CommandText
		{
			get
			{
				return this.commandText;
			}
			set
			{
				this.commandText = value;
			}
		}

		private SingleTaskDataHandler DataHandler
		{
			get
			{
				if (this.dataHandler == null)
				{
					this.dataHandler = new SingleTaskDataHandler(this.CommandText);
				}
				return this.dataHandler;
			}
		}

		[DefaultValue("")]
		[DDIDataColumnExist]
		public string WorkUnitDescriptionColumn { get; set; }

		public MonadSaveTask() : base(null, null)
		{
		}

		public MonadSaveTask(string commandText, string dataObjectName, string workUnitTextColumn, string workUnitIconColumn) : base(workUnitTextColumn, workUnitIconColumn)
		{
			this.CommandText = commandText;
			this.DataObjectName = dataObjectName;
		}

		public override void Cancel()
		{
			this.DataHandler.Cancel();
		}

		public override void Run(object interactionHandler, DataRow row, DataObjectStore store)
		{
			this.DataHandler.ProgressReport += base.OnProgressReport;
			try
			{
				this.DataHandler.Save(interactionHandler as CommandInteractionHandler);
			}
			finally
			{
				this.DataHandler.ProgressReport -= base.OnProgressReport;
			}
			if (!this.DataHandler.HasWorkUnits || !this.DataHandler.WorkUnits.HasFailures)
			{
				store.ClearModifiedColumns(row, this.DataObjectName);
			}
		}

		public override bool IsRunnable(DataRow row, DataObjectStore store)
		{
			bool flag = true;
			if (!string.IsNullOrEmpty(this.DataObjectName))
			{
				flag = (store.GetModifiedPropertiesBasedOnDataObject(row, this.DataObjectName).Count > 0);
			}
			return flag && base.IsRunnable(row, store);
		}

		public override object WorkUnits
		{
			get
			{
				return this.DataHandler.WorkUnits;
			}
		}

		public override List<object> SavedResults
		{
			get
			{
				return this.DataHandler.SavedResults;
			}
		}

		public override void UpdateWorkUnits(DataRow row)
		{
			this.DataHandler.UpdateWorkUnits();
			if (!string.IsNullOrEmpty(base.WorkUnitTextColumn))
			{
				this.DataHandler.WorkUnit.Text = row[base.WorkUnitTextColumn].ToString();
			}
			else if (!string.IsNullOrEmpty(this.CommandTextLambdaExpression))
			{
				this.DataHandler.WorkUnit.Text = this.DataHandler.CommandText;
			}
			if (string.IsNullOrEmpty(this.WorkUnitDescriptionColumn))
			{
				this.DataHandler.WorkUnit.Description = this.workUnitDescription;
			}
			else
			{
				this.DataHandler.WorkUnit.Description = row[this.WorkUnitDescriptionColumn].ToString();
			}
			if (!string.IsNullOrEmpty(base.WorkUnitIconColumn))
			{
				this.DataHandler.WorkUnit.Icon = WinformsHelper.GetIconFromIconLibrary(row[base.WorkUnitIconColumn].ToString());
			}
			this.DataHandler.ResetCancel();
		}

		public override string CommandToRun
		{
			get
			{
				return this.DataHandler.CommandToRun;
			}
		}

		public override string ModifiedParametersDescription
		{
			get
			{
				return this.DataHandler.ModifiedParametersDescription;
			}
		}

		public override void BuildParameters(DataRow row, DataObjectStore store, IList<ParameterProfile> paramInfos)
		{
			if (!string.IsNullOrEmpty(this.CommandTextLambdaExpression))
			{
				this.DataHandler.CommandText = (string)ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(this.CommandTextLambdaExpression), typeof(string), row, null);
			}
			this.DataHandler.Parameters.Clear();
			if (!string.IsNullOrEmpty(this.DataObjectName))
			{
				this.DataHandler.Parameters.AddWithValue("Instance", store.GetDataObject(this.DataObjectName));
			}
			else
			{
				this.DataHandler.KeepInstanceParamerter = true;
			}
			MonadSaveTask.BuildParametersCore(row, paramInfos, this.DataHandler.Parameters);
			this.workUnitDescription = MonadSaveTask.BuildParametersDescription(row, paramInfos);
			this.DataHandler.ClearParameterNames();
			this.DataHandler.SpecifyParameterNames(store.GetModifiedPropertiesBasedOnDataObject(row, this.DataObjectName));
		}

		public static string BuildParametersDescription(DataRow row, IList<ParameterProfile> paramInfos)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (ParameterProfile parameterProfile in paramInfos)
			{
				if (parameterProfile.IsRunnable(row) && !parameterProfile.HideDisplay)
				{
					switch (parameterProfile.ParameterType)
					{
					case ParameterType.Switched:
						stringBuilder.Append(Strings.NameValueFormat(parameterProfile.Name, string.Empty));
						break;
					case ParameterType.Column:
					case ParameterType.ModifiedColumn:
					{
						object value = MonadSaveTask.ConvertToParameterValue(row, parameterProfile);
						stringBuilder.Append(Strings.NameValueFormat(parameterProfile.Name, MonadCommand.FormatParameterValue(value)));
						break;
					}
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static void BuildParametersCore(DataRow row, IList<ParameterProfile> paramInfos, object parameters)
		{
			MonadParameterCollection monadParameterCollection = parameters as MonadParameterCollection;
			foreach (ParameterProfile parameterProfile in paramInfos)
			{
				if (parameterProfile.IsRunnable(row))
				{
					switch (parameterProfile.ParameterType)
					{
					case ParameterType.Switched:
						monadParameterCollection.AddSwitch(parameterProfile.Name);
						break;
					case ParameterType.Column:
					case ParameterType.ModifiedColumn:
					{
						object value = MonadSaveTask.ConvertToParameterValue(row, parameterProfile);
						monadParameterCollection.AddWithValue(parameterProfile.Name, value);
						break;
					}
					}
				}
			}
		}

		public static object ConvertToParameterValue(DataRow row, string parameterName)
		{
			object obj = row[parameterName];
			if (DBNull.Value.Equals(obj) && row.Table != null && !typeof(string).Equals(row.Table.Columns[parameterName].GetType()))
			{
				obj = null;
			}
			return obj;
		}

		public static object ConvertToParameterValue(DataRow row, ParameterProfile paramInfo)
		{
			if (string.IsNullOrEmpty(paramInfo.LambdaExpression))
			{
				return MonadSaveTask.ConvertToParameterValue(row, paramInfo.Reference);
			}
			return ExpressionCalculator.CalculateLambdaExpression(ExpressionCalculator.BuildColumnExpression(paramInfo.LambdaExpression), typeof(object), row, null);
		}

		public override Saver CreateBulkSaver(WorkUnit[] workunits)
		{
			this.dataHandler = new BulkSaveDataHandler(workunits, this.DataHandler.CommandText);
			return this;
		}

		public bool IgnoreIdentityParameter { get; set; }

		public override bool HasPermission(string propertyName, IList<ParameterProfile> parameters)
		{
			ICollection<string> parameterList = EMCRunspaceConfigurationSingleton.GetInstance().GetAllowedParamsForSetCmdlet(this.commandText, null);
			return parameterList != null && parameterList.Contains(propertyName, new CaseInSensitveComparer()) && (from c in parameters
			where parameterList.Contains(c.Name, new CaseInSensitveComparer())
			select c).Count<ParameterProfile>() == parameters.Count && (this.IgnoreIdentityParameter || parameterList.Contains("Identity", new CaseInSensitveComparer()));
		}

		public override string GetConsumedDataObjectName()
		{
			return this.DataObjectName;
		}

		private SingleTaskDataHandler dataHandler;

		private string workUnitDescription;

		private string commandText;

		private string commandTextLambdaExpression;
	}
}
