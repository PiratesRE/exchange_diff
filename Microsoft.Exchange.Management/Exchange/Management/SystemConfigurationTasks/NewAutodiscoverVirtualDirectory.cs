using System;
using System.Collections;
using System.Collections.Specialized;
using System.DirectoryServices;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "AutodiscoverVirtualDirectory", SupportsShouldProcess = true)]
	public sealed class NewAutodiscoverVirtualDirectory : NewExchangeServiceVirtualDirectory<ADAutodiscoverVirtualDirectory>
	{
		public NewAutodiscoverVirtualDirectory()
		{
			base.AppPoolManagedPipelineMode = MetabasePropertyTypes.ManagedPipelineMode.Integrated;
		}

		[Parameter(Mandatory = false)]
		public bool WSSecurityAuthentication
		{
			get
			{
				return base.Fields["WSSecurityAuthentication"] != null && (bool)base.Fields["WSSecurityAuthentication"];
			}
			set
			{
				base.Fields["WSSecurityAuthentication"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OAuthAuthentication
		{
			get
			{
				return base.Fields["OAuthAuthentication"] != null && (bool)base.Fields["OAuthAuthentication"];
			}
			set
			{
				base.Fields["OAuthAuthentication"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAutodiscoverVirtualDirectory;
			}
		}

		protected override string VirtualDirectoryName
		{
			get
			{
				return "Autodiscover";
			}
		}

		protected override string VirtualDirectoryPath
		{
			get
			{
				if (base.Role != VirtualDirectoryRole.ClientAccess)
				{
					return "ClientAccess\\Autodiscover";
				}
				return "FrontEnd\\HttpProxy\\Autodiscover";
			}
		}

		protected override string DefaultApplicationPoolId
		{
			get
			{
				return "MSExchangeAutodiscoverAppPool";
			}
		}

		protected override void SetDefaultAuthenticationMethods(ADExchangeServiceVirtualDirectory virtualDirectory)
		{
			virtualDirectory.WindowsAuthentication = new bool?(true);
			virtualDirectory.WSSecurityAuthentication = new bool?(true);
			virtualDirectory.BasicAuthentication = new bool?(base.Role == VirtualDirectoryRole.ClientAccess);
			virtualDirectory.OAuthAuthentication = new bool?(base.Role == VirtualDirectoryRole.ClientAccess);
			virtualDirectory.DigestAuthentication = new bool?(false);
			virtualDirectory.LiveIdBasicAuthentication = new bool?(false);
			virtualDirectory.LiveIdNegotiateAuthentication = new bool?(false);
		}

		protected override IConfigurable PrepareDataObject()
		{
			if (!base.Fields.Contains("ExtendedProtectionTokenChecking"))
			{
				base.Fields["ExtendedProtectionTokenChecking"] = ExtendedProtectionTokenCheckingMode.None;
			}
			if (!base.Fields.Contains("ExtendedProtectionSPNList"))
			{
				base.Fields["ExtendedProtectionSPNList"] = null;
			}
			if (!base.Fields.Contains("ExtendedProtectionFlags"))
			{
				base.Fields["ExtendedProtectionFlags"] = ExtendedProtectionFlag.None;
			}
			ADAutodiscoverVirtualDirectory adautodiscoverVirtualDirectory = (ADAutodiscoverVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (base.Fields["WSSecurityAuthentication"] != null)
			{
				adautodiscoverVirtualDirectory.WSSecurityAuthentication = new bool?(this.WSSecurityAuthentication);
			}
			if (base.Fields["OAuthAuthentication"] != null)
			{
				adautodiscoverVirtualDirectory.OAuthAuthentication = new bool?(this.OAuthAuthentication);
			}
			return adautodiscoverVirtualDirectory;
		}

		protected override void InternalProcessMetabase()
		{
			TaskLogger.LogEnter();
			base.InternalProcessMetabase();
			if (base.Role == VirtualDirectoryRole.ClientAccess && Datacenter.IsMicrosoftHostedOnly(false) && ((Environment.OSVersion.Version.Major >= 6 && Environment.OSVersion.Version.Minor >= 1) || Environment.OSVersion.Version.Major >= 7))
			{
				if (DirectoryEntry.Exists(this.DataObject.MetabasePath))
				{
					TaskLogger.Trace("MultiTenancy mode: installing AuthModuleFilter isapi filter", new object[0]);
					try
					{
						NewAutodiscoverVirtualDirectory.InstallAuthModuleIsapiFilter(this.DataObject);
						goto IL_DE;
					}
					catch (Exception ex)
					{
						TaskLogger.Trace("Exception occurred in InstallIsapiFilter(): {0}", new object[]
						{
							ex.Message
						});
						this.WriteWarning(Strings.AuthModuleFilterMetabaseIsapiInstallFailure);
						throw;
					}
				}
				base.WriteError(new InvalidOperationException(Strings.ErrorMetabasePathNotFound(this.DataObject.MetabasePath)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			IL_DE:
			TaskLogger.LogExit();
		}

		protected override void AddCustomVDirProperties(ArrayList customProperties)
		{
			base.AddCustomVDirProperties(customProperties);
			customProperties.Add(new MetabaseProperty("DefaultDoc", "autodiscover.xml"));
			customProperties.Add(new MetabaseProperty("ScriptMaps", ".xml," + DotNetFrameworkInfo.AspNetIsapiDllPath + ",1,POST,GET"));
			string path = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess");
			customProperties.Add(new MetabaseProperty("HttpErrors", "401,1,FILE," + System.IO.Path.Combine(path, "help\\401-1.xml")));
		}

		protected override ListDictionary ChildVirtualDirectories
		{
			get
			{
				ListDictionary listDictionary = new ListDictionary();
				if (base.Role == VirtualDirectoryRole.Mailbox)
				{
					string[] array = new string[]
					{
						"bin",
						"help"
					};
					foreach (string text in array)
					{
						if (Directory.Exists(System.IO.Path.Combine(base.Path, text)))
						{
							listDictionary.Add(text, new ArrayList
							{
								new MetabaseProperty("AccessFlags", MetabasePropertyTypes.AccessFlags.NoAccess),
								new MetabaseProperty("AuthFlags", MetabasePropertyTypes.AuthFlags.NoneSet),
								new MetabaseProperty("AppPoolId", base.AppPoolId)
							});
						}
					}
				}
				return listDictionary;
			}
		}

		protected override void InternalValidate()
		{
			if (!base.Fields.IsModified("Path"))
			{
				base.Path = System.IO.Path.Combine(ConfigurationContext.Setup.InstallPath, this.VirtualDirectoryPath);
			}
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			base.InternalValidateBasicLiveIdBasic();
		}

		protected override void InternalProcessComplete()
		{
			base.InternalProcessComplete();
			ExchangeServiceVDirHelper.ForceAnonymous(this.DataObject.MetabasePath);
			ExchangeServiceVDirHelper.EwsAutodiscMWA.OnNewManageWCFEndpoints(this, ExchangeServiceVDirHelper.EwsAutodiscMWA.EndpointProtocol.Autodiscover, this.DataObject.BasicAuthentication, this.DataObject.WindowsAuthentication, this.DataObject.WSSecurityAuthentication ?? false, this.DataObject.OAuthAuthentication ?? false, this.DataObject, base.Role);
		}

		internal static void InstallAuthModuleIsapiFilter(ADAutodiscoverVirtualDirectory vdirObject)
		{
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(vdirObject.MetabasePath))
			{
				AuthModuleIsapiFilter.Install(directoryEntry);
			}
		}

		private const string AutodiscoverVDirName = "Autodiscover";

		private const string AutodiscoverVDirPath = "ClientAccess\\Autodiscover";

		private const string AutodiscoverCafeVDirPath = "FrontEnd\\HttpProxy\\Autodiscover";

		private const string AutodiscoverDefaultAppPoolId = "MSExchangeAutodiscoverAppPool";

		private const string BinFolderName = "bin";

		private const string HelpFolderName = "help";

		private const string DefaultDocName = "autodiscover.xml";
	}
}
