using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AddLanguagePackFileCopyDataHandler : SetupSingleTaskDataHandler
	{
		public AddLanguagePackFileCopyDataHandler(ISetupContext context, MonadConnection connection) : base(context, "install-LanguageFiles", connection)
		{
			LocalLongFullPath.TryParse(ConfigurationContext.Setup.SetupLoggingPath, out this.logFilePath);
			base.WorkUnit.Text = Strings.CopyLanguagePacksDisplayName;
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			SetupLogger.Log(Strings.LanguagePackPackagePath((base.SetupContext.LanguagePackPath == null) ? "" : base.SetupContext.LanguagePackPath.PathName));
			base.Parameters.AddWithValue("LangPackPath", base.SetupContext.LanguagePackPath);
			SetupLogger.Log(Strings.AddLanguagePacksLogFilePath((this.LogFilePath == null) ? "" : this.LogFilePath.PathName));
			base.Parameters.AddWithValue("LogFilePath", this.LogFilePath);
			base.Parameters.AddWithValue("InstallPath", this.TargetDir);
			base.Parameters.AddWithValue("SourceIsBundle", base.SetupContext.LanguagePackSourceIsBundle);
			string[] array = new string[base.SetupContext.LanguagePacksToInstall.Keys.Count];
			string[] array2 = new string[base.SetupContext.LanguagePacksToInstall.Keys.Count];
			string[] array3 = new string[base.SetupContext.LanguagePacksToInstall.Keys.Count];
			int num = 0;
			foreach (KeyValuePair<string, Array> keyValuePair in base.SetupContext.LanguagePacksToInstall)
			{
				array[num] = keyValuePair.Key.ToString();
				array2[num] = keyValuePair.Value.GetValue(0).ToString();
				array3[num] = keyValuePair.Value.GetValue(1).ToString();
				num++;
			}
			base.Parameters.AddWithValue("LanguagePacksToInstall", array);
			base.Parameters.AddWithValue("LPclientFlags", array2);
			base.Parameters.AddWithValue("LPserverFlags", array3);
			SetupLogger.TraceExit();
		}

		public virtual ValidationError[] ValidateConfiguration()
		{
			SetupLogger.TraceEnter(new object[0]);
			List<ValidationError> list = new List<ValidationError>();
			if (base.SetupContext.LanguagePackPath == null)
			{
				list.Add(new SetupValidationError(Strings.LanguagePacksPackagePathNotSpecified));
			}
			else if (this.LogFilePath == null)
			{
				list.Add(new SetupValidationError(Strings.LanguagePacksLogFilePathNotSpecified));
			}
			else if (!File.Exists(base.SetupContext.LanguagePackPath.PathName) && !Directory.Exists(base.SetupContext.LanguagePackPath.PathName))
			{
				list.Add(new SetupValidationError(Strings.LanguagePacksPackagePathNotFound(base.SetupContext.LanguagePackPath.PathName)));
			}
			if (this.TargetDir == null)
			{
				list.Add(new SetupValidationError(Strings.InstallationPathNotSet));
			}
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		public LocalLongFullPath LogFilePath
		{
			get
			{
				return this.logFilePath;
			}
		}

		public NonRootLocalLongFullPath TargetDir
		{
			get
			{
				return base.SetupContext.TargetDir;
			}
		}

		public List<string> SelectedInstallableUnits
		{
			get
			{
				return this.selectedInstallableUnits;
			}
			set
			{
				this.selectedInstallableUnits = value;
			}
		}

		private const string commandText = "install-LanguageFiles";

		private LocalLongFullPath logFilePath;

		private List<string> selectedInstallableUnits;
	}
}
