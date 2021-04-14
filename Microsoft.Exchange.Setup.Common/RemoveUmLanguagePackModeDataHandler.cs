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
	internal sealed class RemoveUmLanguagePackModeDataHandler : UmLanguagePackModeDataHandler
	{
		public RemoveUmLanguagePackModeDataHandler(ISetupContext setupContext, MonadConnection connection) : base(setupContext, connection)
		{
		}

		public override InstallationModes Mode
		{
			get
			{
				return InstallationModes.Uninstall;
			}
		}

		public override PreCheckDataHandler PreCheckDataHandler
		{
			get
			{
				if (this.preCheckDataHandler == null)
				{
					this.preCheckDataHandler = new UninstallPreCheckDataHandler(base.SetupContext, this, base.Connection);
				}
				return this.preCheckDataHandler;
			}
		}

		protected override ConfigurationDataHandler GetInstallableUnitConfigurationDataHandler(string installableUnitName)
		{
			ConfigurationDataHandler configurationDataHandler = null;
			if (!this.removeUmConfigDataHandlers.TryGetValue(installableUnitName, out configurationDataHandler))
			{
				InstallableUnitConfigurationInfo installableUnitConfigurationInfoByName = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(installableUnitName);
				UmLanguagePackConfigurationInfo umLanguagePackConfigurationInfo = installableUnitConfigurationInfoByName as UmLanguagePackConfigurationInfo;
				if (umLanguagePackConfigurationInfo != null)
				{
					configurationDataHandler = new RemoveUmLanguagePackCfgDataHandler(base.SetupContext, base.Connection, umLanguagePackConfigurationInfo.Culture);
					this.removeUmConfigDataHandlers.Add(installableUnitName, configurationDataHandler);
				}
			}
			return configurationDataHandler;
		}

		public override decimal RequiredDiskSpace
		{
			get
			{
				return 0m;
			}
		}

		public override ValidationError[] Validate()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>();
			if (base.InstalledUMLanguagePacks.Count == 0)
			{
				list.Add(new SetupValidationError(Strings.NoUmLanguagePackInstalled));
			}
			else
			{
				foreach (CultureInfo cultureInfo in base.SelectedCultures)
				{
					if (cultureInfo.ToString().ToLower() == "en-us")
					{
						list.Add(new SetupValidationError(Strings.CannotRemoveEnglishUSLanguagePack));
					}
					else
					{
						bool flag = false;
						foreach (CultureInfo cultureInfo2 in base.InstalledUMLanguagePacks)
						{
							if (cultureInfo2.Name.ToLower() == cultureInfo.ToString().ToLower())
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							list.Add(new SetupValidationError(Strings.UmLanguagePackNotInstalledForCulture(cultureInfo.ToString())));
						}
					}
				}
			}
			list.AddRange(base.Validate());
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		public override string RoleSelectionDescription
		{
			get
			{
				return Strings.RemoveUmLanguagePackText;
			}
		}

		public override LocalizedString ConfigurationSummary
		{
			get
			{
				return Strings.UmLanguagePacksToRemove;
			}
		}

		public override string CompletionDescription
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.RemoveUmLanguagePackFailText : Strings.RemoveUmLanguagePackSuccessText;
			}
		}

		public override string CompletionStatus
		{
			get
			{
				return base.WorkUnits.HasFailures ? Strings.RemoveUmLanguagePackFailStatus : Strings.RemoveSuccessStatus;
			}
		}

		private Dictionary<string, ConfigurationDataHandler> removeUmConfigDataHandlers = new Dictionary<string, ConfigurationDataHandler>();

		private UninstallPreCheckDataHandler preCheckDataHandler;
	}
}
