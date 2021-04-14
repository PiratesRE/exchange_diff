using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.Setup.Parser
{
	internal class AnswerFileParser : SetupParser
	{
		public AnswerFileParser()
		{
			base.SeparatorCharacters = "=";
			base.TokenMapping.Add("adamldapport", "adamldapport");
			base.ParserSchema.Add("adamldapport", new ParameterSchemaEntry("adamldapport", ParameterType.MustHaveValue, SetupOperations.Install, SetupRoles.Gateway, new ParseMethod(CommandLineParser.ParseUInt16)));
			base.TokenMapping.Add("adamsslport", "adamsslport");
			base.ParserSchema.Add("adamsslport", new ParameterSchemaEntry("adamsslport", ParameterType.MustHaveValue, SetupOperations.Install, SetupRoles.Gateway, new ParseMethod(CommandLineParser.ParseUInt16)));
			base.TokenMapping.Add("enableerrorreporting", "enableerrorreporting");
			base.ParserSchema.Add("enableerrorreporting", new ParameterSchemaEntry("enableerrorreporting", ParameterType.CannotHaveValue, SetupOperations.AllMSIInstallOperations));
			base.TokenMapping.Add("organizationname", "organizationname");
			base.TokenMapping.Add("on", "organizationname");
			base.ParserSchema.Add("organizationname", new ParameterSchemaEntry("organizationname", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.PrepareAD, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseOrganizationName)));
			base.TokenMapping.Add("donotstarttransport", "donotstarttransport");
			base.ParserSchema.Add("donotstarttransport", new ParameterSchemaEntry("donotstarttransport", ParameterType.CannotHaveValue, SetupOperations.Install | SetupOperations.RecoverServer, SetupRoles.Bridgehead | SetupRoles.Gateway | SetupRoles.FrontendTransport));
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
			base.TokenMapping.Add("disableamfiltering", "disableamfiltering");
			base.ParserSchema.Add("disableamfiltering", new ParameterSchemaEntry("disableamfiltering", ParameterType.CannotHaveValue, SetupOperations.Install, SetupRoles.Bridgehead));
			base.TokenMapping.Add("iacceptexchangeserverlicenseterms", "iacceptexchangeserverlicenseterms");
			base.ParserSchema.Add("iacceptexchangeserverlicenseterms", new ParameterSchemaEntry("iacceptexchangeserverlicenseterms", ParameterType.CannotHaveValue, (SetupOperations)(-1), SetupRoles.AllRoles));
			base.TokenMapping.Add("tenantorganizationconfig", "tenantorganizationconfig");
			base.ParserSchema.Add("tenantorganizationconfig", new ParameterSchemaEntry("tenantorganizationconfig", ParameterType.MustHaveValue, SetupOperations.Install | SetupOperations.PrepareAD, SetupRoles.AllRoles, new ParseMethod(CommandLineParser.ParseSourceFile)));
		}

		public Dictionary<string, SetupParameter> ParseFile(string fileName)
		{
			Dictionary<string, SetupParameter> result;
			try
			{
				List<string> list = new List<string>();
				using (StreamReader streamReader = new StreamReader(fileName))
				{
					while (streamReader.Peek() >= 0)
					{
						string text = streamReader.ReadLine();
						if (!string.IsNullOrEmpty(text))
						{
							text = text.Trim();
							if (!text.StartsWith("[") && !text.StartsWith("#"))
							{
								list.Add(text);
							}
						}
					}
					result = base.ParseAll(list);
				}
			}
			catch (IOException innerException)
			{
				throw new ParseException(Strings.AnswerFileCouldNotBeOpened(fileName), innerException);
			}
			return result;
		}
	}
}
