using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AddUmLanguagePackModeDataHandler : UmLanguagePackModeDataHandler
	{
		public AddUmLanguagePackModeDataHandler(ISetupContext setupContext, MonadConnection connection) : base(setupContext, connection)
		{
		}

		public override InstallationModes Mode
		{
			get
			{
				return InstallationModes.Install;
			}
		}

		protected override ConfigurationDataHandler GetInstallableUnitConfigurationDataHandler(string installableUnitName)
		{
			ConfigurationDataHandler configurationDataHandler = null;
			if (!this.addUmConfigDataHandlers.TryGetValue(installableUnitName, out configurationDataHandler))
			{
				InstallableUnitConfigurationInfo installableUnitConfigurationInfoByName = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(installableUnitName);
				UmLanguagePackConfigurationInfo umLanguagePackConfigurationInfo = installableUnitConfigurationInfoByName as UmLanguagePackConfigurationInfo;
				if (umLanguagePackConfigurationInfo != null)
				{
					configurationDataHandler = new AddUmLanguagePackCfgDataHandler(base.SetupContext, base.Connection, umLanguagePackConfigurationInfo.Culture, this.PackageDirectory);
					this.addUmConfigDataHandlers.Add(installableUnitName, configurationDataHandler);
				}
			}
			return configurationDataHandler;
		}

		public override PreCheckDataHandler PreCheckDataHandler
		{
			get
			{
				if (this.preCheckDataHandler == null)
				{
					this.preCheckDataHandler = new InstallPreCheckDataHandler(base.SetupContext, this, base.Connection);
				}
				return this.preCheckDataHandler;
			}
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
			PrerequisiteAnalysisTaskDataHandler instance = PrerequisiteAnalysisTaskDataHandler.GetInstance(base.SetupContext, base.Connection);
			instance.AddRoleByUnitName("UmLanguagePack");
			instance.TargetDir = base.SetupContext.TargetDir;
			instance.AddSelectedInstallableUnits(this.SelectedInstallableUnits);
		}

		public override decimal RequiredDiskSpace
		{
			get
			{
				decimal num = 0m;
				foreach (CultureInfo umlang in base.SelectedCultures)
				{
					num += UmLanguagePackConfigurationInfo.GetUmLanguagePackSizeForCultureInfo(umlang);
				}
				return num;
			}
		}

		public override string RoleSelectionDescription
		{
			get
			{
				return Strings.AddUmLanguagePackText;
			}
		}

		public override LocalizedString ConfigurationSummary
		{
			get
			{
				return Strings.UmLanguagePacksToAdd;
			}
		}

		public LongPath PackageDirectory
		{
			get
			{
				return base.SetupContext.SourceDir;
			}
		}

		private Dictionary<string, ConfigurationDataHandler> addUmConfigDataHandlers = new Dictionary<string, ConfigurationDataHandler>();

		private InstallPreCheckDataHandler preCheckDataHandler;
	}
}
