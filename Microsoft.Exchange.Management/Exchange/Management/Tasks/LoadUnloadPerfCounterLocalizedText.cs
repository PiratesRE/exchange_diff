using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Management.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LoadUnloadPerfCounterLocalizedText
	{
		internal void LoadLocalizedText(string iniFileName, string categoryName)
		{
			TaskLogger.LogEnter();
			if (!File.Exists(iniFileName))
			{
				throw new FileNotFoundException(iniFileName);
			}
			this.BackupPerfRegistry("install-perf-" + categoryName);
			this.LogInstalledCategoryNames(Path.GetFullPath(iniFileName), "Add");
			int num = this.RunExecutable("lodctr.exe", "\"" + iniFileName + "\"", Path.GetDirectoryName(Path.GetFullPath(iniFileName)), 1200000);
			if (num != 0)
			{
				throw new TaskException(Strings.ExceptionPerfCounterLodctrFailed(num));
			}
			TaskLogger.LogExit();
		}

		internal void UnloadLocalizedText(string iniFileName, string categoryName)
		{
			TaskLogger.LogEnter();
			if (!File.Exists(iniFileName))
			{
				throw new FileNotFoundException(iniFileName);
			}
			this.BackupPerfRegistry("uninstall-perf-" + categoryName);
			this.LogInstalledCategoryNames(Path.GetFullPath(iniFileName), "Rem");
			int num = this.RunExecutable("unlodctr.exe", "\"" + categoryName + "\"", null, 1200000);
			if (num != 0 && num != 1010)
			{
				throw new TaskException(Strings.ExceptionPerfCounterUnlodctrFailed(num));
			}
			TaskLogger.LogExit();
		}

		private void BackupPerfRegistry(string backupFileNamePrefix)
		{
			string text = string.Format("{0}-{1}.bak", backupFileNamePrefix, ExDateTime.Now.ToString("yyyyMMdd-hhmmss.fff"));
			string arguments = string.Format("\"/s:{0}\"", text);
			string text2 = Path.Combine(ConfigurationContext.Setup.LoggingPath, "lodctr_backups");
			Directory.CreateDirectory(text2);
			this.RunExecutable("lodctr.exe", arguments, text2, 1200000);
			string text3 = Path.Combine(text2, text);
			if (File.Exists(text3))
			{
				FileInfo fileInfo = new FileInfo(text3);
				if (fileInfo.Length > 0L)
				{
					DirectoryInfo directoryInfo = new DirectoryInfo(text2);
					string searchPattern = string.Format("{0}-{1}.bak", backupFileNamePrefix, "*");
					FileInfo[] files = directoryInfo.GetFiles(searchPattern);
					if (files.Length > 1)
					{
						foreach (FileInfo fileInfo2 in files)
						{
							if (fileInfo2.Name.CompareTo(text) != 0)
							{
								try
								{
									fileInfo2.Delete();
								}
								catch (Exception ex)
								{
									if (!(ex is IOException) && !(ex is SecurityException))
									{
										throw ex;
									}
									TaskLogger.Log(Strings.LanguagePackPerfCounterDeleteError(fileInfo2.FullName, ex.Message));
								}
							}
						}
					}
				}
			}
		}

		private int RunExecutable(string executableFilename, string arguments, string workingDirectory, int timeout)
		{
			string text;
			string text2;
			return ProcessRunner.Run(executableFilename, arguments, timeout, workingDirectory, out text, out text2);
		}

		private void LogInstalledCategoryNames(string iniFileName, string action_prefix)
		{
			int num = 40000;
			SafeHGlobalHandle safeHGlobalHandle2;
			SafeHGlobalHandle safeHGlobalHandle = safeHGlobalHandle2 = new SafeHGlobalHandle(Marshal.AllocHGlobal(num * 2));
			try
			{
				int privateProfileStringSpecial = LoadUnloadPerfCounterLocalizedText.GetPrivateProfileStringSpecial("objects", null, null, safeHGlobalHandle, num, iniFileName);
				if (privateProfileStringSpecial > 0 && privateProfileStringSpecial < num - 2)
				{
					int num2 = 160000;
					SafeHGlobalHandle safeHGlobalHandle4;
					SafeHGlobalHandle safeHGlobalHandle3 = safeHGlobalHandle4 = new SafeHGlobalHandle(Marshal.AllocHGlobal(num2 * 2));
					try
					{
						int privateProfileStringSpecial2 = LoadUnloadPerfCounterLocalizedText.GetPrivateProfileStringSpecial("text", null, null, safeHGlobalHandle3, num2, iniFileName);
						if (privateProfileStringSpecial2 > 0 && privateProfileStringSpecial2 < num2 - 2)
						{
							StringBuilder stringBuilder = new StringBuilder(10000);
							string path = Path.Combine(ConfigurationContext.Setup.LoggingPath, "lodctr_backups\\InstalledPerfCategories.log");
							IntPtr ptr = safeHGlobalHandle.DangerousGetHandle();
							for (;;)
							{
								string text = Marshal.PtrToStringUni(ptr);
								if (string.IsNullOrEmpty(text))
								{
									break;
								}
								int num3 = text.LastIndexOf('_');
								if (num3 > 0)
								{
									int num4 = text.LastIndexOf('_', num3 - 1, num3);
									if (num4 > 0)
									{
										string strA = text.Substring(0, num4);
										IntPtr ptr2 = safeHGlobalHandle3.DangerousGetHandle();
										for (;;)
										{
											string text2 = Marshal.PtrToStringUni(ptr2);
											if (string.IsNullOrEmpty(text2))
											{
												break;
											}
											int num5 = text2.LastIndexOf('_');
											if (num5 > 0)
											{
												int num6 = text2.LastIndexOf('_', num5 - 1, num5);
												if (num6 > 0)
												{
													string strB = text2.Substring(0, num6);
													if (string.Compare(strA, strB) == 0 && !text2.EndsWith("_HELP") && LoadUnloadPerfCounterLocalizedText.GetPrivateProfileString("text", text2, null, stringBuilder, stringBuilder.Capacity, iniFileName) != 0)
													{
														File.AppendAllText(path, string.Format("{0}-{1}-{2}={3}\r\n", new object[]
														{
															action_prefix,
															Path.GetFileName(iniFileName),
															text2,
															stringBuilder
														}), new UnicodeEncoding());
													}
												}
											}
											ptr2 = (IntPtr)(ptr2.ToInt64() + (long)((text2.Length + 1) * 2));
										}
									}
								}
								ptr = (IntPtr)(ptr.ToInt64() + (long)((text.Length + 1) * 2));
							}
						}
					}
					finally
					{
						if (safeHGlobalHandle4 != null)
						{
							((IDisposable)safeHGlobalHandle4).Dispose();
						}
					}
				}
			}
			finally
			{
				if (safeHGlobalHandle2 != null)
				{
					((IDisposable)safeHGlobalHandle2).Dispose();
				}
			}
		}

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileStringW")]
		protected internal static extern int GetPrivateProfileString(string applicationName, string keyName, string defaultString, StringBuilder returnedString, int size, string fileName);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetPrivateProfileStringW")]
		protected internal static extern int GetPrivateProfileStringSpecial(string applicationName, string keyName, string defaultString, SafeHandle lpBuffer, int size, string fileName);

		private const string LodctrFileName = "lodctr.exe";

		private const string UnodctrFileName = "unlodctr.exe";

		private const string LodctrBackupsSubdir = "lodctr_backups";

		private const string BackupFilePattern = "{0}-{1}.bak";

		private const int lodctrTimeout = 1200000;
	}
}
