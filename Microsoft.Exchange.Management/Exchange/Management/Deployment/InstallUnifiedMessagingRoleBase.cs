using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class InstallUnifiedMessagingRoleBase : ManageUnifiedMessagingRole
	{
		protected override void PopulateContextVariables()
		{
			base.Fields["IsPostInstallUMAddLP"] = false;
			base.PopulateContextVariables();
			if (!base.Fields.IsModified("SourcePath"))
			{
				this.SourcePath = this.GetMsiSourcePath();
			}
			string text = Path.Combine((string)base.Fields["SourcePath"], "en\\");
			base.WriteVerbose(Strings.UmLanguagePackDirectory(text));
			this.PackagePath = Path.Combine(text, "UMLanguagePack.en-US.msi");
			base.WriteVerbose(Strings.UmLanguagePackFullPath(this.PackagePath));
			this.TelePackagePath = Path.Combine(text, "MSSpeech_SR_TELE.en-US.msi");
			base.WriteVerbose(Strings.UmLanguagePackFullPath(this.TelePackagePath));
			this.TransPackagePath = Path.Combine(text, "MSSpeech_SR_TRANS.en-US.msi");
			base.WriteVerbose(Strings.UmLanguagePackFullPath(this.TransPackagePath));
			this.TtsPackagePath = Path.Combine(text, "MSSpeech_TTS.en-US.msi");
			base.WriteVerbose(Strings.UmLanguagePackFullPath(this.TtsPackagePath));
			base.LogFilePath = Path.Combine((string)base.Fields["SetupLoggingPath"], "add-UMLanguagePack.en-US.msilog");
			base.WriteVerbose(Strings.UmLanguagePackLogFile(base.LogFilePath));
		}

		private string GetMsiSourcePath()
		{
			string text = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{4934D1EA-BE46-48B1-8847-F1AF20E892C1}", "InstallSource", null);
			if (text == null)
			{
				base.WriteError(new LocalizedException(Strings.ExceptionRegistryKeyNotFound("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{4934D1EA-BE46-48B1-8847-F1AF20E892C1}\\InstallSource")), ErrorCategory.InvalidData, null);
			}
			return text;
		}

		[Parameter(Mandatory = false)]
		public string SourcePath
		{
			get
			{
				return (string)base.Fields["SourcePath"];
			}
			set
			{
				base.Fields["SourcePath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string PackagePath
		{
			get
			{
				return (string)base.Fields["PackagePath"];
			}
			set
			{
				base.Fields["PackagePath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TelePackagePath
		{
			get
			{
				return (string)base.Fields["TelePackagePath"];
			}
			set
			{
				base.Fields["TelePackagePath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TransPackagePath
		{
			get
			{
				return (string)base.Fields["TransPackagePath"];
			}
			set
			{
				base.Fields["TransPackagePath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TtsPackagePath
		{
			get
			{
				return (string)base.Fields["TtsPackagePath"];
			}
			set
			{
				base.Fields["TtsPackagePath"] = value;
			}
		}
	}
}
