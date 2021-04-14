using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CommandLineParser : SetupParser
	{
		public CommandLineParser(ISetupLogger logger)
		{
			base.SeparatorCharacters = ":";
			this.setupLogger = logger;
		}

		public static bool IsClientAccessRole { get; private set; }

		public static int TotalClientAccessRoles { get; private set; }

		public static bool IsMailboxRole { get; private set; }

		public static int TotalMailboxRoles { get; private set; }

		public static object ParseMode(string givenMode)
		{
			string a;
			if ((a = givenMode.ToLowerInvariant()) != null)
			{
				SetupOperations setupOperations;
				if (!(a == "install"))
				{
					if (!(a == "uninstall"))
					{
						if (!(a == "recoverserver"))
						{
							if (!(a == "upgrade"))
							{
								goto IL_50;
							}
							setupOperations = SetupOperations.Upgrade;
						}
						else
						{
							setupOperations = SetupOperations.RecoverServer;
						}
					}
					else
					{
						setupOperations = SetupOperations.Uninstall;
					}
				}
				else
				{
					setupOperations = SetupOperations.Install;
				}
				return setupOperations;
			}
			IL_50:
			throw new ParseException(Strings.InvalidMode(givenMode));
		}

		public static object ParseRoles(string givenRoles)
		{
			CommandLineParser.IsClientAccessRole = false;
			CommandLineParser.IsMailboxRole = false;
			CommandLineParser.TotalClientAccessRoles = 0;
			CommandLineParser.TotalMailboxRoles = 0;
			string text = givenRoles.ToLowerInvariant();
			RoleCollection roleCollection = new RoleCollection();
			SetupRoles setupRoles = SetupRoles.None;
			string[] array = text.Split(",".ToCharArray());
			foreach (string text2 in array)
			{
				string text3 = text2.Trim();
				if (!string.IsNullOrEmpty(text3))
				{
					string key;
					switch (key = text3)
					{
					case "h":
					case "ht":
					case "hubtransport":
						if ((setupRoles & SetupRoles.Bridgehead) != SetupRoles.None)
						{
							throw new ParseException(Strings.RoleSpecifiedMultipleTimes("HubTransport"));
						}
						setupRoles |= SetupRoles.Bridgehead;
						roleCollection.Add(new BridgeheadRole());
						goto IL_490;
					case "e":
					case "et":
					case "edgetransport":
						if ((setupRoles & SetupRoles.Gateway) != SetupRoles.None)
						{
							throw new ParseException(Strings.RoleSpecifiedMultipleTimes("EdgeTransport"));
						}
						setupRoles |= SetupRoles.Gateway;
						roleCollection.Add(new GatewayRole());
						goto IL_490;
					case "t":
					case "mt":
					case "managementtools":
						if ((setupRoles & SetupRoles.AdminTools) != SetupRoles.None)
						{
							throw new ParseException(Strings.RoleSpecifiedMultipleTimes("ManagementTools"));
						}
						setupRoles |= SetupRoles.AdminTools;
						roleCollection.Add(new AdminToolsRole());
						goto IL_490;
					case "monitoring":
					case "mn":
					case "n":
						if ((setupRoles & SetupRoles.Monitoring) != SetupRoles.None)
						{
							throw new ParseException(Strings.RoleSpecifiedMultipleTimes("Monitoring"));
						}
						setupRoles |= SetupRoles.Monitoring;
						roleCollection.Add(new MonitoringRole());
						goto IL_490;
					case "centraladmin":
					case "eca":
					case "a":
						if ((setupRoles & SetupRoles.CentralAdmin) != SetupRoles.None)
						{
							throw new ParseException(Strings.RoleSpecifiedMultipleTimes("CentralAdmin"));
						}
						setupRoles |= SetupRoles.CentralAdmin;
						roleCollection.Add(new CentralAdminRole());
						goto IL_490;
					case "centraladmindatabase":
					case "cadb":
					case "d":
						if ((setupRoles & SetupRoles.CentralAdminDatabase) != SetupRoles.None)
						{
							throw new ParseException(Strings.RoleSpecifiedMultipleTimes("CentralAdminDatabase"));
						}
						setupRoles |= SetupRoles.CentralAdminDatabase;
						roleCollection.Add(new CentralAdminDatabaseRole());
						goto IL_490;
					case "centraladminfrontend":
					case "mafe":
					case "ma":
						if ((setupRoles & SetupRoles.CentralAdminFrontEnd) != SetupRoles.None)
						{
							throw new ParseException(Strings.RoleSpecifiedMultipleTimes("CentralAdminFrontEnd"));
						}
						setupRoles |= SetupRoles.CentralAdminFrontEnd;
						roleCollection.Add(new CentralAdminFrontEndRole());
						goto IL_490;
					case "frontendtransport":
					case "fet":
					case "ft":
						if ((setupRoles & SetupRoles.FrontendTransport) != SetupRoles.None)
						{
							throw new ParseException(Strings.RoleSpecifiedMultipleTimes("FrontendTransport"));
						}
						setupRoles |= SetupRoles.FrontendTransport;
						roleCollection.Add(new FrontendTransportRole());
						goto IL_490;
					case "clientaccess":
					case "ca":
					case "c":
						CommandLineParser.IsClientAccessRole = true;
						goto IL_490;
					case "m":
					case "mb":
					case "mailbox":
						CommandLineParser.IsMailboxRole = true;
						goto IL_490;
					case "o":
					case "os":
					case "osp":
						if ((setupRoles & SetupRoles.OSP) != SetupRoles.None)
						{
							throw new ParseException(Strings.RoleSpecifiedMultipleTimes("OfficeServicePulse"));
						}
						setupRoles |= SetupRoles.OSP;
						roleCollection.Add(new OSPRole());
						goto IL_490;
					case "":
						goto IL_490;
					}
					throw new ParseException(Strings.InvalidRole(text3));
				}
				IL_490:;
			}
			if (CommandLineParser.IsClientAccessRole)
			{
				if ((setupRoles & SetupRoles.Cafe) == SetupRoles.None)
				{
					roleCollection.Add(new CafeRole());
					CommandLineParser.TotalClientAccessRoles++;
				}
				if ((setupRoles & SetupRoles.FrontendTransport) == SetupRoles.None)
				{
					roleCollection.Add(new FrontendTransportRole());
					CommandLineParser.TotalClientAccessRoles++;
				}
			}
			if (CommandLineParser.IsMailboxRole)
			{
				CommandLineParser.TotalMailboxRoles = 0;
				if ((setupRoles & SetupRoles.Mailbox) == SetupRoles.None)
				{
					roleCollection.Add(new MailboxRole());
					CommandLineParser.TotalMailboxRoles++;
				}
				if ((setupRoles & SetupRoles.ClientAccess) == SetupRoles.None)
				{
					roleCollection.Add(new ClientAccessRole());
					CommandLineParser.TotalMailboxRoles++;
				}
				if ((setupRoles & SetupRoles.Bridgehead) == SetupRoles.None)
				{
					roleCollection.Add(new BridgeheadRole());
					CommandLineParser.TotalMailboxRoles++;
				}
				if ((setupRoles & SetupRoles.UnifiedMessaging) == SetupRoles.None)
				{
					roleCollection.Add(new UnifiedMessagingRole());
					CommandLineParser.TotalMailboxRoles++;
				}
			}
			return roleCollection;
		}

		public static object ParseUInt16(string s)
		{
			ushort num;
			try
			{
				num = ushort.Parse(s);
			}
			catch (FormatException innerException)
			{
				throw new ParseException(Strings.NotAValidNumber(s), innerException);
			}
			catch (OverflowException innerException2)
			{
				throw new ParseException(Strings.NotInTheRange(s), innerException2);
			}
			return num;
		}

		public static object ParseCultureList(string cultures)
		{
			string text = cultures.ToLowerInvariant();
			List<CultureInfo> list = new List<CultureInfo>();
			string[] array = text.Split(",".ToCharArray());
			foreach (string text2 in array)
			{
				string text3 = text2.Trim();
				if (!string.IsNullOrEmpty(text3))
				{
					try
					{
						CultureInfo item = CultureInfo.CreateSpecificCulture(text3);
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
					catch (ArgumentException innerException)
					{
						throw new ParseException(Strings.NotAValidCultureString(text3), innerException);
					}
				}
			}
			return list;
		}

		public static object ParseSourceDir(string path)
		{
			LongPath result = CommandLineParser.ParseToLongPath(path);
			if (!Directory.Exists(path))
			{
				throw new ParseException(Strings.DirectoryNotExist(path));
			}
			return result;
		}

		public static object ParseSourceFile(string file)
		{
			LongPath result = CommandLineParser.ParseToLongPath(file);
			if (!File.Exists(file))
			{
				throw new ParseException(Strings.FileNotExist(file));
			}
			return result;
		}

		public static object ParseNonRootLocalLongFullPath(string path)
		{
			NonRootLocalLongFullPath nonRootLocalLongFullPath;
			try
			{
				nonRootLocalLongFullPath = NonRootLocalLongFullPath.Parse(path);
			}
			catch (ArgumentException innerException)
			{
				throw new ParseException(Strings.NotAValidNonRootLocalLongFullPath(path), innerException);
			}
			if (!Directory.Exists(nonRootLocalLongFullPath.DriveName))
			{
				throw new ParseException(Strings.NotAValidNonRootLocalLongFullPath(path));
			}
			return nonRootLocalLongFullPath;
		}

		public static object ParseOrganizationName(string name)
		{
			return name;
		}

		public static object ParseBool(string s)
		{
			bool flag;
			try
			{
				flag = bool.Parse(s);
			}
			catch (FormatException innerException)
			{
				throw new ParseException(Strings.NotBooleanValue(s), innerException);
			}
			return flag;
		}

		public static object ParseDbFilePath(string path)
		{
			EdbFilePath edbFilePath;
			try
			{
				edbFilePath = EdbFilePath.Parse(path);
			}
			catch (ArgumentException innerException)
			{
				throw new ParseException(Strings.InvalidEdbFilePath(path), innerException);
			}
			catch (FormatException innerException2)
			{
				throw new ParseException(Strings.InvalidEdbFilePath(path), innerException2);
			}
			if (!Directory.Exists(edbFilePath.DriveName))
			{
				throw new ParseException(Strings.InvalidEdbFilePath(path));
			}
			return edbFilePath;
		}

		public Dictionary<string, object> ParseCommandLine(string[] args)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			List<string> args2 = this.ProcessCommandLine(args);
			Dictionary<string, SetupParameter> dictionary2 = base.ParseAll(args2);
			Dictionary<string, SetupParameter> dictionary3 = null;
			if (dictionary2.ContainsKey("roles") && !dictionary2.ContainsKey("mode"))
			{
				dictionary2.Add("mode", base.Parse("mode:install"));
			}
			foreach (SetupParameter setupParameter in dictionary2.Values)
			{
				this.setupLogger.Log(Strings.CommandLineSetupParameterEntry(setupParameter.Name, setupParameter.Value));
			}
			SetupOperations currentOperation = this.CalculateOperationType(dictionary2);
			if (dictionary2.ContainsKey("answerfile"))
			{
				AnswerFileParser answerFileParser = new AnswerFileParser();
				string text = dictionary2["answerfile"].Value as string;
				if (string.IsNullOrEmpty(text))
				{
					throw new ParseException(Strings.AnswerFileNameNotValid(text));
				}
				dictionary3 = answerFileParser.ParseFile(text);
				foreach (SetupParameter setupParameter2 in dictionary3.Values)
				{
					this.setupLogger.Log(Strings.AnswerFileSetupParameterEntry(setupParameter2.Name, setupParameter2.Value));
				}
				answerFileParser.ValidateParameters(dictionary3, currentOperation);
			}
			this.ValidateParameters(dictionary2, currentOperation);
			foreach (SetupParameter setupParameter3 in dictionary2.Values)
			{
				dictionary.Add(setupParameter3.Name, setupParameter3.Value);
			}
			if (dictionary3 != null)
			{
				foreach (SetupParameter setupParameter4 in dictionary3.Values)
				{
					if (dictionary.ContainsKey(setupParameter4.Name))
					{
						throw new ParseException(Strings.ParameterSpecifiedMultipleTimes(setupParameter4.Name));
					}
					dictionary.Add(setupParameter4.Name, setupParameter4.Value);
				}
			}
			return dictionary;
		}

		public abstract SetupOperations CalculateOperationType(Dictionary<string, SetupParameter> parameters);

		private static LongPath ParseToLongPath(string path)
		{
			LongPath result;
			try
			{
				result = LongPath.Parse(path);
			}
			catch (ArgumentException ex)
			{
				throw new ParseException(new LocalizedString(ex.Message), ex);
			}
			return result;
		}

		private List<string> ProcessCommandLine(string[] args)
		{
			List<string> list = new List<string>();
			if (args == null || args.Length == 0)
			{
				return list;
			}
			int i = 0;
			if (!args[i].StartsWith("/") && !args[i].StartsWith("-"))
			{
				throw new ParseException(Strings.ParameterShouldStartWith);
			}
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder(128);
			List<string> list2 = new List<string>(args);
			while (i < list2.Count)
			{
				if (list2[i].Contains(":\""))
				{
					list2[i] = list2[i].Replace(":\"", ":\\");
					string[] array = list2[i].Split(new char[]
					{
						' '
					});
					if (array.Length > 1)
					{
						list2.RemoveAt(i);
						list2.InsertRange(i, array);
					}
				}
				if (list2[i].StartsWith("/") || list2[i].StartsWith("-"))
				{
					flag = (list2[i].IndexOfAny(base.SeparatorCharacters.ToCharArray()) == -1 || list2[i].IndexOfAny(base.SeparatorCharacters.ToCharArray()) == list2[i].Length - 1);
					if (stringBuilder.Length != 0)
					{
						list.Add(stringBuilder.ToString());
					}
					stringBuilder.Clear();
					stringBuilder.Append(list2[i].Substring(1));
				}
				else if (flag)
				{
					if (stringBuilder.Length == 0 || base.SeparatorCharacters.IndexOf(stringBuilder[stringBuilder.Length - 1]) == -1)
					{
						stringBuilder.Append(base.SeparatorCharacters[0]);
						stringBuilder.Append(list2[i]);
					}
					else
					{
						stringBuilder.Append(list2[i]);
					}
					flag = false;
				}
				else
				{
					stringBuilder.Append(",");
					stringBuilder.Append(list2[i]);
				}
				i++;
			}
			if (stringBuilder.Length != 0)
			{
				list.Add(stringBuilder.ToString());
			}
			return list;
		}

		private const string BridgeheadShort = "h";

		private const string BridgeheadMedium = "ht";

		private const string BridgeheadLong = "hubtransport";

		private const string Bridgehead = "HubTransport";

		private const string GatewayShort = "e";

		private const string GatewayMedium = "et";

		private const string GatewayLong = "edgetransport";

		private const string Gateway = "EdgeTransport";

		private const string AdminToolsShort = "t";

		private const string AdminToolsMedium = "mt";

		private const string AdminToolsLong = "managementtools";

		private const string AdminTools = "ManagementTools";

		private const string MonitoringShort = "n";

		private const string MonitoringMedium = "mn";

		private const string MonitoringLong = "monitoring";

		private const string Monitoring = "Monitoring";

		private const string CentralAdminShort = "a";

		private const string CentralAdminMedium = "eca";

		private const string CentralAdminLong = "centraladmin";

		private const string CentralAdmin = "CentralAdmin";

		private const string CentralAdminDatabaseShort = "d";

		private const string CentralAdminDatabaseMedium = "cadb";

		private const string CentralAdminDatabaseLong = "centraladmindatabase";

		private const string CentralAdminDatabase = "CentralAdminDatabase";

		private const string CentralAdminFrontEndShort = "ma";

		private const string CentralAdminFrontEndMedium = "mafe";

		private const string CentralAdminFrontEndLong = "centraladminfrontend";

		private const string CentralAdminFrontEnd = "CentralAdminFrontEnd";

		private const string FrontendTransportShort = "ft";

		private const string FrontendTransportMedium = "fet";

		private const string FrontendTransportLong = "frontendtransport";

		private const string FrontendTransport = "FrontendTransport";

		private const string MailboxShort = "m";

		private const string MailboxMedium = "mb";

		private const string MailboxLong = "mailbox";

		private const string Mailbox = "Mailbox";

		private const string ClientAccessShort = "c";

		private const string ClientAccessMedium = "ca";

		private const string ClientAccessLong = "clientaccess";

		private const string ClientAccess = "ClientAccess";

		private const string OSPShort = "o";

		private const string OSPMedium = "os";

		private const string OSPLong = "osp";

		private const string OSP = "OfficeServicePulse";

		private const string RoleSeparator = ",";

		private const string NetworkSeparator = ",";

		private const string CultureSeparator = ",";

		private readonly ISetupLogger setupLogger;
	}
}
