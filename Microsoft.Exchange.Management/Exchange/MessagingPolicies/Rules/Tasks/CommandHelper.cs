using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	internal static class CommandHelper
	{
		private static IEnumerable<CmdletConfigurationEntry> ExchCmdletConfigurationEntries
		{
			get
			{
				if (CommandHelper.cmdletConfigEntries == null)
				{
					Assembly assembly = Assembly.Load("Microsoft.Exchange.PowerShell.Configuration");
					Type type = assembly.GetType("Microsoft.Exchange.Management.PowerShell.CmdletConfigurationEntries");
					CommandHelper.cmdletConfigEntries = new List<CmdletConfigurationEntry>();
					CommandHelper.cmdletConfigEntries.AddRange(CommandHelper.GetExchCmdletConfigEntries(type, "ExchangeNonEdgeCmdletConfigurationEntries"));
					CommandHelper.cmdletConfigEntries.AddRange(CommandHelper.GetExchCmdletConfigEntries(type, "ExchangeCmdletConfigurationEntries"));
					CommandHelper.cmdletConfigEntries.AddRange(CommandHelper.GetExchCmdletConfigEntries(type, "ExchangeEdgeCmdletConfigurationEntries"));
					CommandHelper.cmdletConfigEntries.AddRange(CommandHelper.GetExchCmdletConfigEntries(type, "ExchangeNonGallatinCmdletConfigurationEntries"));
				}
				return CommandHelper.cmdletConfigEntries;
			}
		}

		public static string AddOrganizationScope(string command, string organization)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}
			if (string.IsNullOrEmpty(organization))
			{
				return command;
			}
			Type cmdletImplementingType = CommandHelper.GetCmdletImplementingType(command);
			if (cmdletImplementingType == null)
			{
				return command;
			}
			if (CommandHelper.IsOrganizationParameterSupported(cmdletImplementingType))
			{
				return CommandHelper.AddOrganizationParameter(command, organization);
			}
			if (CommandHelper.IsIdentityParameterSupported(cmdletImplementingType))
			{
				return CommandHelper.AddOrganizationToIdentity(command, organization);
			}
			return command;
		}

		private static Type GetCmdletImplementingType(string command)
		{
			Match match = Regex.Match(command, "^\\s*(\\w+-\\w+)\\s");
			if (!match.Success)
			{
				return null;
			}
			CmdletConfigurationEntry cmdletConfigurationEntry = CommandHelper.ExchCmdletConfigurationEntries.FirstOrDefault((CmdletConfigurationEntry entry) => string.Compare(match.Groups[1].Captures[0].Value, entry.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
			if (cmdletConfigurationEntry == null)
			{
				return null;
			}
			return cmdletConfigurationEntry.ImplementingType;
		}

		private static IEnumerable<CmdletConfigurationEntry> GetExchCmdletConfigEntries(Type type, string propName)
		{
			CmdletConfigurationEntry[] array = null;
			PropertyInfo property = type.GetProperty(propName, BindingFlags.Static | BindingFlags.Public);
			if (property != null)
			{
				array = (CmdletConfigurationEntry[])property.GetValue(null, null);
			}
			return array ?? new CmdletConfigurationEntry[0];
		}

		private static bool IsOrganizationParameterSupported(Type implementingType)
		{
			return CommandHelper.organizationTypes.Any((Type baseType) => CommandHelper.IsTypeDerivedFrom(implementingType, baseType));
		}

		private static bool IsIdentityParameterSupported(Type implementingType)
		{
			return CommandHelper.identityTypes.Any((Type baseType) => CommandHelper.IsTypeDerivedFrom(implementingType, baseType));
		}

		private static string AddOrganizationParameter(string command, string organization)
		{
			string text = string.Format("-{0}", "Organization");
			if (CmdletValidator.IsParameterPresent(command, text))
			{
				throw new InvalidOperationException("OrganizationParameter specified");
			}
			return string.Format("{0} {1} \"{2}\"", command, text, organization);
		}

		private static string AddOrganizationToIdentity(string command, string organization)
		{
			Match match = Regex.Match(command, "\\s-(?<param>\\w+)(:\\s*|\\s+)(?<value>\".*?\"(\\s*,\\s*\".*?\"){0,}|'.*?'(\\s*,\\s*'.*?'){0,}|\\@\\{.*?\\}(,\\s*\\@\\{.*?\\}){0,}|[\\w\\d\\$][\\w\\d-]+)");
			if (CommandHelper.IsParameterSpecified(match, "Identity"))
			{
				CaptureCollection captures = match.Groups["param"].Captures;
				for (int i = 0; i < captures.Count; i++)
				{
					if (string.Compare(captures[i].Value, "Identity", StringComparison.InvariantCultureIgnoreCase) == 0)
					{
						return CommandHelper.ReplaceIdentityValue(match.Groups["value"].Captures[i], command, organization);
					}
				}
			}
			match = Regex.Match(command, "^\\s*\\w+-\\w+\\s+((?<id1>(?<quote>\"|').*?\\k<quote>)|(?<id2>[\\w\\d][\\w\\d\\.-]+))");
			if (match.Success)
			{
				return CommandHelper.ReplaceIdentityValue(match.Groups["id1"].Success ? match.Groups["id1"].Captures[0] : match.Groups["id2"].Captures[0], command, organization);
			}
			return command;
		}

		private static bool IsParameterSpecified(Match match, string paramName)
		{
			if (match.Success && match.Groups["param"].Success && match.Groups["param"].Captures.Count > 0)
			{
				return match.Groups["param"].Captures.Cast<Capture>().Any((Capture capture) => string.Compare(capture.Value, paramName, StringComparison.InvariantCultureIgnoreCase) == 0);
			}
			return false;
		}

		private static bool IsTypeDerivedFrom(Type type, Type baseType)
		{
			if (type != null && baseType != null)
			{
				if (CommandHelper.GetWeakerTypeName(type) == CommandHelper.GetWeakerTypeName(baseType))
				{
					return true;
				}
				if (type.BaseType != null)
				{
					return CommandHelper.IsTypeDerivedFrom(type.BaseType, baseType);
				}
			}
			return false;
		}

		private static string GetWeakerTypeName(Type type)
		{
			Match match = Regex.Match(type.FullName, "^(?<tname>[\\w\\d\\.]+(`\\d)?)(\\[\\[.*?\\]\\])?$");
			if (!match.Success)
			{
				return type.FullName;
			}
			return match.Groups["tname"].Captures[0].Value;
		}

		private static string ReplaceIdentityValue(Capture capture, string command, string organization)
		{
			string value = capture.Value;
			string text = CommandHelper.BuildStrongIdentity(value, organization);
			if (value != text)
			{
				StringBuilder stringBuilder = new StringBuilder(command.Substring(0, capture.Index));
				stringBuilder.Append(text);
				stringBuilder.Append(command.Substring(capture.Index + capture.Length));
				return stringBuilder.ToString();
			}
			return command;
		}

		private static string BuildStrongIdentity(string identity, string organization)
		{
			Match match = Regex.Match(identity, "^\\s*(?<quote>\"|')(?<id>.*?)(?<right>\\k<quote>\\s*)$");
			if (match.Success)
			{
				return string.Format("{0}{1}\\{2}{3}", new object[]
				{
					match.Groups["quote"].Captures[0].Value,
					organization,
					match.Groups["id"].Captures[0].Value,
					match.Groups["right"].Captures[0].Value
				});
			}
			return Regex.Replace(identity, "^(\\S+)$", string.Format("\"{0}\\$1\"", organization));
		}

		private const string regexCmdlet = "^\\s*(\\w+-\\w+)\\s";

		private const string regexTypeName = "^(?<tname>[\\w\\d\\.]+(`\\d)?)(\\[\\[.*?\\]\\])?$";

		private const string regexQuotedValue = "^\\s*(?<quote>\"|')(?<id>.*?)(?<right>\\k<quote>\\s*)$";

		private const string regexIdentityPos = "^\\s*\\w+-\\w+\\s+((?<id1>(?<quote>\"|').*?\\k<quote>)|(?<id2>[\\w\\d][\\w\\d\\.-]+))";

		private const string regexParams = "\\s-(?<param>\\w+)(:\\s*|\\s+)(?<value>\".*?\"(\\s*,\\s*\".*?\"){0,}|'.*?'(\\s*,\\s*'.*?'){0,}|\\@\\{.*?\\}(,\\s*\\@\\{.*?\\}){0,}|[\\w\\d\\$][\\w\\d-]+)";

		private static List<CmdletConfigurationEntry> cmdletConfigEntries;

		private static readonly Type[] organizationTypes = new Type[]
		{
			typeof(NewRecipientObjectTaskBase<>),
			typeof(NewMultitenancySystemConfigurationObjectTask<>),
			typeof(NewMultitenancyFixedNameSystemConfigurationObjectTask<>),
			typeof(GetMultitenancySystemConfigurationObjectTask<, >)
		};

		private static readonly Type[] identityTypes = new Type[]
		{
			typeof(RemoveTaskBase<, >),
			typeof(SetObjectWithIdentityTaskBase<, , >)
		};
	}
}
