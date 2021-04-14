using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class MonadPipelineSaveTask : Saver
	{
		public MonadPipelineSaveTask(string commandText, string dataObjectName, string workUnitTextColumn, string workUnitIconColumn) : base(workUnitTextColumn, workUnitIconColumn)
		{
			this.CommandText = commandText;
			this.dataObjectName = dataObjectName;
		}

		public MonadPipelineSaveTask() : this(null, null, null, null)
		{
		}

		public string CommandText
		{
			get
			{
				return this.originalCommandText;
			}
			set
			{
				this.originalCommandText = value;
				if (value != null)
				{
					this.dataHandler = new SingleTaskDataHandler(string.Empty, new MonadConnection(PSConnectionInfoSingleton.GetInstance().GetConnectionStringForScript(), new CommandInteractionHandler(), ADServerSettingsSingleton.GetInstance().CreateRunspaceServerSettingsObject(), PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo()));
					this.dataHandler.Command.CommandType = CommandType.Text;
				}
			}
		}

		internal MonadCommand Command
		{
			get
			{
				return this.dataHandler.Command;
			}
		}

		public override string CommandToRun
		{
			get
			{
				return this.dataHandler.CommandText;
			}
		}

		public override List<object> SavedResults
		{
			get
			{
				return this.dataHandler.SavedResults;
			}
		}

		public override string ModifiedParametersDescription
		{
			get
			{
				return this.modifiedParametersDescription;
			}
		}

		public override object WorkUnits
		{
			get
			{
				return this.dataHandler.WorkUnits;
			}
		}

		public override void UpdateWorkUnits(DataRow row)
		{
			this.dataHandler.UpdateWorkUnits();
			if (!string.IsNullOrEmpty(base.WorkUnitTextColumn))
			{
				this.dataHandler.WorkUnit.Text = row[base.WorkUnitTextColumn].ToString();
			}
			this.dataHandler.WorkUnit.Description = this.ModifiedParametersDescription;
			if (!string.IsNullOrEmpty(base.WorkUnitIconColumn))
			{
				this.dataHandler.WorkUnit.Icon = WinformsHelper.GetIconFromIconLibrary(row[base.WorkUnitIconColumn].ToString());
			}
			this.dataHandler.ResetCancel();
		}

		public override void BuildParameters(DataRow row, DataObjectStore store, IList<ParameterProfile> paramInfos)
		{
			this.dataHandler.Parameters.Clear();
			this.dataHandler.ClearParameterNames();
			this.modifiedParametersDescription = MonadSaveTask.BuildParametersDescription(row, paramInfos);
			this.dataHandler.CommandText = MonadPipelineSaveTask.BuildCommandScript(this.originalCommandText, row, paramInfos);
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
			if (!this.dataHandler.HasWorkUnits || !this.dataHandler.WorkUnits.HasFailures)
			{
				store.ClearModifiedColumns(row, this.dataObjectName);
			}
		}

		public override void Cancel()
		{
			this.dataHandler.Cancel();
		}

		public static string BuildCommandScript(string rawScript, DataRow row, IList<ParameterProfile> paramInfos)
		{
			IList<string> paramStrings = MonadPipelineSaveTask.BuildParameterStrings(row, paramInfos);
			return MonadPipelineSaveTask.ReplaceParameterPlaceholder(rawScript, paramStrings, paramInfos);
		}

		private static IList<string> BuildParameterStrings(DataRow row, IList<ParameterProfile> paramInfos)
		{
			List<string> list = new List<string>();
			foreach (ParameterProfile parameterProfile in paramInfos)
			{
				if (parameterProfile.IsRunnable(row))
				{
					switch (parameterProfile.ParameterType)
					{
					case ParameterType.Switched:
						list.Add(string.Format("-{0} ", parameterProfile.Name));
						break;
					case ParameterType.Column:
					case ParameterType.ModifiedColumn:
					{
						string arg = MonadCommand.FormatParameterValue(MonadSaveTask.ConvertToParameterValue(row, parameterProfile));
						list.Add(string.Format("-{0} {1} ", parameterProfile.Name, arg));
						break;
					}
					default:
						list.Add(string.Empty);
						break;
					}
				}
				else
				{
					list.Add(string.Empty);
				}
			}
			return list;
		}

		private static string ReplaceParameterPlaceholder(string rawScript, IList<string> paramStrings, IList<ParameterProfile> paramInfos)
		{
			return MonadPipelineSaveTask.placeholderRegex.Replace(rawScript, delegate(Match match)
			{
				string text = match.Value.Substring(1, match.Value.Length - 2);
				int parameterIndex;
				if (!int.TryParse(text, out parameterIndex))
				{
					string paramName = text;
					parameterIndex = MonadPipelineSaveTask.GetParameterIndex(paramName, paramInfos);
				}
				return paramStrings[parameterIndex];
			});
		}

		private static int GetParameterIndex(string paramName, IList<ParameterProfile> paramInfos)
		{
			int num = -1;
			for (int i = 0; i < paramInfos.Count; i++)
			{
				if (paramInfos[i].Name == paramName)
				{
					if (num != -1)
					{
						throw new NotSupportedException(string.Format("Multiple parameters are found for '{0}'.", paramName));
					}
					num = i;
				}
			}
			if (num == -1)
			{
				throw new NotSupportedException(string.Format("Parameter '{0}' is not found.", paramName));
			}
			return num;
		}

		private SingleTaskDataHandler dataHandler;

		private string originalCommandText;

		private string dataObjectName;

		private string modifiedParametersDescription;

		private static Regex placeholderRegex = new Regex("\\{\\w+\\}");
	}
}
