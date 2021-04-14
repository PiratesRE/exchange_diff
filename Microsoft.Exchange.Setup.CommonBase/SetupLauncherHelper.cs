using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Analysis;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Setup.AcquireLanguagePack;
using Microsoft.Exchange.Setup.Bootstrapper.Common;
using Microsoft.Exchange.Setup.Parser;
using Microsoft.Win32;

namespace Microsoft.Exchange.Setup.CommonBase
{
	internal static class SetupLauncherHelper
	{
		public static IRegistryDataProvider RegistryProvider { get; set; }

		public static bool SetupRequiredFilesUpdated(IEnumerable<string> oldList, IEnumerable<string> newList, string dirToCheck)
		{
			if (string.IsNullOrEmpty(dirToCheck))
			{
				throw new ArgumentNullException("dirToCheck");
			}
			if (!Directory.Exists(dirToCheck))
			{
				throw new ArgumentException("dirToCheck");
			}
			if (oldList == null)
			{
				throw new ArgumentNullException("oldList");
			}
			if (newList != null)
			{
				if (!newList.All((string x) => oldList.Any((string y) => x.Equals(y, StringComparison.InvariantCultureIgnoreCase))))
				{
					return true;
				}
			}
			IEnumerable<string> source = Directory.EnumerateFiles(dirToCheck);
			IEnumerable<string> setupRequiredFiles = newList ?? oldList;
			return source.Any((string x) => setupRequiredFiles.Any((string y) => x.IndexOf(y, StringComparison.InvariantCultureIgnoreCase) >= 0));
		}

		public static bool IsFromInstalledExchangeDir()
		{
			string installedExchangeDir = SetupLauncherHelper.GetInstalledExchangeDir();
			if (string.IsNullOrEmpty(installedExchangeDir))
			{
				return false;
			}
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (string.IsNullOrEmpty(directoryName))
			{
				return false;
			}
			string text = string.Format("{0}{1}", installedExchangeDir, "bin");
			return text.Equals(directoryName, StringComparison.InvariantCultureIgnoreCase);
		}

		public static bool IsRestart(Dictionary<string, object> parsedArguments)
		{
			return parsedArguments.ContainsKey("restart");
		}

