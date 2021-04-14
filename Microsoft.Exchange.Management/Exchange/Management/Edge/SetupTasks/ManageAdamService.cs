using System;
using System.CodeDom.Compiler;
using System.DirectoryServices;
using System.IO;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	internal class ManageAdamService
	{
		public static void InstallAdam(string instanceName, string dataPath, string logPath, int port, int sslPort, WriteVerboseDelegate writeVerbose)
		{
			Utils.CreateDirectory(dataPath, "DataFilesPath");
			Utils.CreateDirectory(logPath, "LogFilesPath");
			AdamServiceSettings.DeleteFromRegistry(instanceName);
			AdamServiceSettings adamServiceSettings = new AdamServiceSettings(instanceName, Path.Combine(dataPath, "Adam"), Path.Combine(logPath, "Adam"), port, sslPort);
			using (TempFileCollection tempFileCollection = new TempFileCollection())
			{
				string answerFileName = ManageAdamService.MakeAnswerFile(tempFileCollection, adamServiceSettings);
				ManageAdamService.InstallAdamInstance(answerFileName, adamServiceSettings, writeVerbose);
				adamServiceSettings.SaveToRegistry();
			}
		}

		public static void UninstallAdam(string instanceName)
		{
			ManageAdamService.RunAdamUninstall(instanceName);
			AdamServiceSettings.DeleteFromRegistry(instanceName);
		}

		public static void ImportAdamSchema(string instanceName, string schemaFilePath, string macroName, string macroValue)
		{
			string tempDir = Utils.GetTempDir();
			Directory.CreateDirectory(tempDir);
			try
			{
				int num = ManageAdamService.RunAdamSchemaImport(instanceName, schemaFilePath, macroName, macroValue, tempDir);
				string schemaImportLogFilePath = Path.Combine(tempDir, "ldif.log");
				ManageAdamService.AppendSchemaImportLogFileIfExists(schemaImportLogFilePath, ManageAdamService.AdamSchemaImportCumulativeLogFilePath, instanceName, schemaFilePath, macroName, macroValue);
				string schemaImportLogFilePath2 = Path.Combine(tempDir, "ldif.err");
				ManageAdamService.AppendSchemaImportLogFileIfExists(schemaImportLogFilePath2, ManageAdamService.AdamSchemaImportCumulativeErrorFilePath, instanceName, schemaFilePath, macroName, macroValue);
				if (num != 0)
				{
					throw new AdamSchemaImportProcessFailureException("ldifde.exe", num);
				}
			}
			finally
			{
				Directory.Delete(tempDir, true);
			}
		}

		public static void AppendSchemaImportLogFileIfExists(string schemaImportLogFilePath, string schemaImportCumulativeLogFilePath, string instanceName, string schemaFilePath, string macroName, string macroValue)
		{
			if (File.Exists(schemaImportLogFilePath))
			{
				bool flag = File.Exists(schemaImportCumulativeLogFilePath);
				using (StreamWriter streamWriter = File.AppendText(schemaImportCumulativeLogFilePath))
				{
					if (flag)
					{
						streamWriter.WriteLine("");
					}
					streamWriter.WriteLine(ExDateTime.Now);
					streamWriter.WriteLine("Parameters for ADAM LDF file import:");
					streamWriter.WriteLine("Instance name: \"" + instanceName + "\"");
					streamWriter.WriteLine("Schema file path: \"" + schemaFilePath + "\"");
					streamWriter.WriteLine("Macro name: \"" + macroName + "\"");
					streamWriter.WriteLine("Macro value: \"" + macroValue + "\"");
					streamWriter.WriteLine("Log output:");
					string value = File.ReadAllText(schemaImportLogFilePath);
					streamWriter.Write(value);
				}
			}
		}

		public static string GetAdamServiceName(string instanceName)
		{
			return "ADAM_" + instanceName;
		}

		private static void InstallAdamInstance(string answerFileName, AdamServiceSettings adamServiceSettings, WriteVerboseDelegate writeVerbose)
		{
			TaskLogger.LogEnter();
			ManageAdamService.RunAdamInstall(answerFileName);
			ManageAdamService.SetAdamServiceArgs(adamServiceSettings.InstanceName, writeVerbose);
			ManageAdamService.SetAcls(adamServiceSettings);
			TaskLogger.LogExit();
		}

		private static void RunAdamInstall(string answerFileName)
		{
			string arguments = "/answer:\"" + answerFileName + "\"";
			ManageAdamService.RunAdamInstallUninstallProcess(true, "adamInstall.exe", arguments, ManageAdamService.GetAdamToolsDir());
		}

		private static void SetAdamServiceArgs(string instanceName, WriteVerboseDelegate writeVerbose)
		{
			string arguments = string.Concat(new string[]
			{
				"description ",
				ManageAdamService.GetAdamServiceName(instanceName),
				" \"",
				Strings.AdamServiceDescription,
				"\""
			});
			int num = Utils.LogRunProcess("sc.exe", arguments, null);
			if (num != 0)
			{
				writeVerbose(Strings.AdamFailedSetServiceArgs("sc.exe", num, "Description"));
			}
			arguments = string.Concat(new string[]
			{
				"config ",
				ManageAdamService.GetAdamServiceName(instanceName),
				" DisplayName= \"",
				Strings.AdamServiceDisplayName,
				"\""
			});
			num = Utils.LogRunProcess("sc.exe", arguments, null);
			if (num != 0)
			{
				writeVerbose(Strings.AdamFailedSetServiceArgs("sc.exe", num, "DisplayName"));
			}
		}

		private static string GetAdamInstallExePath()
		{
			string adamToolsDir = ManageAdamService.GetAdamToolsDir();
			return Path.Combine(adamToolsDir, "adamInstall.exe");
		}

		private static void RunAdamUninstall(string instanceName)
		{
			string arguments = "/q /force /i:\"" + instanceName + "\"";
			ManageAdamService.RunAdamInstallUninstallProcess(false, "adamUninstall.exe", arguments, ManageAdamService.GetAdamToolsDir());
		}

		private static void SetAcls(AdamServiceSettings adamServiceSettings)
		{
			ManageAdamService.RunDsAcls(adamServiceSettings, "OU=MSExchangeGateway");
			using (DirectoryEntry rootDirectoryEntry = AdsUtils.GetRootDirectoryEntry(adamServiceSettings.LdapPort))
			{
				string text = (string)rootDirectoryEntry.Properties["ConfigurationNamingContext"].Value;
				ManageAdamService.RunDsAcls(adamServiceSettings, text);
				string subTreeDn = "CN=Deleted Objects," + text;
				ManageAdamService.RunDsAcls(adamServiceSettings, subTreeDn);
				ManageAdamService.SetAdministrator(adamServiceSettings, text);
			}
		}

		private static void SetAdministrator(AdamServiceSettings adamServiceSettings, string configContainerDn)
		{
			string path = string.Format("{0}:{1}/CN=Administrators,CN=Roles,{2}", "LDAP://localhost", adamServiceSettings.LdapPort, configContainerDn);
			string value = string.Format("<SID={0}>", ManageAdamService.BuiltinAdminSid);
			using (DirectoryEntry directoryEntry = new DirectoryEntry(path))
			{
				directoryEntry.Properties["member"].Add(value);
				directoryEntry.CommitChanges();
			}
		}

		private static void RunDsAcls(AdamServiceSettings adamServiceSettings, string subTreeDn)
		{
			string arguments = string.Format("\"\\\\localhost:{0}\\{1}\" /I:T /G \"NT AUTHORITY\\SYSTEM\":GR;; \"NT AUTHORITY\\NETWORKSERVICE\":GR;; \"{2}\":GA;;", adamServiceSettings.LdapPort, subTreeDn, ManageAdamService.BuiltinAdminSid);
			int num = Utils.LogRunProcess("dsacls.exe", arguments, ManageAdamService.GetAdamToolsDir());
			if (num != 0)
			{
				throw new AdamSetAclsProcessFailureException("dsacls.exe", num, subTreeDn);
			}
		}

		private static string GetBuiltinAdminSid()
		{
			return new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).Value;
		}

		private static int RunAdamSchemaImport(string instanceName, string schemaFilePath, string macroName, string macroValue, string tempDir)
		{
			int ldapPort = AdamServiceSettings.GetFromRegistry(instanceName).LdapPort;
			string schemaImportProcessArguments = ManageAdamService.GetSchemaImportProcessArguments(schemaFilePath, ldapPort, tempDir, macroName, macroValue);
			return Utils.LogRunProcess("ldifde.exe", schemaImportProcessArguments, ManageAdamService.GetAdamToolsDir());
		}

		private static string GetSchemaImportProcessArguments(string schemaFilePath, int serviceLdapPort, string tempDir, string macroName, string macroValue)
		{
			return string.Format("-i -f \"{0}\" -s localhost:{1} -j \"{2}\" -c \"{3}\" \"{4}\"", new object[]
			{
				schemaFilePath,
				serviceLdapPort,
				tempDir,
				macroName,
				macroValue
			});
		}

		private static void RunAdamInstallUninstallProcess(bool installing, string fileName, string arguments, string path)
		{
			ManageAdamService.ClearAdamInstallUninstallResults();
			int num = Utils.LogRunProcess(fileName, arguments, path);
			ManageAdamService.CheckAdamInstallUninstallResults(installing);
			if (num == 0)
			{
				return;
			}
			if (!installing)
			{
				Strings.AdamUninstallProcessFailure(fileName, num);
			}
			else
			{
				Strings.AdamInstallProcessFailure(fileName, num);
			}
			if (installing)
			{
				throw new AdamInstallProcessFailureException(fileName, num);
			}
			throw new AdamUninstallProcessFailureException(fileName, num);
		}

		private static void ClearAdamInstallUninstallResults()
		{
			Utils.DeleteRegSubKeyTreeIfExist(Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\ADAM_Installer_Results");
		}

		private static void CheckAdamInstallUninstallResults(bool installing)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\ADAM_Installer_Results"))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue(installing ? "ADAMInstallWarnings" : "ADAMUninstallWarnings");
					if (value != null)
					{
						string warning = value as string;
						TaskLogger.Log(installing ? Strings.AdamInstallWarning(warning) : Strings.AdamUninstallWarning(warning));
					}
					object value2 = registryKey.GetValue(installing ? "ADAMInstallErrorCode" : "ADAMUninstallErrorCode");
					object value3 = registryKey.GetValue(installing ? "ADAMInstallErrorMessage" : "ADAMUninstallErrorMessage");
					if (value2 != null || value3 != null)
					{
						if (value3 != null)
						{
							string error = value3 as string;
							TaskLogger.Log(installing ? Strings.AdamInstallError(error) : Strings.AdamUninstallError(error));
							if (installing)
							{
								throw new AdamInstallErrorException(error);
							}
							throw new AdamUninstallErrorException(error);
						}
						else
						{
							int num = (int)value2;
							if (installing && 20033 == num)
							{
								TaskLogger.Log(Strings.AdamInstallFailureDataOrLogFolderNotEmpty);
								throw new AdamInstallFailureDataOrLogFolderNotEmptyException();
							}
							TaskLogger.Log(installing ? Strings.AdamInstallGeneralFailureWithResult(num) : Strings.AdamUninstallGeneralFailureWithResult(num));
							if (installing)
							{
								throw new AdamInstallGeneralFailureWithResultException(num);
							}
							throw new AdamUninstallGeneralFailureWithResultException(num);
						}
					}
				}
			}
		}

		private static string MakeAnswerFile(TempFileCollection tempFiles, AdamServiceSettings adamServiceSettings)
		{
			string text = tempFiles.AddExtension("ini");
			string path = Path.Combine(ConfigurationContext.Setup.SetupDataPath, "AdamInstallAnswer.ini");
			using (StreamReader streamReader = File.OpenText(path))
			{
				using (StreamWriter streamWriter = File.CreateText(text))
				{
					string text2;
					while ((text2 = streamReader.ReadLine()) != null)
					{
						if (!string.IsNullOrEmpty(text2) && text2[0] != ';')
						{
							if (text2.StartsWith("InstanceName"))
							{
								text2 = Utils.MakeIniFileSetting("InstanceName", adamServiceSettings.InstanceName);
							}
							else if (text2.StartsWith("DataFilesPath"))
							{
								text2 = Utils.MakeIniFileSetting("DataFilesPath", adamServiceSettings.DataFilesPath);
							}
							else if (text2.StartsWith("LogFilesPath"))
							{
								text2 = Utils.MakeIniFileSetting("LogFilesPath", adamServiceSettings.LogFilesPath);
							}
							else if (text2.StartsWith("LocalLDAPPortToListenOn"))
							{
								text2 = Utils.MakeIniFileSetting("LocalLDAPPortToListenOn", adamServiceSettings.LdapPort.ToString());
							}
							else if (text2.StartsWith("LocalSSLPortToListenOn"))
							{
								text2 = Utils.MakeIniFileSetting("LocalSSLPortToListenOn", adamServiceSettings.SslPort.ToString());
							}
							else if (text2.StartsWith("NewApplicationPartitionToCreate"))
							{
								text2 = Utils.MakeIniFileSetting("NewApplicationPartitionToCreate", "OU=MSExchangeGateway");
							}
							streamWriter.WriteLine(text2);
							TaskLogger.Log(new LocalizedString("Answer File:" + text2));
						}
					}
					streamWriter.Flush();
				}
			}
			return text;
		}

		private static string GetAdamToolsDir()
		{
			return Path.Combine(Utils.GetWindowsDir(), "ADAM");
		}

		private const string InstanceIniKey = "InstanceName";

		private const string DataFilesIniKey = "DataFilesPath";

		private const string LogFilesPathIniKey = "LogFilesPath";

		private const string LdapPortIniKey = "LocalLDAPPortToListenOn";

		private const string SslPortIniKey = "LocalSSLPortToListenOn";

		internal const string NewPartitionIniKey = "NewApplicationPartitionToCreate";

		private const string AdamInstallerResultsRegKey = "Software\\Microsoft\\Windows\\CurrentVersion\\ADAM_Installer_Results";

		private const string AdamInstallErrorCodeRegValueName = "ADAMInstallErrorCode";

		private const string AdamInstallErrorMessageRegValueName = "ADAMInstallErrorMessage";

		private const string AdamInstallWarningsRegValueName = "ADAMInstallWarnings";

		private const string AdamUninstallErrorCodeRegValueName = "ADAMUninstallErrorCode";

		private const string AdamUninstallErrorMessageRegValueName = "ADAMUninstallErrorMessage";

		private const string AdamUninstallWarningsRegValueName = "ADAMUninstallWarnings";

		private const string AdamSharedToolsFolderName = "ADAM";

		private const string AdamInstallExeFileName = "adamInstall.exe";

		private const string AdamUninstallExeFileName = "adamUninstall.exe";

		private const string AdamSchemaImportExportExeFileName = "ldifde.exe";

		private const string AdamSchemaImportExportLogFileName = "ldif.log";

		private const string AdamSchemaImportExportErrorFileName = "ldif.err";

		private const string AdamUnattendTemplateFileName = "AdamInstallAnswer.ini";

		private const string AdamDsAclsExeFileName = "dsacls.exe";

		private const string AdamExchangeSubdirName = "Adam";

		public const string AdamServiceNamePrefix = "ADAM_";

		private const string ServiceControlExeFileName = "sc.exe";

		internal static readonly string AdamSchemaImportCumulativeLogFilePath = Path.Combine(Utils.GetSetupLogDir(), "ldif.log");

		internal static readonly string AdamSchemaImportCumulativeErrorFilePath = Path.Combine(Utils.GetSetupLogDir(), "ldif.err");

		private static readonly string BuiltinAdminSid = ManageAdamService.GetBuiltinAdminSid();

		private enum AdamInstallExitCodes
		{
			DataOrLogFolderNotEmpty = 20033
		}
	}
}
