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
	internal sealed class InstallFileDataHandler : FileDataHandler
	{
		public InstallFileDataHandler(ISetupContext context, MsiConfigurationInfo msiConfig, MonadConnection connection) : base(context, "install-msipackage", msiConfig, connection)
		{
			if (msiConfig is DatacenterMsiConfigurationInfo)
			{
				base.WorkUnit.Text = Strings.CopyDatacenterFileText;
			}
			else
			{
				base.WorkUnit.Text = Strings.CopyFileText;
			}
			this.WatsonEnabled = base.SetupContext.WatsonEnabled;
		}

		protected override void AddParameters()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.AddParameters();
			base.Parameters.AddWithValue("targetdirectory", this.TargetDirectory);
			base.Parameters.AddWithValue("features", base.GetFeatures().ToArray());
			base.Parameters.AddWithValue("packagepath", Path.Combine(this.PackagePath, this.MsiConfigurationInfo.Name));
			base.Parameters.AddWithValue("updatesdir", this.UpdatesDir);
			base.Parameters.AddWithValue("PropertyValues", string.Format("DISABLEERRORREPORTING={0} PRODUCTLANGUAGELCID={1} DEFAULTLANGUAGENAME={2} DEFAULTLANGUAGELCID={3} INSTALLCOMMENT=\"{4}{5}\"", new object[]
			{
				this.WatsonEnabled ? 0 : 1,
				base.SetupContext.ExchangeCulture.LCID,
				base.SetupContext.ExchangeCulture.ThreeLetterWindowsLanguageName,
				base.SetupContext.ExchangeCulture.LCID,
				Strings.InstalledLanguageComment,
				base.SetupContext.ExchangeCulture.EnglishName
			}));
			SetupLogger.TraceExit();
		}

		public override ValidationError[] Validate()
		{
			SetupLogger.TraceEnter(new object[]
			{
				this.PackagePath,
				this.TargetDirectory
			});
			List<ValidationError> list = new List<ValidationError>();
			if (base.SetupContext.ParsedArguments.ContainsKey("sourcedir"))
			{
				if (!Directory.Exists(this.PackagePath))
				{
					list.Add(new SetupValidationError(Strings.MsiDirectoryNotFound(this.PackagePath)));
				}
				else
				{
					string path = Path.Combine(this.PackagePath, this.MsiConfigurationInfo.Name);
					if (!File.Exists(path))
					{
						list.Add(new SetupValidationError(Strings.MsiFileNotFound(this.PackagePath, this.MsiConfigurationInfo.Name)));
					}
				}
				if (this.UpdatesDir != null && !Directory.Exists(this.UpdatesDir.PathName))
				{
					list.Add(new SetupValidationError(Strings.MsiDirectoryNotFound(this.UpdatesDir.PathName)));
				}
			}
			list.AddRange(base.Validate());
			SetupLogger.TraceExit();
			return list.ToArray();
		}

		protected override void OnSaveData()
		{
			SetupLogger.TraceEnter(new object[0]);
			base.OnSaveData();
			ConfigurationContext.Setup.ResetInstallPath();
			SetupLogger.TraceExit();
		}

		private string PackagePath
		{
			get
			{
				if (!(null == base.SetupContext.SourceDir))
				{
					return base.SetupContext.SourceDir.PathName;
				}
				return "";
			}
		}

		public string TargetDirectory
		{
			get
			{
				return this.targetDirectory;
			}
			set
			{
				this.targetDirectory = value;
			}
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

		public LongPath UpdatesDir
		{
			get
			{
				return base.SetupContext.UpdatesDir;
			}
		}

		private bool watsonEnabled;

		private string targetDirectory;
	}
}