		public static bool AreEarlierVersionsOfServerRolesInstalled()
		{
			bool flag = SetupLauncherHelper.HasRegistryKey(SetupChecksRegistryConstant.RegistryExchangePathE12, null) != null;
			bool flag2 = SetupLauncherHelper.HasRegistryKey(SetupChecksRegistryConstant.RegistryExchangePathE14, null) != null;
			if (!flag && !flag2)
			{
				return false;
			}
			if (flag)
			{
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE12, "HubTransportRole"), null) != null)
				{
					return true;
				}
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE12, "ClientAccessRole"), null) != null)
				{
					return true;
				}
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE12, "Hygiene"), null) != null)
				{
					return true;
				}
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE12, "MailboxRole"), null) != null)
				{
					return true;
				}
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE12, "UnifiedMessagingRole"), null) != null)
				{
					return true;
				}
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE12, "AdminToolsRole"), null) != null)
				{
					return true;
				}
			}
			else
			{
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE14, "HubTransportRole"), null) != null)
				{
					return true;
				}
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE14, "ClientAccessRole"), null) != null)
				{
					return true;
				}
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE14, "Hygiene"), null) != null)
				{
					return true;
				}
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE14, "MailboxRole"), null) != null)
				{
					return true;
				}
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE14, "UnifiedMessagingRole"), null) != null)
				{
					return true;
				}
				if (SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePathE14, "AdminToolsRole"), null) != null)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsExchangeInstalled()
		{
			return !string.IsNullOrEmpty(SetupLauncherHelper.GetInstalledExchangeDir());
		}

		public static bool IsUninstallMode(Dictionary<string, object> parsedArguments)
		{
			if (parsedArguments != null && parsedArguments.ContainsKey("mode"))
			{
				SetupOperations setupOperations = (SetupOperations)parsedArguments["mode"];
				if (setupOperations == SetupOperations.Uninstall)
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsUpgradeMode(Dictionary<string, object> parsedArguments)
		{
			if (parsedArguments != null && parsedArguments.ContainsKey("mode"))
			{
				SetupOperations setupOperations = (SetupOperations)parsedArguments["mode"];
				if (setupOperations == SetupOperations.Upgrade)
				{
					return true;
				}
			}
			return false;
		}

		public static bool UpdatesDirContainsValidFiles(string dirToCheck)
		{
			return LanguagePackXmlHelper.ContainsOnlyDownloadedFiles(dirToCheck);
		}

		public static bool CopyExSetupFiles(string rootDir)
		{
			string srcDir = Path.Combine(rootDir, "Setup\\ServerRoles\\Common");
			string dstDir = Path.Combine(SetupHelper.WindowsDir, "Temp\\ExchangeSetup");
			bool result = true;
			bool isSourceRemote = SetupHelper.IsSourceRemote;
			if (isSourceRemote)
			{
				SetupLauncherHelper.Log(Strings.RemoteCopyAllSetupFilesStart(srcDir, dstDir));
			}
			else
			{
				SetupLauncherHelper.Log(Strings.LocalCopyAllSetupFilesStart(srcDir, dstDir));
			}
			try
			{
				SetupHelper.CopyFiles(srcDir, dstDir, false);
			}
			catch (InsufficientDiskSpaceException e)
			{
				SetupLauncherHelper.LogError(e);
				result = false;
			}
			catch (FileNotExistsException e2)
			{
				SetupLauncherHelper.LogError(e2);
				result = false;
			}
			if (isSourceRemote)
			{
				SetupLauncherHelper.Log(Strings.RemoteCopyAllSetupFilesEnd(srcDir, dstDir));
			}
			else
			{
				SetupLauncherHelper.Log(Strings.LocalCopyAllSetupFilesEnd(srcDir, dstDir));
			}
			return result;
		}

		public static bool CopyMspFiles(string updatesDir, string destDir)
		{
			if (string.IsNullOrEmpty(updatesDir))
			{
				throw new ArgumentNullException("updatesDir");
			}
			if (!Directory.Exists(updatesDir))
			{
				throw new ArgumentException("updatesDir");
			}
			if (string.IsNullOrEmpty(destDir))
			{
				throw new ArgumentNullException("destDir");
			}
			if (!Directory.Exists(destDir))
			{
				throw new ArgumentException("destDir");
			}
			try
			{
				SetupHelper.CopyFiles(updatesDir, destDir, true, SetupChecksFileConstant.GetSetupRequiredFiles());
			}
			catch (InsufficientDiskSpaceException e)
			{
				SetupLauncherHelper.LogError(e);
				return false;
			}
			catch (FileNotExistsException e2)
			{
				SetupLauncherHelper.LogError(e2);
				return false;
			}
			return true;
		}

		public static int LoadAssembly(string[] args, Dictionary<string, object> parsedArguments, SetupBase theApp, string assemblyFileName, string assemblyTypeFullName)
		{
			string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			AssemblyResolver[] resolvers = AssemblyResolver.Install(new AssemblyResolver[]
			{
				new FileSearchAssemblyResolver
				{
					FileNameFilter = new Func<string, bool>(AssemblyResolver.ExchangePrefixedAssembliesOnly),
					SearchPaths = new string[]
					{
						directoryName
					},
					Recursive = true
				}
			});
			int num = 0;
			try
			{
				string text = Path.Combine(directoryName, assemblyFileName);
				SetupLauncherHelper.Log(Strings.AssemblyDLLFileLocation(text));
				object[] parameters = new object[]
				{
					args,
					parsedArguments,
					theApp
				};
				Assembly assembly = Assembly.LoadFile(text);
				foreach (Type type in assembly.GetTypes())
				{
					if (type.IsClass && type.FullName == assemblyTypeFullName)
					{
						object obj = Activator.CreateInstance(type);
						MethodInfo method = type.GetMethod("StartMain");
						num = (int)method.Invoke(obj, parameters);
						break;
					}
				}
			}
			catch (Exception e)
			{
				SetupLauncherHelper.LogError(e);
				SetupLauncherHelper.Log(new LocalizedString("CurrentResult SetupLauncherHelper.loadassembly:444: " + 1));
				return 1;
			}
			finally
			{
				AssemblyResolver.Uninstall(resolvers);
			}
			SetupLauncherHelper.Log(new LocalizedString("CurrentResult SetupLauncherHelper.loadassembly:452: " + num));
			return num;
		}

		public static int CompareRunningVersionWithInstalledVersion(string fullFileName)
		{
			string versionOfInstalledExe = SetupLauncherHelper.GetVersionOfInstalledExe();
			string versionOfRunningApplication = SetupLauncherHelper.GetVersionOfRunningApplication(fullFileName);
			if (!string.IsNullOrEmpty(versionOfInstalledExe))
			{
				return AnalysisHelpers.VersionCompare(versionOfInstalledExe, versionOfRunningApplication);
			}
			return 0;
		}

		public static string GetUpdatesDirFromRegistry()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(SetupChecksRegistryConstant.RegistryPathForLanguagePack, true))
			{
				if (registryKey != null)
				{
					string text = (string)registryKey.GetValue("LanguagePackBundlePath");
					if (Directory.Exists(text))
					{
						return text;
					}
				}
			}
			return null;
		}

		public static void LogInstalledExchangeDirAcl()
		{
			string installedExchangeDir = SetupLauncherHelper.GetInstalledExchangeDir();
			if (string.IsNullOrEmpty(installedExchangeDir))
			{
				return;
			}
			int? num = (int?)SetupLauncherHelper.HasRegistryKey(Path.Combine(SetupChecksRegistryConstant.RegistryExchangePath, "Setup"), "LogAcl");
			if (num == null || num.Value == 0)
			{
				return;
			}
			try
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(installedExchangeDir);
				if ((num.Value & 1) != 0)
				{
					SetupLauncherHelper.Log(Strings.DirectoryAclInfo(installedExchangeDir, SetupLauncherHelper.GetFormattedAclInfo(directoryInfo)));
				}
				if ((num.Value & 2) != 0)
				{
					foreach (DirectoryInfo directoryInfo2 in directoryInfo.EnumerateDirectories("*", SearchOption.AllDirectories))
					{
						SetupLauncherHelper.Log(Strings.DirectoryAclInfo(directoryInfo2.FullName, SetupLauncherHelper.GetFormattedAclInfo(directoryInfo2)));
					}
				}
			}
			catch (Exception e)
			{
				SetupLauncherHelper.LogError(e);
			}
		}

		internal static string GetFormattedAclInfo(DirectoryInfo dirInfo)
		{
			if (dirInfo == null)
			{
				return string.Empty;
			}
			Type typeFromHandle = typeof(NTAccount);
			DirectorySecurity accessControl = dirInfo.GetAccessControl(AccessControlSections.All);
			StringBuilder stringBuilder = new StringBuilder();
			IdentityReference owner = accessControl.GetOwner(typeFromHandle);
			IdentityReference group = accessControl.GetGroup(typeFromHandle);
			stringBuilder.AppendLine(string.Format("Owner: {0}", (owner == null) ? string.Empty : owner.Value));
			stringBuilder.AppendLine(string.Format("Group: {0}", (group == null) ? string.Empty : group.Value));
			stringBuilder.AppendLine("FileSystemAccessRules:");
			foreach (object obj in accessControl.GetAccessRules(true, true, typeFromHandle))
			{
				FileSystemAccessRule fileSystemAccessRule = (FileSystemAccessRule)obj;
				stringBuilder.AppendLine(string.Format("Identity: {0}", (fileSystemAccessRule.IdentityReference == null) ? string.Empty : fileSystemAccessRule.IdentityReference.Value));
				stringBuilder.AppendLine(string.Format("Type: {0}", fileSystemAccessRule.AccessControlType));
				stringBuilder.AppendLine(string.Format("Rights: {0}", fileSystemAccessRule.FileSystemRights));
				stringBuilder.AppendLine(string.Format("IsInherited: {0}", fileSystemAccessRule.IsInherited));
				stringBuilder.AppendLine(string.Format("PropagationFlags: {0}", fileSystemAccessRule.PropagationFlags));
				stringBuilder.AppendLine(string.Format("InheritanceFlags: {0}", fileSystemAccessRule.InheritanceFlags));
			}
			return stringBuilder.ToString();
		}

		private static string GetInstalledExchangeDir()
		{
			return (string)SetupLauncherHelper.HasRegistryKey(string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePath, "Setup"), "MsiInstallPath");
		}

		private static object HasRegistryKey(string path, string name)
		{
			if (SetupLauncherHelper.RegistryProvider == null)
			{
				SetupLauncherHelper.RegistryProvider = new RegistryDataProvider();
			}
			try
			{
				return SetupLauncherHelper.RegistryProvider.GetRegistryKeyValue(Registry.LocalMachine, path, name);
			}
			catch (FailureException ex)
			{
				SetupLauncherHelper.Log(Strings.MissingRegistryKey(ex.Message));
			}
			return null;
		}

		private static string GetVersionOfRunningApplication(string fullFileName)
		{
			return SetupLauncherHelper.ConvertVersionToNumericLike(SetupHelper.GetVersionOfApplication(fullFileName));
		}

		private static string GetVersionOfInstalledExe()
		{
			string result = string.Empty;
			if (SetupLauncherHelper.IsExchangeInstalled())
			{
				string path = string.Format("{0}\\{1}", SetupChecksRegistryConstant.RegistryExchangePath, "Setup");
				int? num = (int?)SetupLauncherHelper.HasRegistryKey(path, "MsiProductMajor");
				int? num2 = (int?)SetupLauncherHelper.HasRegistryKey(path, "MsiProductMinor");
				int? num3 = (int?)SetupLauncherHelper.HasRegistryKey(path, "MsiBuildMajor");
				int? num4 = (int?)SetupLauncherHelper.HasRegistryKey(path, "MsiBuildMinor");
				if (num != null && num2 != null && num3 != null && num4 != null)
				{
					result = SetupLauncherHelper.ConvertVersionToNumericLike(string.Format("{0}.{1}.{2}.{3}", new object[]
					{
						num.ToString(),
						num2.ToString(),
						num3.ToString(),
						num4.ToString()
					}));
				}
			}
			return result;
		}

		private static string ConvertVersionToNumericLike(string buildVersion)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			if (!string.IsNullOrEmpty(buildVersion))
			{
				try
				{
					string[] array = buildVersion.Split(new char[]
					{
						'.'
					});
					for (int i = 0; i < 4; i++)
					{
						stringBuilder.Append(Convert.ToInt32(array[i]).ToString());
						if (i != 3)
						{
							stringBuilder.Append('.');
						}
					}
					goto IL_78;
				}
				catch (Exception e)
				{
					SetupLauncherHelper.LogError(e);
					SetupLauncherHelper.Log(Strings.UnableToFindBuildVersion);
					goto IL_78;
				}
			}
			SetupLauncherHelper.Log(Strings.UnableToFindBuildVersion);
			IL_78:
			return stringBuilder.ToString();
		}

		private static void Log(LocalizedString message)
		{
			if (SetupBase.TheApp != null)
			{
				SetupBase.TheApp.Logger.Log(message);
			}
		}

		private static void LogError(Exception e)
		{
			if (SetupBase.TheApp != null)
			{
				SetupBase.TheApp.Logger.LogError(e);
			}
		}

		private const string StartMainMethodName = "StartMain";
	}
}
