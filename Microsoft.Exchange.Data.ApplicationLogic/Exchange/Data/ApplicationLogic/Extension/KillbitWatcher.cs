using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	public class KillbitWatcher : DisposeTrackableBase
	{
		private static void OnError(object source, ErrorEventArgs e)
		{
			Exception ex = (e != null) ? e.GetException() : null;
			if (ex != null)
			{
				KillbitWatcher.HandleFileWatcherException(ex);
			}
		}

		private static void HandleFileWatcherException(Exception e)
		{
			KillbitWatcher.Tracer.TraceError<string, Exception>(0L, "killbit file system watcher failed because of exception {0}.", KillBitHelper.KillBitDirectory, e);
			ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_KillbitFileWatcherFailed, null, new object[]
			{
				"ProcessKillBit",
				ExtensionDiagnostics.GetLoggedExceptionString(e)
			});
			Win32Exception ex = e as Win32Exception;
			if (ex != null && ex.NativeErrorCode == 5)
			{
				return;
			}
			ExWatson.HandleException(new UnhandledExceptionEventArgs(e, false), ReportOptions.None);
		}

		private static void Watch(KillbitWatcher.ReadKillBitFromFileCallback readKillBitFromFileCallback)
		{
			KillbitWatcher.watcher = new FileSystemWatcher();
			KillbitWatcher.watcher.Error += KillbitWatcher.OnError;
			KillbitWatcher.watcher.Path = KillBitHelper.KillBitDirectory;
			KillbitWatcher.watcher.Filter = "killbit.xml";
			KillbitWatcher.watcher.NotifyFilter = NotifyFilters.LastWrite;
			KillbitWatcher.watcher.Changed += readKillBitFromFileCallback.Invoke;
			KillbitWatcher.watcher.Created += readKillBitFromFileCallback.Invoke;
			KillbitWatcher.watcher.EnableRaisingEvents = true;
		}

		public static void TryWatch(KillbitWatcher.ReadKillBitFromFileCallback readKillBitFromFileCallback)
		{
			if (!Directory.Exists(KillBitHelper.KillBitDirectory))
			{
				Exception ex = null;
				try
				{
					KillbitWatcher.Tracer.TraceInformation<string>(0, 0L, "Killbit folder {0} is not there. Creating it.", KillBitHelper.KillBitDirectory);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_KillbitFolderNotExist, null, new object[]
					{
						"ProcessKillBit"
					});
					Directory.CreateDirectory(KillBitHelper.KillBitDirectory);
				}
				catch (DirectoryNotFoundException ex2)
				{
					ex = ex2;
				}
				catch (IOException ex3)
				{
					ex = ex3;
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex = ex4;
				}
				if (ex != null)
				{
					KillbitWatcher.Tracer.TraceError<string, Exception>(0L, "Cannot created killbit folder {0} due to Exception {1}.", KillBitHelper.KillBitDirectory, ex);
					ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_CanNotCreateKillbitFolder, null, new object[]
					{
						"ProcessKillBit",
						ExtensionDiagnostics.GetLoggedExceptionString(ex)
					});
					return;
				}
			}
			try
			{
				KillbitWatcher.Watch(readKillBitFromFileCallback);
			}
			catch (Exception e)
			{
				KillbitWatcher.HandleFileWatcherException(e);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<KillbitWatcher>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (!disposing)
			{
				return;
			}
			if (KillbitWatcher.watcher == null)
			{
				return;
			}
			KillbitWatcher.watcher.Dispose();
		}

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private static FileSystemWatcher watcher;

		public delegate void ReadKillBitFromFileCallback(object source, FileSystemEventArgs e);
	}
}
