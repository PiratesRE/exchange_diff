using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SnapIn;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class OrganizationRelationshipSaveTask : Saver
	{
		public OrganizationRelationshipSaveTask(string commandText, string dataObjectName, string workUnitTextColumn, string workUnitIconColumn) : base(workUnitTextColumn, workUnitIconColumn)
		{
			this.CommandText = commandText;
			this.dataObjectName = dataObjectName;
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
			}
		}

		private SingleTaskDataHandler DataHandler
		{
			get
			{
				if (this.dataHandler == null)
				{
					this.dataHandler = new SingleTaskDataHandler(this.CommandText, new MonadConnection("pooled=false", new CommandInteractionHandler(), ADServerSettingsSingleton.GetInstance().CreateRunspaceServerSettingsObject(), PSConnectionInfoSingleton.GetInstance().GetMonadConnectionInfo()));
					this.dataHandler.Command.CommandType = CommandType.Text;
				}
				return this.dataHandler;
			}
		}

		public OrganizationRelationshipSaveTask() : this(null, null, null, null)
		{
		}

		public override void UpdateWorkUnits(DataRow row)
		{
			this.DataHandler.UpdateWorkUnits();
			if (!string.IsNullOrEmpty(base.WorkUnitTextColumn))
			{
				this.DataHandler.WorkUnit.Text = row[base.WorkUnitTextColumn].ToString();
			}
			this.DataHandler.WorkUnit.Description = this.ModifiedParametersDescription;
			if (!string.IsNullOrEmpty(base.WorkUnitIconColumn))
			{
				this.DataHandler.WorkUnit.Icon = WinformsHelper.GetIconFromIconLibrary(row[base.WorkUnitIconColumn].ToString());
			}
			this.DataHandler.ResetCancel();
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
				store.ClearModifiedColumns(row, this.dataObjectName);
			}
		}

		public override void BuildParameters(DataRow row, DataObjectStore store, IList<ParameterProfile> paramInfos)
		{
			this.DataHandler.Parameters.Clear();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			if ((bool)row[this.AutoDiscovery])
			{
				string text = MonadCommand.FormatParameterValue(row[this.AutoDiscoveryDomain]);
				stringBuilder2.AppendFormat(this.AutoDiscoveryCommandText, text);
				stringBuilder.Append(Strings.NameValueFormat(this.AutoDiscoveryDomain, text));
			}
			stringBuilder2.Append(this.originalCommandText);
			stringBuilder2.Append(" ");
			foreach (ParameterProfile parameterProfile in paramInfos)
			{
				if ((store.ModifiedColumns.Contains(parameterProfile.Reference) || this.Identity.Equals(parameterProfile.Name)) && parameterProfile.IsRunnable(row))
				{
					switch (parameterProfile.ParameterType)
					{
					case ParameterType.Switched:
						stringBuilder2.AppendFormat("-{0} ", parameterProfile.Name);
						stringBuilder.AppendLine(parameterProfile.Name);
						break;
					case ParameterType.Column:
					case ParameterType.ModifiedColumn:
					{
						string text2 = MonadCommand.FormatParameterValue(MonadSaveTask.ConvertToParameterValue(row, parameterProfile.Reference));
						stringBuilder.Append(Strings.NameValueFormat(parameterProfile.Name, text2));
						if (this.Identity.Equals(parameterProfile.Name) && row[parameterProfile.Reference] is ADObjectId)
						{
							text2 = string.Format("'{0}'", ((ADObjectId)row[parameterProfile.Reference]).ObjectGuid.ToString());
						}
						stringBuilder2.AppendFormat("-{0} {1} ", parameterProfile.Name, text2);
						break;
					}
					}
				}
			}
			this.modifiedParametersDescription = stringBuilder.ToString();
			this.DataHandler.CommandText = stringBuilder2.ToString();
			this.DataHandler.ClearParameterNames();
		}

		public override bool IsRunnable(DataRow row, DataObjectStore store)
		{
			bool flag = (bool)row[this.AutoDiscovery];
			if (!flag)
			{
				int num = store.ModifiedColumns.Count;
				if (store.ModifiedColumns.Contains(this.AutoDiscovery))
				{
					num--;
				}
				if (store.ModifiedColumns.Contains(this.AutoDiscoveryDomain))
				{
					num--;
				}
				flag = (num > 0);
			}
			return flag;
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

		public override string CommandToRun
		{
			get
			{
				return this.DataHandler.CommandText;
			}
		}

		public override string ModifiedParametersDescription
		{
			get
			{
				return this.modifiedParametersDescription;
			}
		}

		public override bool HasPermission(string propertyName, IList<ParameterProfile> parameters)
		{
			return EMCRunspaceConfigurationSingleton.GetInstance().IsCmdletAllowedInScope(this.CommandText, new string[]
			{
				propertyName,
				"Identity"
			});
		}

		private readonly string AutoDiscovery = "AutoDiscovery";

		private readonly string Identity = "Identity";

		private readonly string AutoDiscoveryDomain = "AutoDiscoveryDomain";

		private readonly string AutoDiscoveryCommandText = "Get-FederationInformation -DomainName {0} | ";

		private SingleTaskDataHandler dataHandler;

		private string originalCommandText;

		private string dataObjectName;

		private string modifiedParametersDescription;
	}
}
