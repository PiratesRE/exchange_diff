using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Setup.Parser
{
	internal abstract class SetupParser
	{
		public string SeparatorCharacters { get; set; }

		public Dictionary<string, string> TokenMapping
		{
			get
			{
				return this.tokenMapping;
			}
		}

		public Dictionary<string, ParameterSchemaEntry> ParserSchema
		{
			get
			{
				return this.parserSchema;
			}
		}

		public SetupParameter Parse(string parseString)
		{
			string text = null;
			int num = parseString.IndexOfAny(this.SeparatorCharacters.ToCharArray());
			string text2;
			bool flag;
			if (num < 0)
			{
				text2 = parseString.ToLowerInvariant();
				flag = false;
			}
			else
			{
				text2 = parseString.Substring(0, num).ToLowerInvariant();
				text2 = text2.TrimEnd(new char[0]);
				text = parseString.Substring(num + 1, parseString.Length - (num + 1));
				text = text.TrimStart(new char[0]);
				flag = true;
			}
			if (!this.TokenMapping.ContainsKey(text2))
			{
				throw new ParseException(Strings.UnknownParameter(text2));
			}
			text2 = this.TokenMapping[text2];
			ParameterSchemaEntry parameterSchemaEntry = this.ParserSchema[text2];
			if (!flag)
			{
				if (parameterSchemaEntry.ParameterType == ParameterType.MustHaveValue)
				{
					throw new ParseException(Strings.ParameterMustHaveValue(text2));
				}
			}
			else
			{
				if (parameterSchemaEntry.ParameterType == ParameterType.CannotHaveValue)
				{
					throw new ParseException(Strings.ParameterCannotHaveValue(text2));
				}
				if (string.IsNullOrEmpty(text))
				{
					throw new ParseException(Strings.EmptyValueSpecified(text2));
				}
			}
			return new SetupParameter(text2, parameterSchemaEntry.ParseMethod(text));
		}

		public Dictionary<string, SetupParameter> ParseAll(List<string> args)
		{
			Dictionary<string, SetupParameter> dictionary = new Dictionary<string, SetupParameter>();
			foreach (string parseString in args)
			{
				SetupParameter setupParameter = this.Parse(parseString);
				if (dictionary.ContainsKey(setupParameter.Name))
				{
					throw new ParseException(Strings.ParameterSpecifiedMultipleTimes(setupParameter.Name));
				}
				dictionary.Add(setupParameter.Name, setupParameter);
			}
			return dictionary;
		}

		public virtual void ValidateParameters(Dictionary<string, SetupParameter> parameters, SetupOperations currentOperation)
		{
			if (currentOperation == SetupOperations.None)
			{
				throw new ParseException(Strings.CurrentOperationNotSet);
			}
			foreach (SetupParameter setupParameter in parameters.Values)
			{
				ParameterSchemaEntry parameterSchemaEntry = this.ParserSchema[setupParameter.Name];
				if ((currentOperation & parameterSchemaEntry.ValidOperations) == SetupOperations.None)
				{
					throw new ParseException(Strings.ParameterNotValidForCurrentOperation(setupParameter.Name, currentOperation.ToString()));
				}
			}
		}

		private readonly Dictionary<string, string> tokenMapping = new Dictionary<string, string>();

		private readonly Dictionary<string, ParameterSchemaEntry> parserSchema = new Dictionary<string, ParameterSchemaEntry>();

		public static class Tokens
		{
			public const string Mode = "mode";

			public const string Roles = "roles";

			public const string PrepareAD = "preparead";

			public const string PrepareSCT = "preparesct";

			public const string PrepareSchema = "prepareschema";

			public const string PrepareDomain = "preparedomain";

			public const string PrepareAllDomains = "preparealldomains";

			public const string SourceDir = "sourcedir";

			public const string TargetDir = "targetdir";

			public const string AnswerFile = "answerfile";

			public const string DomainController = "domaincontroller";

			public const string AdamLdapPort = "adamldapport";

			public const string AdamSslPort = "adamsslport";

			public const string NewProvisionedServer = "newprovisionedserver";

			public const string RemoveProvisionedServer = "removeprovisionedserver";

			public const string EnableErrorReporting = "enableerrorreporting";

			public const string NoSelfSignedCertificates = "noselfsignedcertificates";

			public const string OrganizationName = "organizationname";

			public const string DoNotStartTransport = "donotstarttransport";

			public const string AddUmLanguagePack = "addumlanguagepack";

			public const string RemoveUmLanguagePack = "removeumlanguagepack";

			public const string UpdatesDir = "updatesdir";

			public const string LanguagePack = "languagepack";

			public const string CustomerFeedbackEnabled = "customerfeedbackenabled";

			public const string Industry = "industry";

			public const string ExternalCASServerDomain = "externalcasserverdomain";

			public const string Mdbname = "mdbname";

			public const string DbFilePath = "dbfilepath";

			public const string LogFolderPath = "logfolderpath";

			public const string ActiveDirectorySplitPermissions = "ActiveDirectorySplitPermissions";

			public const string InstallWindowsComponents = "installwindowscomponents";

			public const string DisableAMFiltering = "disableamfiltering";

			public const string Restart = "restart";

			public const string IAcceptExchangeServerLicenseTerms = "iacceptexchangeserverlicenseterms";

			public const string TenantOrganizationConfig = "tenantorganizationconfig";
		}
	}
}
