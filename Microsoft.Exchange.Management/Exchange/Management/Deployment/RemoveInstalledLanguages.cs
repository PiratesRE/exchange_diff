using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("remove", "installedlanguages", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class RemoveInstalledLanguages : ComponentInfoBasedTask
	{
		public RemoveInstalledLanguages()
		{
			base.ComponentInfoFileNames = new List<string>();
			base.ImplementsResume = false;
			base.Fields["InstallationMode"] = InstallationModes.Uninstall;
			base.ComponentInfoFileNames.Add("setup\\data\\LanguagePackUninstallationComponent.xml");
			DateTime dateTime = (DateTime)ExDateTime.Now;
			base.Fields["LogDateTime"] = dateTime.ToString("yyyyMMdd-HHmmss");
		}

		[Parameter(Mandatory = true)]
		public InstallationModes InstallMode
		{
			get
			{
				return this.InstallationMode;
			}
			set
			{
				this.InstallationMode = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath LogFilePath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["LogFilePath"];
			}
			set
			{
				base.Fields["LogFilePath"] = value;
			}
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.LanguagePackDescription;
			}
		}

		[Parameter(Mandatory = false)]
		public string PropertyValues
		{
			get
			{
				return (string)base.Fields["PropertyValues"];
			}
			set
			{
				base.Fields["PropertyValues"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string[] LanguagePacksToRemove
		{
			get
			{
				return (string[])base.Fields["LanguagePacksToRemove"];
			}
			set
			{
				base.Fields["LanguagePacksToRemove"] = value;
			}
		}
	}
}
