using System;
using System.Collections.Generic;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Parser
{
	internal class UICommandLineParser : CommandLineParser
	{
		public UICommandLineParser(ISetupLogger logger) : base(logger)
		{
			base.TokenMapping.Add("mode", "mode");
			base.TokenMapping.Add("m", "mode");
			base.ParserSchema.Add("mode", new ParameterSchemaEntry("mode", ParameterType.MustHaveValue, SetupOperations.AllUIInstallations, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseMode)));
			base.TokenMapping.Add("sourcedir", "sourcedir");
			base.TokenMapping.Add("s", "sourcedir");
			base.ParserSchema.Add("sourcedir", new ParameterSchemaEntry("sourcedir", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.Upgrade | SetupOperations.AddUmLanguagePack, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseSourceDir)));
			base.TokenMapping.Add("updatesdir", "updatesdir");
			base.TokenMapping.Add("u", "updatesdir");
			base.ParserSchema.Add("updatesdir", new ParameterSchemaEntry("updatesdir", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.RecoverServer | SetupOperations.Upgrade | SetupOperations.PrepareAD | SetupOperations.PrepareSchema | SetupOperations.PrepareDomain | SetupOperations.LanguagePack | SetupOperations.AddUmLanguagePack | SetupOperations.RemoveUmLanguagePack | SetupOperations.NewProvisionedServer | SetupOperations.PrepareSCT, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseSourceDir)));
			base.TokenMapping.Add("restart", "restart");
			base.TokenMapping.Add("sr", "restart");
			base.ParserSchema.Add("restart", new ParameterSchemaEntry("restart", ParameterType.CannotHaveValue, SetupOperations.Install | SetupOperations.RecoverServer | SetupOperations.Upgrade | SetupOperations.PrepareAD | SetupOperations.PrepareSchema | SetupOperations.PrepareDomain | SetupOperations.LanguagePack | SetupOperations.AddUmLanguagePack | SetupOperations.RemoveUmLanguagePack | SetupOperations.NewProvisionedServer | SetupOperations.PrepareSCT, SetupRoles.AllRoles));
			base.TokenMapping.Add("languagepack", "languagepack");
			base.TokenMapping.Add("lp", "languagepack");
			base.ParserSchema.Add("languagepack", new ParameterSchemaEntry("languagepack", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.RecoverServer | SetupOperations.Upgrade | SetupOperations.PrepareAD | SetupOperations.PrepareSchema | SetupOperations.PrepareDomain | SetupOperations.LanguagePack | SetupOperations.AddUmLanguagePack | SetupOperations.RemoveUmLanguagePack | SetupOperations.NewProvisionedServer | SetupOperations.PrepareSCT, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseSourceFile)));
			base.TokenMapping.Add("addumlanguagepack", "addumlanguagepack");
			base.ParserSchema.Add("addumlanguagepack", new ParameterSchemaEntry("addumlanguagepack", ParameterType.MustHaveValue, SetupOperations.AddUmLanguagePack, SetupRoles.None, new ParseMethod(CommandLineParser.ParseCultureList)));
			base.TokenMapping.Add("removeumlanguagepack", "removeumlanguagepack");
			base.ParserSchema.Add("removeumlanguagepack", new ParameterSchemaEntry("removeumlanguagepack", ParameterType.MustHaveValue, SetupOperations.RemoveUmLanguagePack, SetupRoles.None, new ParseMethod(CommandLineParser.ParseCultureList)));
		}

		public override SetupOperations CalculateOperationType(Dictionary<string, SetupParameter> parameters)
		{
			SetupOperations setupOperations = SetupOperations.None;
			if (parameters.ContainsKey("mode"))
			{
				SetupOperations setupOperations2 = (SetupOperations)parameters["mode"].Value;
				SetupOperations setupOperations3 = setupOperations2;
				switch (setupOperations3)
				{
				case SetupOperations.Install:
					setupOperations = SetupOperations.Install;
					break;
				case SetupOperations.Uninstall:
					setupOperations = SetupOperations.Uninstall;
					break;
				default:
					if (setupOperations3 != SetupOperations.Upgrade)
					{
						throw new ParseException(Strings.InvalidUIMode(setupOperations2.ToString()));
					}
					setupOperations = SetupOperations.Upgrade;
					break;
				}
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
			return setupOperations;
		}
	}
}
