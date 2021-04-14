using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageUmLanguagePack : ComponentInfoBasedTask
	{
		public ManageUmLanguagePack()
		{
			base.Fields["InstallationMode"] = InstallationModes.Install;
			base.ComponentInfoFileNames = new List<string>();
			this.ShouldRestartUMService = true;
			base.ImplementsResume = false;
		}

		[Parameter(Mandatory = true)]
		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)base.Fields["Language"];
			}
			set
			{
				base.Fields["Language"] = value;
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

		[Parameter(Mandatory = false)]
		public bool ShouldRestartUMService
		{
			get
			{
				return (bool)base.Fields["ShouldRestartUMService"];
			}
			set
			{
				base.Fields["ShouldRestartUMService"] = value;
			}
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.UmLanguagePackDescription(this.Language.ToString());
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

		protected override void InternalProcessRecord()
		{
			if (this.ShouldRestartUMService)
			{
				base.ComponentInfoFileNames.Add("setup\\data\\UmLanguagePackInitialization.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\UmLanguagePackComponent.xml");
				base.ComponentInfoFileNames.Add("setup\\data\\UnifiedMessagingFinalization.xml");
			}
			else
			{
				base.ComponentInfoFileNames.Add("setup\\data\\UmLanguagePackComponent.xml");
			}
			base.InternalProcessRecord();
		}

		protected override void InternalBeginProcessing()
		{
			RegistryKey registryKey = null;
			try
			{
				registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Microsoft Speech Server\\2.0\\Applications\\ExUM", true);
				if (registryKey != null)
				{
					registryKey.SetValue("PreloadedResourceManifestXml", string.Empty, RegistryValueKind.String);
					registryKey.SetValue("PreloadedResourceManifest", string.Empty, RegistryValueKind.String);
				}
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
			base.InternalBeginProcessing();
		}
	}
}
