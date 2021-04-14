using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Management;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Install", "Languages", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class InstallLanguages : ComponentInfoBasedTask
	{
		[Parameter(Mandatory = true)]
		public LongPath LangPackPath
		{
			get
			{
				return (LongPath)base.Fields["LangPackPath"];
			}
			set
			{
				base.Fields["LangPackPath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public NonRootLocalLongFullPath InstallPath
		{
			get
			{
				return (NonRootLocalLongFullPath)base.Fields["InstallPath"];
			}
			set
			{
				base.Fields["InstallPath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string[] LanguagePacksToInstall
		{
			get
			{
				return (string[])base.Fields["LanguagePacksToInstall"];
			}
			set
			{
				base.Fields["LanguagePacksToInstall"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string[] LPClientFlags
		{
			get
			{
				return (string[])base.Fields["LPClientFlags"];
			}
			set
			{
				base.Fields["LPClientFlags"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string[] LPServerFlags
		{
			get
			{
				return (string[])base.Fields["LPServerFlags"];
			}
			set
			{
				base.Fields["LPServerFlags"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public bool SourceIsBundle
		{
			get
			{
				return (bool)base.Fields["SourceIsBundle"];
			}
			set
			{
				base.Fields["SourceIsBundle"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public InstallationModes InstallMode
		{
			get
			{
				return this.InstallationMode;
			}
			set
			{
				this.InstallationMode = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath LogFilePath
		{
			get
			{
				return (LocalLongFullPath)base.Fields["LogFilePath"];
			}
			set
			{
				base.Fields["LogFilePath"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string InstallVersion
		{
			get
			{
				return (string)base.Fields["InstallVersion"];
			}
			set
			{
				base.Fields["InstallVersion"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string ClientLPVersion
		{
			get
			{
				return (string)base.Fields["ClientLPVersion"];
			}
			set
			{
				base.Fields["ClientLPVersion"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				List<string> list = new List<string>();
				list.Add(this.InstallPath.PathName + "\\Setup\\Perf");
				string platform = base.Platform;
				list.Add(this.InstallPath.PathName + "\\Bin\\perf\\" + platform);
				foreach (string text in list)
				{
					if (Directory.Exists(text))
					{
						if (Directory.GetDirectories(text).Length > 1)
						{
							MergePerfCounterIni mergePerfCounterIni = new MergePerfCounterIni();
							mergePerfCounterIni.ParseDirectories(text);
							this.ParsePerfCounterDefinitionFiles(text);
						}
					}
					else
					{
						TaskLogger.Log(Strings.ExceptionDirectoryNotFound(text));
					}
				}
				if (ManageServiceBase.GetServiceStatus("Winmgmt") == ServiceControllerStatus.Running)
				{
					this.UpdateMFLFiles();
				}
				else
				{
					TaskLogger.Log(Strings.SkippedUpdatingMFLFiles);
				}
				this.UpdateOwaLPs();
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void UpdateOwaLPs()
		{
			TaskLogger.LogEnter();
			TaskLogger.Log(Strings.StartingToCopyOwaLPFiles);
			try
			{
				if (this.InstallVersion == this.ClientLPVersion)
				{
					TaskLogger.Log(Strings.SameOwaVersionDoNotCopyLPFiles);
				}
				else
				{
					string sourcePathForLPfiles = string.Format("{0}\\ClientAccess\\owa\\prem\\{1}\\scripts\\", this.InstallPath.PathName, this.ClientLPVersion);
					string destinationPathForLPfiles = string.Format("{0}\\ClientAccess\\owa\\prem\\{1}\\scripts\\", this.InstallPath.PathName, this.InstallVersion);
					string sourcePathForLPfiles2 = string.Format("{0}\\ClientAccess\\owa\\prem\\{1}\\ext\\def\\", this.InstallPath.PathName, this.ClientLPVersion);
					string destinationPathForLPfiles2 = string.Format("{0}\\ClientAccess\\owa\\prem\\{1}\\ext\\def\\", this.InstallPath.PathName, this.InstallVersion);
					this.CopyLanguageDirectories(sourcePathForLPfiles, destinationPathForLPfiles);
					this.CopyLanguageDirectories(sourcePathForLPfiles2, destinationPathForLPfiles2);
				}
			}
			finally
			{
				TaskLogger.Log(Strings.FinishedCopyingOwaLPFiles);
				TaskLogger.LogExit();
			}
		}

		private void CopyLanguageDirectories(string sourcePathForLPfiles, string destinationPathForLPfiles)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourcePathForLPfiles);
			if (directoryInfo.Exists)
			{
				DirectoryInfo[] directories = directoryInfo.GetDirectories();
				foreach (DirectoryInfo directoryInfo2 in directories)
				{
					try
					{
						CultureInfo.GetCultureInfo(directoryInfo2.Name);
						this.CopyDirectory(directoryInfo2.FullName, destinationPathForLPfiles + directoryInfo2.Name);
					}
					catch (CultureNotFoundException)
					{
					}
				}
				return;
			}
			TaskLogger.LogWarning(Strings.WarningLPDirectoryNotFound(sourcePathForLPfiles));
		}

		private void UpdateMFLFiles()
		{
			TaskLogger.LogEnter();
			TaskLogger.Log(Strings.StartingToUpdateMFLFiles);
			ManagementObjectSearcher managementObjectSearcher = null;
			try
			{
				ConnectionOptions options = new ConnectionOptions();
				ManagementScope scope = new ManagementScope("root\\cimv2", options);
				ObjectQuery query = new ObjectQuery("select * from __Namespace");
				managementObjectSearcher = new ManagementObjectSearcher(scope, query);
				using (ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get())
				{
					int count = managementObjectCollection.Count;
					int num = 0;
					foreach (ManagementBaseObject managementBaseObject in managementObjectCollection)
					{
						ManagementObject managementObject = (ManagementObject)managementBaseObject;
						if (managementObject["Name"].ToString().StartsWith("ms_"))
						{
							string text = managementObject["Name"].ToString();
							string value = text.Substring(3);
							int culture = Convert.ToInt32(value, 16);
							CultureInfo cultureInfo;
							try
							{
								cultureInfo = new CultureInfo(culture);
								goto IL_C8;
							}
							catch (ArgumentException)
							{
								continue;
							}
							goto IL_BF;
							IL_C8:
							if (cultureInfo.Parent == CultureInfo.InvariantCulture)
							{
								string str = "ms_";
								string text2 = str + cultureInfo.LCID.ToString("X3");
								string text3 = this.InstallPath.PathName;
								text3 = Path.Combine(text3, "Bin");
								text3 = Path.Combine(text3, cultureInfo.Name);
								text3 = Path.Combine(text3, "Exchange.mfl");
								this.ReplaceInFile(text3, text2, text);
								TaskLogger.Log(Strings.ChangeMFLFile(text3, text2, text));
								MofCompiler mofCompiler = new MofCompiler();
								WbemCompileStatusInfo wbemCompileStatusInfo = default(WbemCompileStatusInfo);
								wbemCompileStatusInfo.InitializeStatusInfo();
								int num2 = mofCompiler.CompileFile(text3, null, null, null, null, 0, 0, 0, ref wbemCompileStatusInfo);
								if (num2 != 0)
								{
									TaskLogger.Log(Strings.ErrorCompilingMFL(text3, text, num2));
									goto IL_18E;
								}
								goto IL_18E;
							}
							IL_BF:
							cultureInfo = cultureInfo.Parent;
							goto IL_C8;
						}
						IL_18E:
						num++;
						base.WriteProgress(this.Description, this.Description, num * 100 / count);
					}
				}
			}
			finally
			{
				if (managementObjectSearcher != null)
				{
					managementObjectSearcher.Dispose();
				}
				TaskLogger.Log(Strings.FinishedUpdatingMFLFiles);
				TaskLogger.LogExit();
			}
		}

		private void ReplaceInFile(string filePath, string searchText, string replaceText)
		{
			if (File.Exists(filePath))
			{
				StreamReader streamReader = new StreamReader(filePath);
				string text = streamReader.ReadToEnd();
				streamReader.Close();
				text = Regex.Replace(text, searchText, replaceText);
				StreamWriter streamWriter = new StreamWriter(filePath);
				streamWriter.Write(text);
				streamWriter.Close();
				return;
			}
			TaskLogger.Log(Strings.MFLFileNotFound(filePath));
		}

		private void CopyDirectory(string sourceDirName, string destDirName)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
			if (!directoryInfo.Exists)
			{
				TaskLogger.LogWarning(Strings.WarningLPDirectoryNotFound(sourceDirName));
				return;
			}
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}
			FileInfo[] files = directoryInfo.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				string text = Path.Combine(destDirName, fileInfo.Name);
				if (!File.Exists(text))
				{
					fileInfo.CopyTo(text, true);
				}
			}
			foreach (DirectoryInfo directoryInfo2 in directories)
			{
				string destDirName2 = Path.Combine(destDirName, directoryInfo2.Name);
				this.CopyDirectory(directoryInfo2.FullName, destDirName2);
			}
		}

		private void ParsePerfCounterDefinitionFiles(string definitionFilePath)
		{
			LoadUnloadPerfCounterLocalizedText loadUnloadText = new LoadUnloadPerfCounterLocalizedText();
			foreach (string path in Directory.GetFiles(definitionFilePath, "*.ini"))
			{
				try
				{
					string text = Path.Combine(definitionFilePath, path);
					string driverName = this.GetDriverName(text);
					this.InstallCounterStrings(loadUnloadText, driverName, text);
				}
				catch (TaskException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidData, null);
				}
			}
		}

		private void InstallCounterStrings(LoadUnloadPerfCounterLocalizedText loadUnloadText, string categoryName, string iniFilePath)
		{
			string name = string.Format("SYSTEM\\CurrentControlSet\\Services\\{0}\\Performance", categoryName);
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
			{
				if (registryKey != null)
				{
					try
					{
						loadUnloadText.UnloadLocalizedText(iniFilePath, categoryName);
						loadUnloadText.LoadLocalizedText(iniFilePath, categoryName);
					}
					catch (FileNotFoundException exception)
					{
						base.WriteError(exception, ErrorCategory.InvalidData, null);
					}
					catch (TaskException exception2)
					{
						base.WriteError(exception2, ErrorCategory.InvalidData, null);
					}
				}
			}
		}

		protected override LocalizedString Description
		{
			get
			{
				return Strings.LanguagePackCfgDescription;
			}
		}

		private string GetDriverName(string iniFile)
		{
			string result = string.Empty;
			StringBuilder stringBuilder = new StringBuilder(512);
			if (InstallLanguages.GetPrivateProfileString("info", "drivername", null, stringBuilder, stringBuilder.Capacity, iniFile) != 0)
			{
				result = stringBuilder.ToString();
			}
			return result;
		}

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileStringW")]
		private static extern int GetPrivateProfileString(string applicationName, string keyName, string defaultString, StringBuilder returnedString, int size, string fileName);
	}
}
