using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Replay.IO;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Win32;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay.MountPoint
{
	internal static class MountPointUtil
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.VolumeManagerTracer;
			}
		}

		internal static MountedFolderPath GetVolumePathName(string pathName, out Exception exception)
		{
			MountPointUtil.Tracer.TraceDebug<string>(0L, "GetVolumePathName(): Entering...  [ pathName= {0} ]", pathName);
			int num = pathName.Length + 1;
			StringBuilder stringBuilder = new StringBuilder(num);
			exception = null;
			if (!NativeMethods.GetVolumePathName(pathName, stringBuilder, (uint)num))
			{
				exception = new Win32Exception();
				MountPointUtil.Tracer.TraceError<string, Exception>(0L, "GetVolumePathName() for path '{0}' failed with error: {1}", pathName, exception);
				return null;
			}
			string mountedFolderPath = stringBuilder.ToString();
			MountedFolderPath mountedFolderPath2 = new MountedFolderPath(mountedFolderPath);
			MountPointUtil.Tracer.TraceDebug<string, MountedFolderPath>(0L, "GetVolumePathName() for path '{0}' returning mount point '{1}'", pathName, mountedFolderPath2);
			return mountedFolderPath2;
		}

		internal static MountedFolderPath GetVolumeNameForVolumeMountPoint(MountedFolderPath volumeMountPoint, out Exception exception)
		{
			MountPointUtil.Tracer.TraceDebug<MountedFolderPath>(0L, "GetVolumeNameForVolumeMountPoint(): Entering...  [ volumeMountPoint= {0} ]", volumeMountPoint);
			int num = 50;
			StringBuilder stringBuilder = new StringBuilder(num);
			exception = null;
			if (!NativeMethods.GetVolumeNameForVolumeMountPoint(volumeMountPoint.Path, stringBuilder, (uint)num))
			{
				exception = new Win32Exception();
				MountPointUtil.Tracer.TraceError<MountedFolderPath, Exception>(0L, "GetVolumeNameForVolumeMountPoint() for mount point '{0}' failed with error: {1}", volumeMountPoint, exception);
				return null;
			}
			string mountedFolderPath = stringBuilder.ToString();
			MountedFolderPath mountedFolderPath2 = new MountedFolderPath(mountedFolderPath);
			MountPointUtil.Tracer.TraceDebug<MountedFolderPath, MountedFolderPath>(0L, "GetVolumeNameForVolumeMountPoint() for mount point '{0}' returning volume name '{1}'", volumeMountPoint, mountedFolderPath2);
			return mountedFolderPath2;
		}

		internal static MountedFolderPath[] GetVolumePathNamesForVolumeName(MountedFolderPath volumeName, out Exception exception)
		{
			MountPointUtil.Tracer.TraceDebug<MountedFolderPath>(0L, "GetVolumePathNamesForVolumeName(): Entering...  [ volumeName= {0} ]", volumeName);
			exception = null;
			uint num = 4U;
			uint num2 = 0U;
			string text = new string(new char[num]);
			string[] source = new string[0];
			MountedFolderPath[] result = new MountedFolderPath[0];
			bool volumePathNamesForVolumeName = NativeMethods.GetVolumePathNamesForVolumeName(volumeName.Path, text, num, ref num2);
			if (!volumePathNamesForVolumeName)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 234)
				{
					exception = new Win32Exception(lastWin32Error);
					MountPointUtil.Tracer.TraceError<MountedFolderPath, Exception>(0L, "GetVolumePathNamesForVolumeName() for volumeName '{0}' failed with error: {1}", volumeName, exception);
					return result;
				}
				num = num2;
				text = new string(new char[num]);
				volumePathNamesForVolumeName = NativeMethods.GetVolumePathNamesForVolumeName(volumeName.Path, text, num, ref num2);
			}
			if (!volumePathNamesForVolumeName)
			{
				exception = new Win32Exception();
				MountPointUtil.Tracer.TraceError<MountedFolderPath, Exception>(0L, "GetVolumePathNamesForVolumeName() for volumeName '{0}' failed a second time with error: {1}", volumeName, exception);
				return result;
			}
			string text2 = text;
			char[] separator = new char[1];
			source = text2.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			return (from mp in source
			select new MountedFolderPath(mp)).ToArray<MountedFolderPath>();
		}

		public static Exception GetVolumeInformation(string rootPathName, out StringBuilder volumeNameBuffer, int volumeNameSize)
		{
			MountPointUtil.Tracer.TraceDebug<string>(0L, "GetVolumeInformation(): Entering...  [ rootPathName= {0} ]", rootPathName);
			Exception ex = null;
			volumeNameBuffer = new StringBuilder(volumeNameSize);
			int num = 0;
			int num2 = 0;
			uint num3 = 0U;
			if (!NativeMethods.GetVolumeInformation(rootPathName, volumeNameBuffer, volumeNameSize, ref num, ref num2, ref num3, null, 0))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				ex = new Win32Exception(lastWin32Error);
				MountPointUtil.Tracer.TraceError<string, Exception>(0L, "GetVolumeInformation() for rootPathName '{0}' failed with error: {1}", rootPathName, ex);
			}
			return ex;
		}

		public static string GetVolumeLabel(MountedFolderPath volumeName, out Exception exception)
		{
			MountPointUtil.Tracer.TraceDebug<string>(0L, "GetVolumeLabel(): Entering...  [ volumeName= {0} ]", volumeName.Path);
			exception = null;
			int volumeNameSize = 1024;
			StringBuilder stringBuilder;
			exception = MountPointUtil.GetVolumeInformation(volumeName.Path, out stringBuilder, volumeNameSize);
			if (exception != null)
			{
				MountPointUtil.Tracer.TraceError<string, Exception>(0L, "GetVolumeLabel() for volumeName '{0}' failed with error: {1}", volumeName.Path, exception);
				return null;
			}
			return stringBuilder.ToString();
		}

		internal static bool IsConfigureMountPointsEnabled()
		{
			int propertyValue = -1;
			Exception ex = RegistryUtil.RunRegistryFunction(delegate()
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\AutoReseed"))
				{
					object value = registryKey.GetValue("ConfigureMountPointsPostReInstall");
					propertyValue = ((value == null) ? 0 : ((int)value));
				}
			});
			if (ex != null)
			{
				MountPointUtil.Tracer.TraceError<string, string>(0L, "IsConfigureMountPointsEnabled() Unable to read registry key '{0}'. Error {1}", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\AutoReseed", AmExceptionHelper.GetExceptionMessageOrNoneString(ex));
			}
			if (propertyValue == 0)
			{
				MountPointUtil.Tracer.TraceDebug<string, int>(0L, "IsConfigureMountPointsEnabled() key {0} doesn't exist or is set to {1}. Returning false.", "ConfigureMountPointsPostReInstall", propertyValue);
				return false;
			}
			MountPointUtil.Tracer.TraceDebug<string, int>(0L, "IsConfigureMountPointsEnabled() key {0} exists and is set to {1}. Returning true.", "ConfigureMountPointsPostReInstall", propertyValue);
			return true;
		}

		internal static Exception DisableConfigureMountPoints()
		{
			MountPointUtil.Tracer.TraceDebug<string>(0L, "DisableConfigureMountPoints() Attempting to reset regkey {0} to 0. This will disable post reinstall mountpoint configuration.", "ConfigureMountPointsPostReInstall");
			Exception ex = RegistryUtil.RunRegistryFunction(delegate()
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\AutoReseed"))
				{
					registryKey.SetValue("ConfigureMountPointsPostReInstall", 0, RegistryValueKind.DWord);
				}
			});
			if (ex != null)
			{
				MountPointUtil.Tracer.TraceError<string, string>(0L, "IsConfigureMountPointsEnabled() Unable to set registry key '{0}'. Error {1}", "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\AutoReseed", AmExceptionHelper.GetExceptionMessageOrNoneString(ex));
			}
			return ex;
		}

		internal static bool IsDirectoryMountPoint(string path, out Exception exception)
		{
			exception = null;
			DirectoryInfo di = null;
			exception = MountPointUtil.HandleIOExceptions(delegate
			{
				di = new DirectoryInfo(path);
			});
			if (exception != null)
			{
				return false;
			}
			if (!di.Exists)
			{
				return false;
			}
			if (!BitMasker.IsOn((int)di.Attributes, 1024))
			{
				return false;
			}
			if (di.Parent == null)
			{
				return false;
			}
			using (DirectoryEnumerator dirEnum = new DirectoryEnumerator(di.Parent, false, false))
			{
				NativeMethods.WIN32_FIND_DATA findData = default(NativeMethods.WIN32_FIND_DATA);
				bool foundNext = false;
				string dirName;
				exception = MountPointUtil.HandleIOExceptions(delegate
				{
					foundNext = dirEnum.GetNextDirectoryExtendedInfo(di.Name, out dirName, out findData);
				});
				if (exception != null)
				{
					return false;
				}
				if (!foundNext)
				{
					return false;
				}
				if (findData.Reserved0 == 2684354563U)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsDirectoryAccessibleMountPoint(string path, out Exception exception)
		{
			if (!MountPointUtil.IsDirectoryMountPoint(path, out exception))
			{
				return false;
			}
			exception = null;
			try
			{
				Directory.GetDirectories(path);
			}
			catch (IOException ex)
			{
				exception = ex;
			}
			catch (UnauthorizedAccessException ex2)
			{
				exception = ex2;
			}
			catch (SecurityException ex3)
			{
				exception = ex3;
			}
			return exception == null;
		}

		public static Exception DeleteVolumeMountPoint(MountedFolderPath volumeMountPoint)
		{
			MountPointUtil.Tracer.TraceDebug<MountedFolderPath>(0L, "DeleteVolumeMountPoint(): Entering...  [ volumeMountPoint= {0} ]", volumeMountPoint);
			if (!NativeMethods.DeleteVolumeMountPoint(volumeMountPoint.Path))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 4390)
				{
					Exception ex = new Win32Exception(lastWin32Error);
					MountPointUtil.Tracer.TraceError<MountedFolderPath, Exception>(0L, "DeleteVolumeMountPoint() for path '{0}' failed with error: {1}", volumeMountPoint, ex);
					return ex;
				}
				MountPointUtil.Tracer.TraceDebug<MountedFolderPath, int>(0L, "DeleteVolumeMountPoint() for path '{0}' returned ERROR_NOT_A_REPARSE_POINT ({1}).", volumeMountPoint, lastWin32Error);
			}
			MountPointUtil.Tracer.TraceDebug<MountedFolderPath>(0L, "DeleteVolumeMountPoint(): Deleted mount point at '{0}'", volumeMountPoint);
			return null;
		}

		public static Exception SetVolumeMountPoint(MountedFolderPath volumeMountPoint, MountedFolderPath volumeName)
		{
			MountPointUtil.Tracer.TraceDebug<MountedFolderPath, MountedFolderPath>(0L, "SetVolumeMountPoint(): Entering...  [ volumeMountPoint= {0}, volumeName= {1} ]", volumeMountPoint, volumeName);
			if (!NativeMethods.SetVolumeMountPoint(volumeMountPoint.Path, volumeName.Path))
			{
				Exception ex = new Win32Exception();
				MountPointUtil.Tracer.TraceError<MountedFolderPath, Exception>(0L, "SetVolumeMountPoint() for path '{0}' failed with error: {1}", volumeMountPoint, ex);
				return ex;
			}
			MountPointUtil.Tracer.TraceDebug<MountedFolderPath, MountedFolderPath>(0L, "SetVolumeMountPoint(): Associated mount point '{0}' with volume {1}", volumeMountPoint, volumeName);
			return null;
		}

		public static Exception SetVolumeLabel(MountedFolderPath volumeMountPoint, string volumeLabel)
		{
			MountPointUtil.Tracer.TraceDebug<MountedFolderPath, string>(0L, "SetVolumeLabel(): Entering...  [ volumeMountPoint= {0}, volumeLabel= {1} ]", volumeMountPoint, volumeLabel);
			if (!NativeMethods.SetVolumeLabel(volumeMountPoint.Path, volumeLabel))
			{
				Exception ex = new Win32Exception();
				MountPointUtil.Tracer.TraceError<MountedFolderPath, Exception>(0L, "SetVolumeLabel() for path '{0}' failed with error: {1}", volumeMountPoint, ex);
				return ex;
			}
			MountPointUtil.Tracer.TraceDebug<MountedFolderPath, string>(0L, "SetVolumeLabel(): Associated volume '{0}' with label {1}", volumeMountPoint, volumeLabel);
			return null;
		}

		internal static bool IsPathDirectlyUnderParentPath(string path, string parentPath, out Exception exception)
		{
			exception = null;
			DirectoryInfo di = null;
			exception = MountPointUtil.HandleIOExceptions(delegate
			{
				di = new DirectoryInfo(path);
			});
			if (exception != null)
			{
				MountPointUtil.Tracer.TraceError<string, Exception>(0L, "IsPathDirectlyUnderParentPath() : DirectoryInfo..ctor() on path '{0}' failed with exception: {1}", path, exception);
				return false;
			}
			if (di.Parent == null)
			{
				MountPointUtil.Tracer.TraceDebug<string>(0L, "IsPathDirectlyUnderParentPath() : Path '{0}' is a root path. Returning false.", path);
				return false;
			}
			return string.Equals(parentPath, di.Parent.FullName, StringComparison.OrdinalIgnoreCase);
		}

		internal static DateTime GetLastWriteTimeUtcInDirectory(string directory, bool recurse, out string lastWriteTimePath, out Exception exception)
		{
			DateTime dateTime = DateTime.MaxValue;
			lastWriteTimePath = string.Empty;
			DirectoryInfo di = null;
			exception = MountPointUtil.HandleIOExceptions(delegate
			{
				di = new DirectoryInfo(directory);
			});
			if (exception != null)
			{
				MountPointUtil.Tracer.TraceError<string, Exception>(0L, "GetLastWriteTimeUtcOfFilesInDirectory(): Failed to construct DirectoryInfo for directory '{0}'. Exception: {1}", directory, exception);
				return dateTime;
			}
			if (!di.Exists)
			{
				MountPointUtil.Tracer.TraceError<string>(0L, "GetLastWriteTimeUtcOfFilesInDirectory(): Directory '{0}' is not present.", directory);
				return dateTime;
			}
			string maxTimePath = string.Empty;
			DateTime maxTime = DateTime.MinValue;
			using (DirectoryEnumerator dirEnum = new DirectoryEnumerator(di, recurse, false))
			{
				Exception tempEx = null;
				DateTime currentTime;
				exception = MountPointUtil.HandleIOExceptions(delegate
				{
					IEnumerable<string> enumerable = dirEnum.EnumerateFilesAndDirectoriesExcludingHiddenAndSystem("*");
					foreach (string text in enumerable)
					{
						NativeMethods.WIN32_FILE_ATTRIBUTE_DATA fileAttributesEx = FileOperations.GetFileAttributesEx(text, out tempEx);
						if (tempEx != null)
						{
							MountPointUtil.Tracer.TraceError<string, Exception>(0L, "GetLastWriteTimeUtcOfFilesInDirectory(): GetFileAttributesEx() FAILED for '{0}'. Exception: {1}", text, tempEx);
							break;
						}
						currentTime = DateTimeHelper.FromFileTimeUtc(fileAttributesEx.LastWriteTime);
						if (currentTime > maxTime)
						{
							maxTime = currentTime;
							maxTimePath = text;
						}
					}
				});
				if (exception == null)
				{
					exception = tempEx;
				}
			}
			if (exception != null)
			{
				MountPointUtil.Tracer.TraceError<string, Exception>(0L, "GetLastWriteTimeUtcOfFilesInDirectory(): FAILED for directory '{0}'. Exception: {1}", directory, exception);
				return dateTime;
			}
			if (maxTime > DateTime.MinValue)
			{
				dateTime = maxTime;
				lastWriteTimePath = maxTimePath;
			}
			MountPointUtil.Tracer.TraceDebug<string, DateTime, string>(0L, "GetLastWriteTimeUtcOfFilesInDirectory() for directory '{0}' returning '{1}' for file '{2}'", directory, dateTime, lastWriteTimePath);
			return dateTime;
		}

		internal static bool IsDirectoryNonExistentOrEmpty(string directory, out Exception exception)
		{
			DirectoryInfo di = null;
			exception = MountPointUtil.HandleIOExceptions(delegate
			{
				di = new DirectoryInfo(directory);
			});
			if (exception != null)
			{
				MountPointUtil.Tracer.TraceError<string, Exception>(0L, "IsDirectoryNonExistentOrEmpty(): Failed to construct DirectoryInfo for directory '{0}'. Exception: {1}", directory, exception);
				return false;
			}
			if (!di.Exists)
			{
				MountPointUtil.Tracer.TraceDebug<string>(0L, "IsDirectoryNonExistentOrEmpty(): Directory '{0}' is not present. Returning true.", directory);
				return true;
			}
			bool result;
			using (DirectoryEnumerator dirEnum = new DirectoryEnumerator(di, false, false))
			{
				int count = 0;
				exception = MountPointUtil.HandleIOExceptions(delegate
				{
					count = dirEnum.EnumerateFilesAndDirectoriesExcludingHiddenAndSystem("*").Count<string>();
				});
				if (exception != null)
				{
					MountPointUtil.Tracer.TraceError<string, Exception>(0L, "IsDirectoryNonExistentOrEmpty(): Failed to enumerate files/directories for directory '{0}'. Exception: {1}", directory, exception);
					result = false;
				}
				else if (count > 0)
				{
					MountPointUtil.Tracer.TraceDebug<string, int>(0L, "IsDirectoryNonExistentOrEmpty(): Directory '{0}' has {1} files and/or directories. Returning false.", directory, count);
					result = false;
				}
				else
				{
					MountPointUtil.Tracer.TraceDebug<string, int>(0L, "IsDirectoryNonExistentOrEmpty(): Directory '{0}' has 0 files and/or directories. Returning true.", directory, count);
					result = true;
				}
			}
			return result;
		}

		internal static string EnsurePathHasTrailingBackSlash(string path)
		{
			string result = path;
			if (!path.EndsWith("\\"))
			{
				MountPointUtil.Tracer.TraceDebug<string>(0L, "EnsurePathHasTrailingBackSlash() : Path '{0}' does not have a trailing backslash. Appending it now.", path);
				result = path + "\\";
			}
			return result;
		}

		internal static NativeMethods.WIN32_FIND_DATA GetExtendedNonRootDirectoryInfo(string path, out Exception exception)
		{
			exception = null;
			NativeMethods.WIN32_FIND_DATA win32_FIND_DATA = default(NativeMethods.WIN32_FIND_DATA);
			DirectoryInfo di = null;
			exception = MountPointUtil.HandleIOExceptions(delegate
			{
				di = new DirectoryInfo(path);
			});
			if (exception != null)
			{
				return win32_FIND_DATA;
			}
			if (!di.Exists)
			{
				exception = new DirectoryNotFoundException();
				return win32_FIND_DATA;
			}
			if (di.Parent == null)
			{
				DiagCore.RetailAssert(false, "An invalid root path was passed in: {0}", new object[]
				{
					path
				});
				exception = new IOException(string.Format("An invalid root path was passed in: {0}", path));
				return win32_FIND_DATA;
			}
			NativeMethods.WIN32_FIND_DATA result;
			using (DirectoryEnumerator dirEnum = new DirectoryEnumerator(di.Parent, false, false))
			{
				bool foundNext = false;
				NativeMethods.WIN32_FIND_DATA tempFindData = default(NativeMethods.WIN32_FIND_DATA);
				string dirName;
				exception = MountPointUtil.HandleIOExceptions(delegate
				{
					foundNext = dirEnum.GetNextDirectoryExtendedInfo(di.Name, out dirName, out tempFindData);
				});
				if (exception != null)
				{
					result = win32_FIND_DATA;
				}
				else if (!foundNext)
				{
					exception = new DirectoryNotFoundException();
					result = win32_FIND_DATA;
				}
				else
				{
					exception = null;
					result = tempFindData;
				}
			}
			return result;
		}

		internal static bool DoesContainAnyFiles(string directory, bool recurse, out Exception exception)
		{
			bool foundFiles = false;
			exception = MountPointUtil.HandleIOExceptions(delegate
			{
				DirectoryInfo path = new DirectoryInfo(directory);
				using (DirectoryEnumerator directoryEnumerator = new DirectoryEnumerator(path, recurse, false))
				{
					IEnumerable<string> source = directoryEnumerator.EnumerateFiles("*", DirectoryEnumerator.ExcludeHiddenAndSystemFilter);
					if (source.Any<string>())
					{
						foundFiles = true;
					}
				}
			});
			return foundFiles;
		}

		internal static Exception HandleIOExceptions(Action operation)
		{
			Exception result = null;
			try
			{
				operation();
			}
			catch (IOException ex)
			{
				result = ex;
			}
			catch (SecurityException ex2)
			{
				result = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				result = ex3;
			}
			return result;
		}

		internal static IEnumerable<string> SortDatabaseNames(this IEnumerable<string> dbNames)
		{
			return dbNames.OrderBy((string dbName) => dbName, StringComparer.InvariantCultureIgnoreCase);
		}

		private const string TrailingBackSlash = "\\";

		private const string MountPointConfigurationKeyRoot = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\AutoReseed";

		internal const string MountPointConfigurationKeyName = "ConfigureMountPointsPostReInstall";
	}
}
