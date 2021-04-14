using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics.Components.Setup;
using Microsoft.Exchange.Setup.AcquireLanguagePack;
using Microsoft.Exchange.Setup.Bootstrapper.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Setup.CommonBase
{
	internal static class SetupHelper
	{
		public static SortedList<string, string> FilesToCopy { get; set; }

		public static string WindowsDir
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.Windows);
			}
		}

		public static bool IsSourceRemote
		{
			get
			{
				int driveType = SetupHelper.GetDriveType(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
				return driveType != 3 || driveType != 5;
			}
		}

		internal static bool IsDatacenter { get; set; }

		internal static bool TreatPreReqErrorsAsWarnings { get; set; }

		public static void CopyFiles(string srcDir, string dstDir, bool overwrite)
		{
			SetupHelper.CopyFiles(srcDir, dstDir, overwrite, null);
		}

		public static void CopyFiles(string srcDir, string dstDir, bool overwrite, IList<string> exceptionList)
		{
			SetupHelper.CopyFiles(srcDir, dstDir, overwrite, true, null, exceptionList);
		}

		public static void CopyFiles(string srcDir, string dstDir, bool overwrite, bool recursive, string fileNamePattern)
		{
			SetupHelper.CopyFiles(srcDir, dstDir, overwrite, recursive, fileNamePattern, null);
		}

		public static void CopyFiles(string srcDir, string dstDir, bool overwrite, bool recursive, string fileNamePattern, IList<string> exceptionList)
		{
			try
			{
				SetupHelper.CheckDiskSpace(srcDir, dstDir, overwrite, recursive, fileNamePattern, exceptionList);
				if (!Directory.Exists(dstDir))
				{
					Directory.CreateDirectory(dstDir);
				}
				foreach (KeyValuePair<string, string> keyValuePair in SetupHelper.FilesToCopy)
				{
					FileInfo fileInfo = new FileInfo(keyValuePair.Value);
					if (!Directory.Exists(fileInfo.DirectoryName))
					{
						Directory.CreateDirectory(fileInfo.DirectoryName);
					}
					try
					{
						File.Copy(keyValuePair.Key, keyValuePair.Value, overwrite);
					}
					catch (Exception ex)
					{
						if (ex is IOException || ex is ArgumentException || ex is NotSupportedException)
						{
							throw new FileCopyException(keyValuePair.Key, keyValuePair.Value);
						}
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
				throw new FileCopyException(srcDir, dstDir);
			}
		}

		public static void CopyFiles(IEnumerable<string> fileList, string srcDir, string dstDir, bool ignoreIfNotExist, bool overwrite)
		{
			if (fileList == null || fileList.Count<string>() == 0)
			{
				return;
			}
			try
			{
				SetupHelper.CheckDiskSpace(srcDir, ignoreIfNotExist);
				Directory.CreateDirectory(dstDir);
				foreach (string path in fileList)
				{
					string text = Path.Combine(srcDir, path);
					if (!File.Exists(text))
					{
						if (!ignoreIfNotExist)
						{
							throw new FileNotExistsException(text);
						}
					}
					else
					{
						string text2 = Path.Combine(dstDir, path);
						string directoryName = Path.GetDirectoryName(text2);
						if (!Directory.Exists(directoryName))
						{
							Directory.CreateDirectory(directoryName);
						}
						if (!overwrite)
						{
							if (File.Exists(text2))
							{
								continue;
							}
						}
						try
						{
							File.Copy(text, text2, overwrite);
						}
						catch (Exception ex)
						{
							if (ex is IOException || ex is ArgumentException || ex is NotSupportedException)
							{
								throw new FileCopyException(text, text2);
							}
						}
					}
				}
			}
			catch (UnauthorizedAccessException)
			{
				throw new FileCopyException(srcDir, dstDir);
			}
		}

		public static void DeleteDirectory(string path)
		{
			string[] files = Directory.GetFiles(path);
			string[] directories = Directory.GetDirectories(path);
			foreach (string path2 in files)
			{
				if (File.Exists(path2))
				{
					File.SetAttributes(path2, FileAttributes.Normal);
					File.Delete(path2);
				}
			}
			foreach (string path3 in directories)
			{
				SetupHelper.DeleteDirectory(path3);
			}
			if (Directory.Exists(path))
			{
				Directory.Delete(path, false);
			}
		}

		public static bool ResumeUpgrade()
		{
			bool result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(Path.Combine(SetupChecksRegistryConstant.RegistryExchangePath, "Setup-save")))
			{
				result = (registryKey != null);
			}
			return result;
		}

		public static bool MoveFiles(string srcDir, string dstDir)
		{
			if (!Directory.Exists(dstDir))
			{
				Directory.CreateDirectory(dstDir);
			}
			string[] files = SetupHelper.GetFiles(srcDir);
			foreach (string text in files)
			{
				if (Directory.Exists(text) && !SetupChecksFileConstant.GetExcludedPaths().Contains(text))
				{
					SetupHelper.MoveFiles(text, Path.Combine(dstDir, Path.GetFileName(text)));
				}
				else
				{
					string text2 = Path.Combine(dstDir, Path.GetFileName(text));
					if (File.Exists(text2))
					{
						File.Delete(text2);
					}
					File.Move(text, text2);
				}
			}
			return true;
		}

		public static IList<string> GetSetupRequiredFilesFromAssembly(string dirToSetupAssembly)
		{
			if (string.IsNullOrEmpty(dirToSetupAssembly))
			{
				throw new ArgumentNullException("dirToSetupAssembly");
			}
			if (!Directory.Exists(dirToSetupAssembly))
			{
				return null;
			}
			string path = Path.Combine(dirToSetupAssembly, "Microsoft.Exchange.Setup.Bootstrapper.Common.dll");
			if (!File.Exists(path))
			{
				return null;
			}
			byte[] rawAssembly = File.ReadAllBytes(path);
			Assembly assembly = Assembly.Load(rawAssembly);
			string typeFullName = typeof(SetupChecksFileConstant).FullName;
			Type type = assembly.GetTypes().FirstOrDefault((Type x) => x.FullName.Equals(typeFullName));
			if (type == null)
			{
				throw new ArgumentNullException(typeFullName);
			}
			MethodInfo methodInfo = type.GetMethods(BindingFlags.Static | BindingFlags.Public).FirstOrDefault((MethodInfo x) => x.Name.Equals("GetSetupRequiredFiles"));
			if (methodInfo == null)
			{
				throw new MissingMethodException("GetSetupRequiredFiles");
			}
			return (IList<string>)methodInfo.Invoke(type, null);
		}

		public static string GetVersionOfApplication(string fullFileName)
		{
			string result = string.Empty;
			if (File.Exists(fullFileName))
			{
				FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fullFileName);
				if (!string.IsNullOrEmpty(versionInfo.FileVersion))
				{
					result = versionInfo.FileVersion;
				}
			}
			return result;
		}

		public static void TryFindUpdates(string dirToCheck, out string mspFileName, out string lpFileName)
		{
			mspFileName = null;
			lpFileName = null;
			if (!string.IsNullOrEmpty(dirToCheck) && Directory.Exists(dirToCheck))
			{
				IEnumerable<string> source = Directory.EnumerateFiles(dirToCheck, "*", SearchOption.TopDirectoryOnly);
				string text = source.FirstOrDefault((string x) => !string.IsNullOrEmpty(x) && MsiHelper.IsMspFileExtension(x));
				if (!string.IsNullOrEmpty(text))
				{
					mspFileName = text;
				}
				string text2 = source.FirstOrDefault((string x) => x.EndsWith("LanguagePackBundle.exe", StringComparison.InvariantCultureIgnoreCase));
				if (!string.IsNullOrEmpty(text2))
				{
					lpFileName = text2;
				}
			}
		}

		public static void ValidOSVersion()
		{
			OperatingSystem osversion = Environment.OSVersion;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<OperatingSystem>(2160471357U, ref osversion);
			int num = 0;
			if (!string.IsNullOrEmpty(osversion.ServicePack))
			{
				MatchCollection matchCollection = Regex.Matches(osversion.ServicePack, "(\\d+\\.?\\d*|\\.\\d+)");
				if (matchCollection.Count == 1)
				{
					int.TryParse(matchCollection[0].Value, out num);
				}
			}
			if ((osversion.Version.Major != 6 || osversion.Version.Minor != 1 || num < 1) && (osversion.Version.Major != 6 || osversion.Version.Minor <= 1) && osversion.Version.Major <= 6)
			{
				throw new InvalidOSVersionException();
			}
			if (!Environment.Is64BitProcess || !Environment.Is64BitOperatingSystem)
			{
				throw new Bit64OnlyException();
			}
		}

		public static bool ValidDotNetFrameworkInstalled()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full"))
			{
				try
				{
					if (registryKey == null)
					{
						return false;
					}
					string text = (string)registryKey.GetValue("version");
					if (string.IsNullOrEmpty(text))
					{
						return false;
					}
					Version v = new Version(text);
					ExTraceGlobals.FaultInjectionTracer.TraceTest<Version>(4144377149U, ref v);
					if (v < SetupChecksRegistryConstant.DotNetVersion)
					{
						return false;
					}
				}
				catch (Exception ex)
				{
					if (ex is TypeInitializationException || ex is FileLoadException)
					{
						return false;
					}
					throw;
				}
			}
			return true;
		}

		public static void ValidPowershellInstalled()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\PowerShell\\3\\PowerShellEngine"))
			{
				if (registryKey == null)
				{
					throw new InvalidPSVersionException();
				}
				string text = (string)registryKey.GetValue("PowerShellVersion");
				if (string.IsNullOrEmpty(text))
				{
					throw new InvalidPSVersionException();
				}
				Version v = new Version(text);
				ExTraceGlobals.FaultInjectionTracer.TraceTest<Version>(2533764413U, ref v);
				if (v < SetupChecksRegistryConstant.PowershellVersion)
				{
					throw new InvalidPSVersionException();
				}
			}
		}

		public static bool IsClientVersionOS()
		{
			SetupHelper.OSVERSIONINFOEX osversioninfoex = default(SetupHelper.OSVERSIONINFOEX);
			osversioninfoex.VersionInfoSize = Marshal.SizeOf(typeof(SetupHelper.OSVERSIONINFOEX));
			return SetupHelper.GetVersionEx(ref osversioninfoex) && osversioninfoex.ProductType == 1;
		}

		internal static void GetDatacenterSettings()
		{
			SetupHelper.IsDatacenter = false;
			SetupHelper.TreatPreReqErrorsAsWarnings = false;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs"))
			{
				if (registryKey != null)
				{
					int num = (int)registryKey.GetValue("DatacenterMode", 0);
					if (num != 0)
					{
						SetupHelper.IsDatacenter = true;
					}
					num = (int)registryKey.GetValue("TreatPreReqErrorsAsWarnings", 0);
					if (num != 0)
					{
						SetupHelper.TreatPreReqErrorsAsWarnings = true;
					}
				}
			}
		}

		internal static long FindTotalSize(string srcDir, bool ignoreIfNotExist)
		{
			long num = 0L;
			foreach (string path in SetupChecksFileConstant.GetSetupRequiredFiles())
			{
				string text = Path.Combine(srcDir, path);
				if (!File.Exists(text))
				{
					if (!ignoreIfNotExist)
					{
						return 0L;
					}
				}
				else
				{
					FileInfo fileInfo = new FileInfo(text);
					num += fileInfo.Length;
				}
			}
			return num;
		}

		internal static long FindTotalSize(string srcDir, string dstDir, bool overwrite, bool recursive, string fileNamePattern, IList<string> exceptionList, long totalDirSize)
		{
			string[] fileSystemEntries = Directory.GetFileSystemEntries(srcDir);
			string[] array = fileSystemEntries;
			for (int i = 0; i < array.Length; i++)
			{
				string file = array[i];
				if (Directory.Exists(file))
				{
					if (recursive)
					{
						string fileCompare = file.ToLower() + "\\";
						if (string.IsNullOrEmpty(SetupChecksFileConstant.GetExcludedPaths().FirstOrDefault((string paths) => fileCompare.IndexOf(paths) > 0)))
						{
							totalDirSize = SetupHelper.FindTotalSize(file, Path.Combine(dstDir, Path.GetFileName(file)), overwrite, recursive, fileNamePattern, exceptionList, totalDirSize);
						}
					}
				}
				else
				{
					if (exceptionList != null)
					{
						if (exceptionList.Any((string x) => file.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0))
						{
							goto IL_164;
						}
					}
					if (string.IsNullOrEmpty(fileNamePattern) || Regex.IsMatch(Path.GetFileName(file), fileNamePattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
					{
						FileInfo fileInfo = new FileInfo(file);
						string text = Path.Combine(dstDir, Path.GetFileName(file));
						if (File.Exists(text))
						{
							if (!overwrite)
							{
								goto IL_164;
							}
							totalDirSize += fileInfo.Length;
							FileInfo fileInfo2 = new FileInfo(text);
							totalDirSize -= fileInfo2.Length;
						}
						else
						{
							totalDirSize += fileInfo.Length;
						}
						SetupHelper.FilesToCopy.Add(file, text);
					}
				}
				IL_164:;
			}
			return totalDirSize;
		}

		private static string[] GetFiles(string path)
		{
			if (!Directory.Exists(path))
			{
				return null;
			}
			return Directory.GetFileSystemEntries(path);
		}

		private static void CheckDiskSpace(string srcDir, string dstDir, bool overwrite, bool recursive, string fileNamePattern, IList<string> exceptionList)
		{
			if (!Directory.Exists(srcDir))
			{
				throw new DirectoryNotExistsException(srcDir);
			}
			SetupHelper.FilesToCopy = new SortedList<string, string>();
			long num = SetupHelper.FindTotalSize(srcDir, dstDir, overwrite, recursive, fileNamePattern, exceptionList, 0L);
			DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(SetupHelper.WindowsDir));
			if (num > 0L && driveInfo.AvailableFreeSpace < num)
			{
				throw new InsufficientDiskSpaceException();
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3607506237U);
		}

		private static void CheckDiskSpace(string srcDir, bool ignoreIfNotExist)
		{
			if (!Directory.Exists(srcDir))
			{
				throw new DirectoryNotExistsException(srcDir);
			}
			long num = SetupHelper.FindTotalSize(srcDir, ignoreIfNotExist);
			DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(SetupHelper.WindowsDir));
			if (num > 0L && driveInfo.AvailableFreeSpace < num)
			{
				throw new InsufficientDiskSpaceException();
			}
		}

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern int GetDriveType(string lpRootPathName);

		[DllImport("kernel32.Dll", SetLastError = true)]
		private static extern bool GetVersionEx(ref SetupHelper.OSVERSIONINFOEX osVersionInfo);

		private const byte VERNTWORKSTATION = 1;

		private struct OSVERSIONINFOEX
		{
			public int VersionInfoSize;

			public int MajorVersion;

			public int MinorVersion;

			public int BuildNumber;

			public int PlatformId;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string ServicePackVersion;

			public short ServicePackMajor;

			public short ServicePackMinor;

			public short SuiteMask;

			public byte ProductType;

			public byte Reserved;
		}
	}
}
