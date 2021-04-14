using System;
using System.Diagnostics;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PreCheckDataHandler : SetupSingleTaskDataHandler
	{
		protected PreCheckDataHandler(ISetupContext context, DataHandler topLevelHandler, MonadConnection connection) : base(context, "", connection)
		{
			this.TopLevelHandler = topLevelHandler;
			base.BreakOnError = false;
		}

		public DataHandler TopLevelHandler
		{
			get
			{
				return this.topLevelHandler;
			}
			set
			{
				this.topLevelHandler = value;
			}
		}

		public abstract string ShortDescription { get; }

		public abstract string Title { get; }

		public override void UpdateWorkUnits()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.DataHandlers.Clear();
			if (this.IsPreConfigNeeded())
			{
				if (this.preConfigHandler == null)
				{
					this.preConfigHandler = new PreConfigTaskDataHandler(base.SetupContext, base.Connection);
					this.preConfigHandler.SelectedInstallableUnits = ((ModeDataHandler)this.TopLevelHandler).SelectedInstallableUnits;
				}
				if (this.preConfigHandler.Roles.Length != 0)
				{
					base.DataHandlers.Add(this.preConfigHandler);
				}
			}
			this.AddPrecheckHandler(this.TopLevelHandler);
			foreach (DataHandler handler in this.TopLevelHandler.DataHandlers)
			{
				this.AddPrecheckHandler(handler);
			}
			this.AddAnalysisTaskDataHandler();
			if (base.HasDataHandlers)
			{
				base.UpdateWorkUnits();
			}
			else
			{
				base.WorkUnits.Clear();
			}
			SetupLogger.TraceExit();
		}

		protected override void OnSaveData()
		{
			UserChoiceState userChoiceState = new UserChoiceState();
			userChoiceState.ReadFromContext(base.SetupContext, this.TopLevelHandler as ModeDataHandler);
			userChoiceState.SaveToFile();
			base.OnSaveData();
		}

		private bool IsPreConfigNeeded()
		{
			foreach (DataHandler dataHandler in this.TopLevelHandler.DataHandlers)
			{
				IPrecheckEnabled precheckEnabled = dataHandler as IPrecheckEnabled;
				if (precheckEnabled != null)
				{
					if (dataHandler is ConfigurationDataHandler && !(dataHandler is UpgradeCfgDataHandler))
					{
						ConfigurationDataHandler configurationDataHandler = (ConfigurationDataHandler)dataHandler;
						if (base.SetupContext.IsPartiallyConfigured(configurationDataHandler.InstallableUnitName))
						{
							continue;
						}
					}
					return true;
				}
			}
			return false;
		}

		[Conditional("DEBUG")]
		private void AssertAllRequestedRolesArePartiallyConfigured(DataHandler topLevelHandler)
		{
			foreach (string text in (topLevelHandler as ModeDataHandler).SelectedInstallableUnits)
			{
				if (!InstallableUnitConfigurationInfoManager.IsUmLanguagePackInstallableUnit(text) && !InstallableUnitConfigurationInfoManager.IsLanguagePacksInstallableUnit(text))
				{
					bool isDatacenterOnly = RoleManager.GetRoleByName(text).IsDatacenterOnly;
				}
			}
		}

		private void AddPrecheckHandler(DataHandler handler)
		{
			IPrecheckEnabled precheckEnabled = handler as IPrecheckEnabled;
			if (precheckEnabled != null)
			{
				if (handler is ConfigurationDataHandler && !(handler is UpgradeCfgDataHandler))
				{
					ConfigurationDataHandler configurationDataHandler = (ConfigurationDataHandler)handler;
					if (base.SetupContext.IsPartiallyConfigured(configurationDataHandler.InstallableUnitName))
					{
						return;
					}
				}
				precheckEnabled.UpdatePreCheckTaskDataHandler();
			}
		}

		private void AddAnalysisTaskDataHandler()
		{
			PrerequisiteAnalysisTaskDataHandler instance = PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
			if (instance.HasRoles)
			{
				instance.InitializeParameters();
				base.DataHandlers.Add(instance);
			}
		}

		private DataHandler topLevelHandler;

		private PreConfigTaskDataHandler preConfigHandler;
	}
}
