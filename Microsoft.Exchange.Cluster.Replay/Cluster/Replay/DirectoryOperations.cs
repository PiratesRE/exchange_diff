using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Common.Bitlocker.Utilities;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class DirectoryOperations
	{
		public static bool IsPathOnLockedVolume(string directory)
		{
			Exception ex = null;
			if (BitlockerUtil.IsFilePathOnLockedVolume(directory, out ex))
			{
				return true;
			}
			if (ex != null)
			{
				string text = string.Format("IsPathOnLockedVolume({0}) failed: {1}", directory, ex.Message);
				ReplayCrimsonEvents.BitlockerQueryFailed.LogPeriodic<string, Exception>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, text, ex);
			}
			return false;
		}

		public static Exception TryCreateDirectory(string directory)
		{
			Exception ex = null;
			try
			{
				if (Directory.Exists(directory))
				{
					ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>(0L, "TryCreateDirectory(): Directory '{0}' already exists.", directory);
					return null;
				}
				Directory.CreateDirectory(directory, ObjectSecurity.ExchangeFolderSecurity);
				ExTraceGlobals.ReplicaInstanceTracer.TraceDebug<string>(0L, "TryCreateDirectory(): Directory '{0}' successfully created.", directory);
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			catch (SecurityException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, Exception>(0L, "TryCreateDirectory(): Creating directory '{0}' failed with exception: {1}", directory, ex);
			}
			return ex;
		}

		public static Exception TryDeleteDirectoryRecursively(DirectoryInfo directoryInfo)
		{
			Exception ex = null;
			try
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string>(0L, "TryDeleteDirectoryRecursively(): Deleting directory '{0}' ...", directoryInfo.FullName);
				if (directoryInfo.Exists)
				{
					directoryInfo.Delete(true);
				}
			}
			catch (IOException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.ReplicaInstanceTracer.TraceError<string, Exception>(0L, "TryDeleteDirectoryRecursively(): Deleting directory '{0}' failed with exception: {1}", directoryInfo.FullName, ex);
			}
			return ex;
		}

		public static void ProbeDirectory(string path)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(path);
			IEnumerable<FileInfo> source = directoryInfo.EnumerateFiles();
			source.FirstOrDefault<FileInfo>();
		}
	}
}
