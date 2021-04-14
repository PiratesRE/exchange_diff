using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageSetupBindingTasks : ComponentInfoBasedTask
	{
		public ManageSetupBindingTasks()
		{
			base.Fields["InstallPath"] = ConfigurationContext.Setup.InstallPath;
			base.Fields["DatacenterPath"] = ConfigurationContext.Setup.DatacenterPath;
			base.Fields["SetupLoggingPath"] = ConfigurationContext.Setup.SetupLoggingPath;
			base.Fields["LoggingPath"] = ConfigurationContext.Setup.LoggingPath;
			base.Fields["BinPath"] = ConfigurationContext.Setup.BinPath;
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

		protected virtual string[] GetComponentFiles(string roleNameOrAllRoles)
		{
			if (roleNameOrAllRoles != null)
			{
				if (<PrivateImplementationDetails>{45A016D6-9512-4E33-B5E4-CBF8CD83FD38}.$$method0x6001050-1 == null)
				{
					<PrivateImplementationDetails>{45A016D6-9512-4E33-B5E4-CBF8CD83FD38}.$$method0x6001050-1 = new Dictionary<string, int>(15)
					{
						{
							"AllRoles",
							0
						},
						{
							"AdminToolsRole",
							1
						},
						{
							"BridgeheadRole",
							2
						},
						{
							"ClientAccessRole",
							3
						},
						{
							"GatewayRole",
							4
						},
						{
							"MailboxRole",
							5
						},
						{
							"UnifiedMessagingRole",
							6
						},
						{
							"FrontendTransportRole",
							7
						},
						{
							"CafeRole",
							8
						},
						{
							"MonitoringRole",
							9
						},
						{
							"CentralAdminRole",
							10
						},
						{
							"CentralAdminDatabaseRole",
							11
						},
						{
							"CentralAdminFrontEndRole",
							12
						},
						{
							"LanguagePacksRole",
							13
						},
						{
							"OSPRole",
							14
						}
					};
				}
				int num;
				if (<PrivateImplementationDetails>{45A016D6-9512-4E33-B5E4-CBF8CD83FD38}.$$method0x6001050-1.TryGetValue(roleNameOrAllRoles, out num))
				{
					string arg;
					switch (num)
					{
					case 0:
						arg = "AllRoles";
						break;
					case 1:
						arg = "AdminTools";
						break;
					case 2:
						arg = "Bridgehead";
						break;
					case 3:
						arg = "ClientAccess";
						break;
					case 4:
						arg = "Gateway";
						break;
					case 5:
						arg = "Mailbox";
						break;
					case 6:
						arg = "UnifiedMessaging";
						break;
					case 7:
						arg = "FrontendTransport";
						break;
					case 8:
						arg = "Cafe";
						break;
					case 9:
						arg = "MonitoringRole";
						break;
					case 10:
						arg = "CentralAdmin";
						break;
					case 11:
						arg = "CentralAdminDatabase";
						break;
					case 12:
						arg = "CentralAdminFrontEnd";
						break;
					case 13:
						arg = "LanguagePacks";
						break;
					case 14:
						arg = "OSP";
						break;
					default:
						goto IL_1B2;
					}
					object[] customAttributes = base.GetType().GetCustomAttributes(typeof(CmdletAttribute), false);
					string nounName = ((CmdletAttribute)customAttributes[0]).NounName;
					if (roleNameOrAllRoles != "AllRoles" && RoleManager.GetRoleByName(roleNameOrAllRoles).IsDatacenterOnly)
					{
						return new string[]
						{
							string.Format("setup\\data\\Datacenter{0}{1}Component.xml", arg, nounName)
						};
					}
					return new string[]
					{
						string.Format("setup\\data\\{0}{1}Component.xml", arg, nounName),
						string.Format("setup\\data\\Datacenter{0}{1}Component.xml", arg, nounName)
					};
				}
			}
			IL_1B2:
			throw new ArgumentException(string.Format("Unknown role name '{0}'", roleNameOrAllRoles), "roleNameOrAllRoles");
		}

		[Parameter(Mandatory = true)]
		public virtual InstallationModes Mode
		{
			get
			{
				return base.InstallationMode;
			}
			set
			{
				base.InstallationMode = value;
			}
		}

		[Parameter(Mandatory = true)]
		public virtual string[] Roles
		{
			get
			{
				string text = (string)base.Fields["Roles"];
				if (text == null)
				{
					return null;
				}
				return text.Split(new char[]
				{
					','
				});
			}
			set
			{
				base.Fields["Roles"] = string.Join(",", value);
			}
		}

		[Parameter(Mandatory = false)]
		public virtual Version PreviousVersion
		{
			get
			{
				return (Version)base.Fields["PreviousVersion"];
			}
			set
			{
				base.Fields["PreviousVersion"] = value;
			}
		}

		protected override void PopulateContextVariables()
		{
			base.Fields["TargetVersion"] = ConfigurationContext.Setup.GetExecutingVersion();
			base.PopulateContextVariables();
		}

		protected virtual void PopulateComponentInfoFileNames()
		{
			base.ComponentInfoFileNames.AddRange(this.GetComponentFiles("AllRoles"));
			foreach (string text in this.Roles)
			{
				if (RoleManager.GetRoleByName(text) == null)
				{
					base.WriteError(new LocalizedException(Strings.ErrorUnknownRole(text)), ErrorCategory.InvalidArgument, null);
				}
				else
				{
					base.ComponentInfoFileNames.AddRange(this.GetComponentFiles(text));
				}
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.ComponentInfoFileNames = new List<string>();
			this.PopulateComponentInfoFileNames();
			base.InternalValidate();
			TaskLogger.LogExit();
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
	}
}
