using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Setup.Common
{
	internal abstract class ConfigurationDataHandler : SetupSingleTaskDataHandler, IPrecheckEnabled
	{
		public ConfigurationDataHandler(ISetupContext context, string installableUnitName, string commandText, MonadConnection connection) : base(context, commandText, connection)
		{
			this.InstallableUnitName = installableUnitName;
			this.isADDependentRole = true;
			base.ImplementsDatacenterMode = true;
			base.ImplementsDatacenterDedicatedMode = true;
			base.ImplementsPartnerHostedMode = true;
		}

		public string InstallableUnitName
		{
			get
			{
				return this.installableUnitName;
			}
			protected set
			{
				if (this.InstallableUnitName != value)
				{
					this.installableUnitName = value;
					if (!string.IsNullOrEmpty(this.InstallableUnitName))
					{
						this.InstallableUnitConfigurationInfo = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(this.InstallableUnitName);
						return;
					}
					this.InstallableUnitConfigurationInfo = null;
				}
			}
		}

		public InstallableUnitConfigurationInfo InstallableUnitConfigurationInfo
		{
			get
			{
				return this.installableUnitConfigurationInfo;
			}
			protected set
			{
				if (this.InstallableUnitConfigurationInfo != value)
				{
					this.installableUnitConfigurationInfo = value;
					if (this.InstallableUnitConfigurationInfo != null)
					{
						this.InstallableUnitName = this.InstallableUnitConfigurationInfo.Name;
						base.WorkUnit.Text = this.InstallableUnitConfigurationInfo.DisplayName;
					}
					else
					{
						this.InstallableUnitName = string.Empty;
						base.WorkUnit.Text = string.Empty;
					}
					base.WorkUnit.Icon = null;
				}
			}
		}

		public List<string> SelectedInstallableUnits { get; set; }

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			string domainController = base.SetupContext.DomainController;
			if (this.isADDependentRole && !string.IsNullOrEmpty(domainController))
			{
				base.Parameters.AddWithValue("DomainController", new Fqdn(domainController));
			}
			SetupLogger.TraceExit();
		}

		public virtual void UpdatePreCheckTaskDataHandler()
		{
			if (this.InstallableUnitName != null)
			{
				PrerequisiteAnalysisTaskDataHandler instance = PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
				instance.AddRoleByUnitName(this.InstallableUnitName);
				instance.TargetDir = base.SetupContext.TargetDir;
				instance.AddSelectedInstallableUnits(this.SelectedInstallableUnits);
			}
		}

		public bool ValidationDelayed
		{
			get
			{
				return this.validationDelayed;
			}
			set
			{
				this.validationDelayed = value;
			}
		}

		public sealed override ValidationError[] Validate()
		{
			List<ValidationError> list = new List<ValidationError>();
			if (!this.ValidationDelayed)
			{
				list.AddRange(this.ValidateConfiguration());
				list.AddRange(base.Validate());
			}
			return list.ToArray();
		}

		public virtual ValidationError[] ValidateConfiguration()
		{
			List<ValidationError> list = new List<ValidationError>();
			if (!string.IsNullOrEmpty(this.InstallableUnitName))
			{
				ConfigurationStatus configurationStatus = new ConfigurationStatus(this.InstallableUnitName);
				RolesUtility.GetConfiguringStatus(ref configurationStatus);
				InstallationModes action = configurationStatus.Action;
				if (action != InstallationModes.Unknown && action != base.SetupContext.InstallationMode && (action != InstallationModes.Install || base.SetupContext.InstallationMode != InstallationModes.Uninstall))
				{
					list.Add(new SetupValidationError(Strings.IllegalResumptionException(action.ToString(), base.SetupContext.InstallationMode.ToString())));
				}
			}
			return list.ToArray();
		}

		protected override void OnSaveData()
		{
			this.LogSetupConfigurationStartEvent();
			base.OnSaveData();
			if (base.WorkUnits.Count > 0 && !this.IsSucceeded)
			{
				this.LogSetupConfigurationFailureEvent();
				return;
			}
			this.LogSetupConfigurationSuccessEvent();
		}

		protected string GetMsiSourcePath()
		{
			return base.SetupContext.SourceDir.PathName + Path.DirectorySeparatorChar;
		}

		private void LogSetupConfigurationStartEvent()
		{
			if (!string.IsNullOrEmpty(this.InstallableUnitName))
			{
				SetupEventLog.LogStartEvent(this.InstallableUnitName);
			}
		}

		private void LogSetupConfigurationSuccessEvent()
		{
			if (!string.IsNullOrEmpty(this.InstallableUnitName))
			{
				SetupEventLog.LogSuccessEvent(this.InstallableUnitName);
			}
		}

		private void LogSetupConfigurationFailureEvent()
		{
			if (!string.IsNullOrEmpty(this.InstallableUnitName))
			{
				string text = base.WorkUnits[0].ErrorsDescription;
				if (text.Length > 32766)
				{
					text = text.Substring(0, 32766);
				}
				SetupEventLog.LogFailureEvent(this.InstallableUnitName, text);
			}
		}

		private string installableUnitName;

		private InstallableUnitConfigurationInfo installableUnitConfigurationInfo;

		protected List<ConfigurationDataHandler> nestedConfigurationDataHandlers = new List<ConfigurationDataHandler>();

		private bool validationDelayed;

		protected bool isADDependentRole;
	}
}
