using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AddUmLanguagePackCfgDataHandler : ConfigurationDataHandler
	{
		public AddUmLanguagePackCfgDataHandler(ISetupContext context, MonadConnection connection, CultureInfo umlang, LongPath directoryContainingLanguagePack) : base(context, "", "add-umlanguagepack", connection)
		{
			this.Culture = umlang;
			string umLanguagePackNameForCultureInfo = UmLanguagePackConfigurationInfo.GetUmLanguagePackNameForCultureInfo(this.Culture);
			InstallableUnitConfigurationInfo installableUnitConfigurationInfo = InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(umLanguagePackNameForCultureInfo);
			if (installableUnitConfigurationInfo == null)
			{
				installableUnitConfigurationInfo = new UmLanguagePackConfigurationInfo(this.Culture);
				InstallableUnitConfigurationInfoManager.AddInstallableUnit(umLanguagePackNameForCultureInfo, installableUnitConfigurationInfo);
			}
			base.InstallableUnitName = installableUnitConfigurationInfo.Name;
			this.logFilePath = UMLanguagePackHelper.GetAddUMLanguageLogPath(ConfigurationContext.Setup.SetupLoggingPath, this.Culture);
			this.LanguagePackExecutablePath = UMLanguagePackHelper.GetUMLanguagePackFilename(directoryContainingLanguagePack.PathName, this.Culture);
			if (this.LanguagePackExecutablePath != null)
			{
				SetupLogger.Log(Strings.PackagePathSetTo(this.LanguagePackExecutablePath.PathName));
			}
			this.shouldRestartUMService = !base.SetupContext.IsDatacenter;
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			SetupLogger.Log(Strings.UmLanguagePackPackagePath((this.LanguagePackExecutablePath == null) ? "" : this.LanguagePackExecutablePath.PathName));
			base.Parameters.AddWithValue("LanguagePackExecutablePath", this.LanguagePackExecutablePath);
			SetupLogger.Log(Strings.AddUmLanguagePackLogFilePath((this.LogFilePath == null) ? "" : this.LogFilePath.PathName));
			base.Parameters.AddWithValue("logfilepath", this.LogFilePath);
			base.Parameters.AddWithValue("propertyvalues", string.Format("INSTALLDIR='{0}' ESE=1", this.LanguagePackExecutablePath));
			base.Parameters.AddWithValue("Language", this.Culture);
			base.Parameters.AddWithValue("InstallPath", this.InstallPath);
			base.Parameters.AddWithValue("updatesdir", this.UpdatesDir);
			base.Parameters.AddWithValue("ShouldRestartUMService", this.shouldRestartUMService);
			SetupLogger.TraceExit();
		}

		public override ValidationError[] ValidateConfiguration()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>(base.ValidateConfiguration());
			if (this.LanguagePackExecutablePath == null)
			{
				list.Add(new SetupValidationError(Strings.UmLanguagePackPackagePathNotSpecified));
			}
			else if (this.LogFilePath == null)
			{
				list.Add(new SetupValidationError(Strings.UmLanguagePathLogFilePathNotSpecified));
			}
			else if (!File.Exists(this.LanguagePackExecutablePath.PathName))
			{
				list.Add(new SetupValidationError(Strings.UmLanguagePackFileNotFound(this.LanguagePackExecutablePath.PathName)));
			}
			if (this.InstallPath == null)
			{
				list.Add(new SetupValidationError(Strings.InstallationPathNotSet));
			}
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		public override void UpdatePreCheckTaskDataHandler()
		{
		}

		public bool WatsonEnabled
		{
			get
			{
				return this.watsonEnabled;
			}
			set
			{
				this.watsonEnabled = value;
			}
		}

		private LongPath LanguagePackExecutablePath
		{
			get
			{
				return this.languagePackExecutablePath;
			}
			set
			{
				this.languagePackExecutablePath = value;
			}
		}

		private LongPath UpdatesDir
		{
			get
			{
				return base.SetupContext.UpdatesDir;
			}
			set
			{
				base.SetupContext.UpdatesDir = value;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.umlang;
			}
			private set
			{
				this.umlang = value;
			}
		}

		public LocalLongFullPath LogFilePath
		{
			get
			{
				return this.logFilePath;
			}
		}

		public NonRootLocalLongFullPath InstallPath
		{
			get
			{
				return base.SetupContext.TargetDir;
			}
		}

		private bool watsonEnabled;

		private CultureInfo umlang;

		private LocalLongFullPath logFilePath;

		private LongPath languagePackExecutablePath;

		private readonly bool shouldRestartUMService;
	}
}
