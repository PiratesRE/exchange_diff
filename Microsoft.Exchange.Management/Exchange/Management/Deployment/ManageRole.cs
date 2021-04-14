using System;
using System.IO;
using System.Management.Automation;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageRole : ComponentInfoBasedTask
	{
		protected ManageRole()
		{
			this.role = RoleManager.GetRoleByName(this.RoleName);
			base.Fields["LanguagePacksPath"] = this.GetMsiSourcePath();
			base.Fields["FqdnOrName"] = base.GetFqdnOrNetbiosName();
			base.Fields["InstallPath"] = ConfigurationContext.Setup.InstallPath;
			base.Fields["DatacenterPath"] = ConfigurationContext.Setup.DatacenterPath;
			base.Fields["SetupLoggingPath"] = ConfigurationContext.Setup.SetupLoggingPath;
			base.Fields["LoggingPath"] = ConfigurationContext.Setup.LoggingPath;
			base.Fields["BinPath"] = ConfigurationContext.Setup.BinPath;
			base.Fields["LoggedOnUser"] = this.GetLoggedOnUserName();
			base.Fields["IsFfo"] = new SwitchParameter(false);
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter IsFfo
		{
			get
			{
				return (SwitchParameter)base.Fields["IsFfo"];
			}
			set
			{
				base.Fields["IsFfo"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string LanguagePacksPath
		{
			get
			{
				return (string)base.Fields["LanguagePacksPath"];
			}
			set
			{
				base.Fields["LanguagePacksPath"] = value;
			}
		}

		protected string RoleName
		{
			get
			{
				if (base.Fields["RoleName"] == null)
				{
					Attribute[] customAttributes = Attribute.GetCustomAttributes(base.GetType());
					foreach (Attribute attribute in customAttributes)
					{
						if (attribute is CmdletAttribute)
						{
							CmdletAttribute cmdletAttribute = (CmdletAttribute)attribute;
							base.Fields["RoleName"] = cmdletAttribute.NounName;
							TaskLogger.Trace("Role name is \"{0}\"", new object[]
							{
								base.Fields["RoleName"]
							});
							break;
						}
					}
				}
				return (string)base.Fields["RoleName"];
			}
		}

		protected override InstallationModes InstallationMode
		{
			get
			{
				if ((InstallationModes)base.Fields["InstallationMode"] == InstallationModes.Unknown)
				{
					Attribute[] customAttributes = Attribute.GetCustomAttributes(base.GetType());
					foreach (Attribute attribute in customAttributes)
					{
						if (attribute is CmdletAttribute)
						{
							CmdletAttribute cmdletAttribute = (CmdletAttribute)attribute;
							base.Fields["InstallationMode"] = (InstallationModes)Enum.Parse(typeof(InstallationModes), cmdletAttribute.VerbName);
							TaskLogger.Trace("Attributed mode is \"{0}\"", new object[]
							{
								base.Fields["InstallationMode"]
							});
							break;
						}
					}
				}
				return (InstallationModes)base.Fields["InstallationMode"];
			}
		}

		protected override void CheckInstallationMode()
		{
			InstallationModes installationModes = (InstallationModes)base.Fields["InstallationMode"];
			if (installationModes == InstallationModes.Install && this.role.IsInstalled)
			{
				base.Fields["InstallationMode"] = InstallationModes.BuildToBuildUpgrade;
				TaskLogger.Trace("Updating mode to \"{0}\" because this role is installed", new object[]
				{
					base.Fields["InstallationMode"]
				});
			}
			if ((installationModes == InstallationModes.Install || installationModes == InstallationModes.BuildToBuildUpgrade) && this.role.IsMissingPostSetup)
			{
				base.Fields["InstallationMode"] = InstallationModes.PostSetupOnly;
				TaskLogger.Trace("Updating mode to \"{0}\" because this role is missing post setup", new object[]
				{
					base.Fields["InstallationMode"]
				});
			}
		}

		protected override void FilterComponents()
		{
			base.FilterComponents();
			base.ComponentInfoList.RemoveAll(delegate(SetupComponentInfo component)
			{
				if (!base.IsDatacenter)
				{
					return false;
				}
				if (this.IsFfo)
				{
					return component.DatacenterMode == DatacenterMode.ExO;
				}
				return component.DatacenterMode == DatacenterMode.Ffo;
			});
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.RoleName,
				this.InstallationMode
			});
			base.WriteVerbose(Strings.AttemptingToManageRole(this.InstallationMode.ToString(), this.RoleName));
			try
			{
				if (!this.role.IsUnpacked && this.InstallationMode != InstallationModes.Uninstall)
				{
					base.ThrowTerminatingError(new TaskException(Strings.MustBeUnpacked), ErrorCategory.InvalidOperation, this.role);
				}
				this.CheckInstallationMode();
				if (this.InstallationMode == InstallationModes.Install)
				{
					bool flag = false;
					StringBuilder stringBuilder = new StringBuilder();
					foreach (Role role in RoleManager.GetInstalledRoles())
					{
						if (!role.IsCurrent)
						{
							flag = true;
							stringBuilder.Append(role.RoleName);
							stringBuilder.Append(" ");
						}
					}
					if (flag)
					{
						base.ThrowTerminatingError(new TaskException(Strings.CannotInstallWithNonCurrentRoles(stringBuilder.ToString())), ErrorCategory.ObjectNotFound, this.role);
					}
				}
				base.InternalValidate();
			}
			catch (Exception ex)
			{
				base.WriteVerbose(Strings.RoleNotConfigured(ex.Message));
				throw;
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				base.ComponentInfoList = new SetupComponentInfoCollection();
				try
				{
					base.ComponentInfoList.AddRange(RoleManager.GetRequiredComponents(this.RoleName, this.InstallationMode));
				}
				catch (FileNotFoundException exception)
				{
					base.WriteError(exception, ErrorCategory.ObjectNotFound, null);
				}
				catch (XmlDeserializationException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidData, null);
				}
				if (this.InstallationMode != InstallationModes.Uninstall)
				{
					ProvisioningLayer.Disabled = false;
				}
				base.GenerateAndExecuteTaskScript(InstallationCircumstances.Standalone);
			}
			finally
			{
				TaskLogger.LogExit();
				RoleManager.Reset();
			}
		}

		protected override void PopulateContextVariables()
		{
			base.Fields["PreviousVersion"] = RolesUtility.GetConfiguredVersion(this.RoleName);
			base.Fields["TargetVersion"] = ConfigurationContext.Setup.GetExecutingVersion();
			base.Fields["NetBIOSName"] = base.GetNetBIOSName((string)base.Fields["FqdnOrName"]);
			this.PopulateRoles(RoleManager.Roles, "AllRoles");
			RoleCollection installedRoles = RoleManager.GetInstalledRoles();
			foreach (Role role in installedRoles)
			{
				base.Fields[string.Format("Is{0}Installed", role.RoleName)] = role.IsInstalled;
			}
			this.PopulateRoles(installedRoles, "Roles");
			base.PopulateContextVariables();
		}

		private string GetLoggedOnUserName()
		{
			WindowsPrincipal windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
			return windowsPrincipal.Identity.Name;
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

		private void PopulateRoles(RoleCollection roleCollection, string fieldName)
		{
			if (roleCollection.Count > 0)
			{
				string[] value = roleCollection.ConvertAll<string>((Role r) => r.RoleName).ToArray();
				base.Fields[fieldName] = string.Join(",", value);
			}
		}

		private Role role;
	}
}
