using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Start", "PreConfiguration", SupportsShouldProcess = true)]
	public sealed class StartPreConfiguration : ManageSetupBindingTasks
	{
		public StartPreConfiguration()
		{
			base.ImplementsResume = false;
			base.Fields["InstallWindowsComponents"] = false;
			base.Fields["ADToolsNeeded"] = false;
			base.SetFileSearchPath(null);
		}

		private bool IsADServerRole(string role)
		{
			return Array.Exists<string>(StartPreConfiguration.ADServerRoles, (string item) => item == role);
		}

		private bool IsServerRole(string role)
		{
			return this.IsADServerRole(role) || role == StartPreConfiguration.GatewayRole;
		}

		private bool IsAdminToolsRole(string role)
		{
			return role == StartPreConfiguration.AdminToolsRole;
		}

		private bool IsLanguagePacks(string role)
		{
			return role == StartPreConfiguration.LanguagePacks;
		}

		private bool IsSupportedRole(string role)
		{
			return this.IsServerRole(role) || this.IsAdminToolsRole(role) || this.IsLanguagePacks(role);
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.StartPreConfigurationDescription;
			}
		}

		[Parameter(Mandatory = false)]
		public bool InstallWindowsComponents
		{
			get
			{
				return (bool)base.Fields["InstallWindowsComponents"];
			}
			set
			{
				base.Fields["InstallWindowsComponents"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ADToolsNeeded
		{
			get
			{
				return (bool)base.Fields["ADToolsNeeded"];
			}
			set
			{
				base.Fields["ADToolsNeeded"] = value;
			}
		}

		protected override void PopulateComponentInfoFileNames()
		{
			foreach (string role in this.Roles)
			{
				if (this.IsAdminToolsRole(role))
				{
					base.ComponentInfoFileNames.Add("setup\\data\\AdminToolsPreConfig.xml");
				}
			}
			bool flag = false;
			foreach (string role2 in this.Roles)
			{
				if (this.IsServerRole(role2))
				{
					flag = true;
					base.ComponentInfoFileNames.Add("setup\\data\\AllServerRolesPreConfig.xml");
					break;
				}
			}
			foreach (string role3 in this.Roles)
			{
				if (this.IsADServerRole(role3))
				{
					base.ComponentInfoFileNames.Add("setup\\data\\AllADRolesPreConfig.xml");
					break;
				}
			}
			foreach (string text in this.Roles)
			{
				if (this.IsServerRole(text))
				{
					base.ComponentInfoFileNames.Add(string.Format("setup\\data\\{0}PreConfig.xml", text.Replace("Role", "")));
				}
			}
			foreach (string role4 in this.Roles)
			{
				if (this.IsLanguagePacks(role4))
				{
					base.ComponentInfoFileNames.Add("setup\\data\\LanguagePacksPreConfig.xml");
					break;
				}
			}
			if (flag)
			{
				base.ComponentInfoFileNames.Add("setup\\data\\AllServerRolesPreConfigLast.xml");
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				base.InternalProcessRecord();
			}
			catch (NoComponentInfoFilesException ex)
			{
				base.WriteVerbose(ex.LocalizedString);
			}
			TaskLogger.LogExit();
		}

		private static readonly string[] ADServerRoles = new string[]
		{
			"MailboxRole",
			"UnifiedMessagingRole",
			"ClientAccessRole",
			"BridgeheadRole",
			"FrontendTransportRole",
			"OSPRole",
			"CafeRole"
		};

		private static readonly string GatewayRole = "GatewayRole";

		private static readonly string AdminToolsRole = "AdminToolsRole";

		private static readonly string LanguagePacks = "LanguagePacks";
	}
}
