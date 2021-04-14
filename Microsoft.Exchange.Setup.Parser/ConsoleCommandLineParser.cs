using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Parser
{
	internal class ConsoleCommandLineParser : CommandLineParser
	{
		public ConsoleCommandLineParser(ISetupLogger logger) : base(logger)
		{
			base.TokenMapping.Add("mode", "mode");
			base.TokenMapping.Add("m", "mode");
			base.ParserSchema.Add("mode", new ParameterSchemaEntry("mode", ParameterType.MustHaveValue, SetupOperations.AllModeOperations, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseMode)));
			base.TokenMapping.Add("roles", "roles");
			base.TokenMapping.Add("role", "roles");
			base.TokenMapping.Add("r", "roles");
			base.ParserSchema.Add("roles", new ParameterSchemaEntry("roles", ParameterType.MustHaveValue, SetupOperations.Install, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseRoles)));
			base.TokenMapping.Add("preparead", "preparead");
			base.TokenMapping.Add("p", "preparead");
			base.ParserSchema.Add("preparead", new ParameterSchemaEntry("preparead", ParameterType.CannotHaveValue, SetupOperations.PrepareAD, SetupRoles.None));
			base.TokenMapping.Add("preparesct", "preparesct");
			base.TokenMapping.Add("sct", "preparesct");
			base.ParserSchema.Add("preparesct", new ParameterSchemaEntry("preparesct", ParameterType.CannotHaveValue, SetupOperations.PrepareSCT, SetupRoles.None));
			base.TokenMapping.Add("prepareschema", "prepareschema");
			base.TokenMapping.Add("ps", "prepareschema");
			base.ParserSchema.Add("prepareschema", new ParameterSchemaEntry("prepareschema", ParameterType.CannotHaveValue, SetupOperations.PrepareSchema, SetupRoles.None));
			base.TokenMapping.Add("preparedomain", "preparedomain");
			base.TokenMapping.Add("pd", "preparedomain");
			base.ParserSchema.Add("preparedomain", new ParameterSchemaEntry("preparedomain", ParameterType.MayHaveValue, SetupOperations.PrepareDomain, SetupRoles.None));
			base.TokenMapping.Add("preparealldomains", "preparealldomains");
			base.TokenMapping.Add("pad", "preparealldomains");
			base.ParserSchema.Add("preparealldomains", new ParameterSchemaEntry("preparealldomains", ParameterType.CannotHaveValue, SetupOperations.PrepareDomain, SetupRoles.None));
			base.TokenMapping.Add("sourcedir", "sourcedir");
			base.TokenMapping.Add("s", "sourcedir");
			base.ParserSchema.Add("sourcedir", new ParameterSchemaEntry("sourcedir", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.RecoverServer | SetupOperations.Upgrade | SetupOperations.PrepareAD | SetupOperations.PrepareSchema | SetupOperations.PrepareDomain | SetupOperations.LanguagePack | SetupOperations.AddUmLanguagePack | SetupOperations.RemoveUmLanguagePack | SetupOperations.NewProvisionedServer | SetupOperations.PrepareSCT, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseSourceDir)));
			base.TokenMapping.Add("targetdir", "targetdir");
			base.TokenMapping.Add("t", "targetdir");
			base.ParserSchema.Add("targetdir", new ParameterSchemaEntry("targetdir", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.RecoverServer, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseNonRootLocalLongFullPath)));
			base.TokenMapping.Add("answerfile", "answerfile");
			base.TokenMapping.Add("af", "answerfile");
			base.ParserSchema.Add("answerfile", new ParameterSchemaEntry("answerfile", ParameterType.MustHaveValue, SetupOperations.AllSetupOperations));
			base.TokenMapping.Add("domaincontroller", "domaincontroller");
			base.TokenMapping.Add("dc", "domaincontroller");
			base.ParserSchema.Add("domaincontroller", new ParameterSchemaEntry("domaincontroller", ParameterType.MustHaveValue, SetupOperations.AllSetupOperations, SetupRoles.Mailbox | SetupRoles.Bridgehead | SetupRoles.ClientAccess | SetupRoles.UnifiedMessaging | SetupRoles.AdminTools | SetupRoles.Monitoring | SetupRoles.CentralAdmin | SetupRoles.CentralAdminDatabase | SetupRoles.Cafe | SetupRoles.FrontendTransport | SetupRoles.OSP | SetupRoles.CentralAdminFrontEnd));
			base.TokenMapping.Add("adamldapport", "adamldapport");
			base.ParserSchema.Add("adamldapport", new ParameterSchemaEntry("adamldapport", ParameterType.MustHaveValue, SetupOperations.Install, SetupRoles.Gateway, new ParseMethod(CommandLineParser.ParseUInt16)));
			base.TokenMapping.Add("adamsslport", "adamsslport");
			base.ParserSchema.Add("adamsslport", new ParameterSchemaEntry("adamsslport", ParameterType.MustHaveValue, SetupOperations.Install, SetupRoles.Gateway, new ParseMethod(CommandLineParser.ParseUInt16)));
			base.TokenMapping.Add("newprovisionedserver", "newprovisionedserver");
			base.TokenMapping.Add("nprs", "newprovisionedserver");
			base.ParserSchema.Add("newprovisionedserver", new ParameterSchemaEntry("newprovisionedserver", ParameterType.MayHaveValue, SetupOperations.NewProvisionedServer));
			base.TokenMapping.Add("removeprovisionedserver", "removeprovisionedserver");
			base.TokenMapping.Add("rprs", "removeprovisionedserver");
			base.ParserSchema.Add("removeprovisionedserver", new ParameterSchemaEntry("removeprovisionedserver", ParameterType.MayHaveValue, SetupOperations.AddUmLanguagePack));
			base.TokenMapping.Add("enableerrorreporting", "enableerrorreporting");
			base.ParserSchema.Add("enableerrorreporting", new ParameterSchemaEntry("enableerrorreporting", ParameterType.CannotHaveValue, SetupOperations.AllMSIInstallOperations));
			base.TokenMapping.Add("organizationname", "organizationname");
			base.TokenMapping.Add("on", "organizationname");
			base.ParserSchema.Add("organizationname", new ParameterSchemaEntry("organizationname", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.PrepareAD, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseOrganizationName)));
			base.TokenMapping.Add("donotstarttransport", "donotstarttransport");
			base.ParserSchema.Add("donotstarttransport", new ParameterSchemaEntry("donotstarttransport", ParameterType.CannotHaveValue, SetupOperations.Install | SetupOperations.RecoverServer, SetupRoles.Bridgehead | SetupRoles.Gateway | SetupRoles.FrontendTransport));
			base.TokenMapping.Add("languagepack", "languagepack");
			base.TokenMapping.Add("lp", "languagepack");
			base.ParserSchema.Add("languagepack", new ParameterSchemaEntry("languagepack", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.RecoverServer | SetupOperations.Upgrade | SetupOperations.PrepareAD | SetupOperations.PrepareSchema | SetupOperations.PrepareDomain | SetupOperations.LanguagePack | SetupOperations.AddUmLanguagePack | SetupOperations.RemoveUmLanguagePack | SetupOperations.NewProvisionedServer | SetupOperations.PrepareSCT, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseSourceFile)));
			base.TokenMapping.Add("addumlanguagepack", "addumlanguagepack");
			base.ParserSchema.Add("addumlanguagepack", new ParameterSchemaEntry("addumlanguagepack", ParameterType.MustHaveValue, SetupOperations.AddUmLanguagePack, SetupRoles.None, new ParseMethod(CommandLineParser.ParseCultureList)));
			base.TokenMapping.Add("removeumlanguagepack", "removeumlanguagepack");
			base.ParserSchema.Add("removeumlanguagepack", new ParameterSchemaEntry("removeumlanguagepack", ParameterType.MustHaveValue, SetupOperations.RemoveUmLanguagePack, SetupRoles.None, new ParseMethod(CommandLineParser.ParseCultureList)));
			base.TokenMapping.Add("updatesdir", "updatesdir");
			base.TokenMapping.Add("u", "updatesdir");
			base.ParserSchema.Add("updatesdir", new ParameterSchemaEntry("updatesdir", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.RecoverServer | SetupOperations.Upgrade | SetupOperations.PrepareAD | SetupOperations.PrepareSchema | SetupOperations.PrepareDomain | SetupOperations.LanguagePack | SetupOperations.AddUmLanguagePack | SetupOperations.RemoveUmLanguagePack | SetupOperations.NewProvisionedServer | SetupOperations.PrepareSCT, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseSourceDir)));
			base.TokenMapping.Add("customerfeedbackenabled", "customerfeedbackenabled");
			base.ParserSchema.Add("customerfeedbackenabled", new ParameterSchemaEntry("customerfeedbackenabled", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.PrepareAD, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseBool)));
			base.TokenMapping.Add("mdbname", "mdbname");
			base.ParserSchema.Add("mdbname", new ParameterSchemaEntry("mdbname", ParameterType.MustHaveValue, SetupOperations.Install, SetupRoles.Mailbox));
			base.TokenMapping.Add("dbfilepath", "dbfilepath");
			base.ParserSchema.Add("dbfilepath", new ParameterSchemaEntry("dbfilepath", ParameterType.MustHaveValue, SetupOperations.Install, SetupRoles.Mailbox, new ParseMethod(CommandLineParser.ParseDbFilePath)));
			base.TokenMapping.Add("logfolderpath", "logfolderpath");
			base.ParserSchema.Add("logfolderpath", new ParameterSchemaEntry("logfolderpath", ParameterType.MustHaveValue, SetupOperations.Install, SetupRoles.Mailbox, new ParseMethod(CommandLineParser.ParseNonRootLocalLongFullPath)));
			base.TokenMapping.Add("activedirectorysplitpermissions", "ActiveDirectorySplitPermissions");
			base.ParserSchema.Add("ActiveDirectorySplitPermissions", new ParameterSchemaEntry("ActiveDirectorySplitPermissions", ParameterType.MustHaveValue, SetupOperations.PrepareAD, SetupRoles.None, new ParseMethod(CommandLineParser.ParseBool)));
			base.TokenMapping.Add("installwindowscomponents", "installwindowscomponents");
			base.ParserSchema.Add("installwindowscomponents", new ParameterSchemaEntry("installwindowscomponents", ParameterType.CannotHaveValue, SetupOperations.AllMSIInstallOperations, SetupRoles.AllRoles));
			base.TokenMapping.Add("disableamfiltering", "disableamfiltering");
			base.ParserSchema.Add("disableamfiltering", new ParameterSchemaEntry("disableamfiltering", ParameterType.CannotHaveValue, SetupOperations.Install | SetupOperations.RecoverServer, SetupRoles.Bridgehead));
			base.TokenMapping.Add("restart", "restart");
			base.TokenMapping.Add("rs", "restart");
			base.ParserSchema.Add("restart", new ParameterSchemaEntry("restart", ParameterType.CannotHaveValue, SetupOperations.Install | SetupOperations.RecoverServer | SetupOperations.Upgrade | SetupOperations.PrepareAD | SetupOperations.PrepareSchema | SetupOperations.PrepareDomain | SetupOperations.LanguagePack | SetupOperations.AddUmLanguagePack | SetupOperations.RemoveUmLanguagePack | SetupOperations.NewProvisionedServer | SetupOperations.PrepareSCT, SetupRoles.AllRoles));
			base.TokenMapping.Add("iacceptexchangeserverlicenseterms", "iacceptexchangeserverlicenseterms");
			base.ParserSchema.Add("iacceptexchangeserverlicenseterms", new ParameterSchemaEntry("iacceptexchangeserverlicenseterms", ParameterType.CannotHaveValue, (SetupOperations)(-1), SetupRoles.AllRoles));
			base.TokenMapping.Add("tenantorganizationconfig", "tenantorganizationconfig");
			base.ParserSchema.Add("tenantorganizationconfig", new ParameterSchemaEntry("tenantorganizationconfig", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.PrepareAD, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseSourceFile)));
		}

		public override SetupOperations CalculateOperationType(Dictionary<string, SetupParameter> parameters)
		{
			SetupOperations setupOperations = SetupOperations.None;
			if (parameters == null)
			{
				parameters = new Dictionary<string, SetupParameter>();
			}
			if (parameters.ContainsKey("mode"))
			{
				setupOperations = (SetupOperations)parameters["mode"].Value;
				if (setupOperations == SetupOperations.Install && !parameters.ContainsKey("roles") && !parameters.ContainsKey("languagepack"))
				{
					throw new ParseException(Strings.InstallRequiresRoles);
				}
			}
			if (parameters.ContainsKey("preparead"))
			{
				if (setupOperations != SetupOperations.None)
				{
					throw new ParseException(Strings.PrepareFlagConstraint("preparead"));
				}
				setupOperations = SetupOperations.PrepareAD;
			}
			if (parameters.ContainsKey("preparesct"))
			{
				setupOperations |= SetupOperations.PrepareSCT;
			}
			if (parameters.ContainsKey("prepareschema"))
			{
				if (setupOperations != SetupOperations.None)
				{
					throw new ParseException(Strings.PrepareFlagConstraint("prepareschema"));
				}
				setupOperations = SetupOperations.PrepareSchema;
			}
			if (parameters.ContainsKey("preparedomain") || parameters.ContainsKey("preparealldomains"))
			{
				if (setupOperations != SetupOperations.None)
				{
					throw new ParseException(Strings.PrepareFlagConstraint("preparedomain"));
				}
				setupOperations = SetupOperations.PrepareDomain;
			}
			if (parameters.ContainsKey("newprovisionedserver"))
			{
				if (setupOperations != SetupOperations.None)
				{
					throw new ParseException(Strings.PrepareFlagConstraint("newprovisionedserver"));
				}
				setupOperations = SetupOperations.NewProvisionedServer;
			}
			if (parameters.ContainsKey("removeprovisionedserver"))
			{
				if (setupOperations != SetupOperations.None)
				{
					throw new ParseException(Strings.PrepareFlagConstraint("removeprovisionedserver"));
				}
				setupOperations = SetupOperations.AddUmLanguagePack;
			}
			if (parameters.ContainsKey("addumlanguagepack"))
			{
				if (setupOperations != SetupOperations.None)
				{
					throw new ParseException(Strings.PrepareFlagConstraint("addumlanguagepack"));
				}
				setupOperations = SetupOperations.AddUmLanguagePack;
			}
			if (parameters.ContainsKey("removeumlanguagepack"))
			{
				if (setupOperations != SetupOperations.None)
				{
					throw new ParseException(Strings.PrepareFlagConstraint("removeumlanguagepack"));
				}
				setupOperations = SetupOperations.RemoveUmLanguagePack;
			}
			if (setupOperations == SetupOperations.None)
			{
				if (!parameters.ContainsKey("mode") && parameters.ContainsKey("roles"))
				{
					parameters.Add("mode", base.Parse("mode:install"));
				}
				else if (!parameters.ContainsKey("mode") && parameters.ContainsKey("languagepack"))
				{
					setupOperations = SetupOperations.LanguagePack;
				}
			}
			if ((setupOperations & SetupOperations.AllModeOperations) != SetupOperations.None)
			{
				this.CheckRoleDependency(parameters);
			}
			return setupOperations;
		}

		public override void ValidateParameters(Dictionary<string, SetupParameter> parameters, SetupOperations currentOperation)
		{
			base.ValidateParameters(parameters, currentOperation);
			if (currentOperation == SetupOperations.Install && parameters.ContainsKey("roles"))
			{
				RoleCollection source = (RoleCollection)parameters["roles"].Value;
				if (CommandLineParser.IsClientAccessRole)
				{
					IEnumerable<Role> source2 = from r in source
					where (r.RoleName.Equals("CafeRole") && r.IsInstalled && !r.IsPartiallyInstalled) || (r.RoleName.Equals("FrontendTransportRole") && r.IsInstalled && !r.IsPartiallyInstalled)
					select r;
					if (source2.Count<Role>() == CommandLineParser.TotalClientAccessRoles)
					{
						throw new ParseException(Strings.CASRoleIsInstalled);
					}
				}
				if (CommandLineParser.IsMailboxRole)
				{
					IEnumerable<Role> source3 = from r in source
					where (r.RoleName.Equals("ClientAccessRole") && r.IsInstalled && !r.IsPartiallyInstalled) || (r.RoleName.Equals("MailboxRole") && r.IsInstalled && !r.IsPartiallyInstalled) || (r.RoleName.Equals("UnifiedMessagingRole") && r.IsInstalled && !r.IsPartiallyInstalled) || (r.RoleName.Equals("BridgeheadRole") && r.IsInstalled && !r.IsPartiallyInstalled)
					select r;
					if (source3.Count<Role>() == CommandLineParser.TotalMailboxRoles)
					{
						throw new ParseException(Strings.MBXRoleIsInstalled);
					}
				}
			}
		}

		private void CheckRoleDependency(Dictionary<string, SetupParameter> parameters)
		{
			SetupRoles setupRoles = SetupRoles.None;
			if (parameters.ContainsKey("roles"))
			{
				RoleCollection roleCollection = parameters["roles"].Value as RoleCollection;
				if (roleCollection != null)
				{
					foreach (Role role in roleCollection)
					{
						string roleName;
						switch (roleName = role.RoleName)
						{
						case "MailboxRole":
							setupRoles |= SetupRoles.Mailbox;
							continue;
						case "ClientAccessRole":
							setupRoles |= SetupRoles.ClientAccess;
							continue;
						case "GatewayRole":
							setupRoles |= SetupRoles.Gateway;
							continue;
						case "BridgeheadRole":
							setupRoles |= SetupRoles.Bridgehead;
							continue;
						case "UnifiedMessagingRole":
							setupRoles |= SetupRoles.UnifiedMessaging;
							continue;
						case "AdminToolsRole":
							setupRoles |= SetupRoles.AdminTools;
							continue;
						case "MonitoringRole":
							setupRoles |= SetupRoles.Monitoring;
							continue;
						case "CentralAdminRole":
							setupRoles |= SetupRoles.CentralAdmin;
							continue;
						case "CentralAdminDatabaseRole":
							setupRoles |= SetupRoles.CentralAdminDatabase;
							continue;
						case "CentralAdminFrontEndRole":
							setupRoles |= SetupRoles.CentralAdminFrontEnd;
							continue;
						case "FrontendTransportRole":
							setupRoles |= SetupRoles.FrontendTransport;
							continue;
						case "CafeRole":
							setupRoles |= SetupRoles.Cafe;
							continue;
						case "OSPRole":
							setupRoles |= SetupRoles.OSP;
							continue;
						}
						throw new ParseException(Strings.InvalidRole(role.RoleName));
					}
				}
			}
			foreach (SetupParameter setupParameter in parameters.Values)
			{
				ParameterSchemaEntry parameterSchemaEntry = base.ParserSchema[setupParameter.Name];
				if (parameterSchemaEntry.ValidRoles != SetupRoles.None && parameterSchemaEntry.ValidRoles != SetupRoles.AllRoles && setupRoles != SetupRoles.None && (parameterSchemaEntry.ValidRoles & setupRoles) == SetupRoles.None)
				{
					throw new ParseException(Strings.ParameterNotValidForCurrentRoles(setupParameter.Name));
				}
			}
		}
	}
}
