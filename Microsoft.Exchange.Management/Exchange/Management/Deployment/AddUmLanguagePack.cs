using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.CabUtility;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("add", "umlanguagepack", SupportsShouldProcess = true)]
	public sealed class AddUmLanguagePack : ManageUmLanguagePack
	{
		protected override void PopulateContextVariables()
		{
			base.Fields["IsPostInstallUMAddLP"] = true;
			base.PopulateContextVariables();
		}

		[Parameter(Mandatory = true)]
		public LongPath LanguagePackExecutablePath { get; set; }

		private LongPath PackagePath
		{
			get
			{
				return (LongPath)base.Fields["PackagePath"];
			}
			set
			{
				base.Fields["PackagePath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public NonRootLocalLongFullPath InstallPath
		{
			get
			{
				return (NonRootLocalLongFullPath)base.Fields["InstallPath"];
			}
			set
			{
				base.Fields["InstallPath"] = value;
			}
		}

		private LongPath TelePackagePath
		{
			get
			{
				return (LongPath)base.Fields["TelePackagePath"];
			}
			set
			{
				base.Fields["TelePackagePath"] = value;
			}
		}

		private LongPath TransPackagePath
		{
			get
			{
				return (LongPath)base.Fields["TransPackagePath"];
			}
			set
			{
				base.Fields["TransPackagePath"] = value;
			}
		}

		private LongPath TtsPackagePath
		{
			get
			{
				return (LongPath)base.Fields["TtsPackagePath"];
			}
			set
			{
				base.Fields["TtsPackagePath"] = value;
			}
		}

		public AddUmLanguagePack()
		{
			base.Fields["InstallationMode"] = InstallationModes.Install;
		}

		protected override void InternalValidate()
		{
			if (!UMLanguagePackHelper.IsUmLanguagePack(this.LanguagePackExecutablePath.PathName))
			{
				base.WriteError(new TaskException(Strings.ADDUMInvalidLanguagePack(this.LanguagePackExecutablePath.PathName)), ErrorCategory.InvalidArgument, 0);
			}
			base.InternalValidate();
		}

		protected override void InternalProcessRecord()
		{
			LongPath packagePath = null;
			LongPath telePackagePath = null;
			LongPath transPackagePath = null;
			LongPath ttsPackagePath = null;
			try
			{
				DirectoryInfo directoryInfo = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
				this.extractionDirectory = directoryInfo.FullName;
				EmbeddedCabWrapper.ExtractFiles(this.LanguagePackExecutablePath.PathName, this.extractionDirectory, null);
				string[] directories = Directory.GetDirectories(this.extractionDirectory);
				if (directories.Length != 1)
				{
					base.WriteError(new TaskException(Strings.UmLanguagePackInvalidExtraction), ErrorCategory.NotSpecified, 0);
				}
				string extractionPathWithLanguageFolder = directories[0];
				this.SetPackagePath(extractionPathWithLanguageFolder, "UMLanguagePack", ref packagePath);
				this.SetPackagePath(extractionPathWithLanguageFolder, "MSSpeech_SR_TELE", ref telePackagePath);
				this.SetPackagePath(extractionPathWithLanguageFolder, "MSSpeech_SR_TRANS", ref transPackagePath);
				this.SetPackagePath(extractionPathWithLanguageFolder, "MSSpeech_TTS", ref ttsPackagePath);
			}
			catch (Exception ex)
			{
				base.WriteError(new TaskException(Strings.UmLanguagePackException(ex.Message)), ErrorCategory.NotSpecified, 0);
			}
			this.PackagePath = packagePath;
			this.TelePackagePath = telePackagePath;
			this.TransPackagePath = transPackagePath;
			this.TtsPackagePath = ttsPackagePath;
			if (!File.Exists(this.PackagePath.PathName))
			{
				base.WriteError(new TaskException(Strings.UmLanguagePackMsiFileNotFound(this.PackagePath.PathName)), ErrorCategory.InvalidArgument, 0);
			}
			if (!File.Exists(this.TelePackagePath.PathName))
			{
				base.WriteError(new TaskException(Strings.UmLanguagePackMsiFileNotFound(this.TelePackagePath.PathName)), ErrorCategory.InvalidArgument, 0);
			}
			if (!File.Exists(this.TransPackagePath.PathName))
			{
				this.TransPackagePath = null;
			}
			if (!File.Exists(this.TtsPackagePath.PathName))
			{
				base.WriteError(new TaskException(Strings.UmLanguagePackMsiFileNotFound(this.TtsPackagePath.PathName)), ErrorCategory.InvalidArgument, 0);
			}
			base.InternalProcessRecord();
		}

		protected override void InternalEndProcessing()
		{
			if (!string.IsNullOrEmpty(this.extractionDirectory) && Directory.Exists(this.extractionDirectory))
			{
				try
				{
					Directory.Delete(this.extractionDirectory, true);
				}
				catch (Exception ex)
				{
					this.WriteWarning(Strings.UmLanguagePackTempFilesNotDeleted(ex.Message));
				}
			}
			base.InternalEndProcessing();
		}

		private void SetPackagePath(string extractionPathWithLanguageFolder, string packagePrefix, ref LongPath packagePath)
		{
			string path = string.Format("{0}.{1}.msi", packagePrefix, base.Language.ToString());
			string path2 = Path.Combine(extractionPathWithLanguageFolder, path);
			if (!LongPath.TryParse(path2, out packagePath))
			{
				packagePath = null;
				base.WriteError(new TaskException(Strings.UmLanguagePackPackagePathNotSpecified), ErrorCategory.NotSpecified, 0);
			}
		}

		private string extractionDirectory;
	}
}
