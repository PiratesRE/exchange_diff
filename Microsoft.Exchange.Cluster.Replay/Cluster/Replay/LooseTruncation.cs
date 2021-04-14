using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LooseTruncation
	{
		public bool Enabled { get; set; }

		public int MinCopiesProtect { get; set; }

		public int MinLogsToPreserve { get; set; }

		public long LowSpaceThresholdInMB { get; set; }

		public LooseTruncation()
		{
			this.InitFromRegistry();
		}

		private void InitFromRegistry()
		{
			Exception ex = null;
			this.Enabled = false;
			this.MinCopiesProtect = 0;
			this.MinLogsToPreserve = 100000;
			this.LowSpaceThresholdInMB = 200000L;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\BackupInformation\\"))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue("LooseTruncation_MinCopiesToProtect");
						if (value != null && value is int)
						{
							int num = (int)value;
							if (num <= 0)
							{
								return;
							}
							this.MinCopiesProtect = num;
						}
						value = registryKey.GetValue("LooseTruncation_MinLogsToProtect");
						if (value != null && value is int)
						{
							int num = (int)value;
							if (num >= 0)
							{
								this.MinLogsToPreserve = num;
							}
						}
						value = registryKey.GetValue("LooseTruncation_MinDiskFreeSpaceThresholdInMB");
						if (value != null && value is int)
						{
							int num = (int)value;
							if (num <= 0)
							{
								return;
							}
							this.LowSpaceThresholdInMB = (long)num;
						}
						this.Enabled = true;
					}
				}
			}
			catch (SecurityException ex2)
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
				LooseTruncation.Tracer.TraceError<Exception>(0L, "LooseTruncation failed to read registry: {0}", ex);
			}
		}

		public bool SpaceIsLow(IReplayConfiguration config)
		{
			string destinationLogPath = config.DestinationLogPath;
			ulong num;
			ulong num2;
			Exception freeSpace = DiskHelper.GetFreeSpace(destinationLogPath, out num, out num2);
			if (freeSpace != null)
			{
				LooseTruncation.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "SpaceIsLow: GetFreeSpace() failed against directory '{0}'. Exception: {1}", destinationLogPath, freeSpace);
				ReplayCrimsonEvents.TruncationFailedToGetDiskSpace.LogPeriodic<string, string, string>(config.Identity, DiagCore.DefaultEventSuppressionInterval, config.Identity, freeSpace.Message, destinationLogPath);
				return false;
			}
			bool flag = false;
			long num3 = (long)(num2 / 1048576UL);
			if (num3 < this.LowSpaceThresholdInMB)
			{
				flag = true;
			}
			LooseTruncation.Tracer.TraceDebug<bool, long, long>((long)this.GetHashCode(), "SpaceIsLow={0}. FreeMB={1} LowSpaceThresholdInMB={2}", flag, num3, this.LowSpaceThresholdInMB);
			return flag;
		}

		public long MinRequiredGen(IFileChecker fileChecker, IReplayConfiguration config)
		{
			long num = fileChecker.FileState.LowestGenerationRequired;
			if (!config.CircularLoggingEnabled && fileChecker.FileState.LastGenerationBackedUp != 0L)
			{
				num = Math.Min(num, fileChecker.FileState.LastGenerationBackedUp);
			}
			long copyNotificationGenerationNumber = config.ReplayState.CopyNotificationGenerationNumber;
			if (copyNotificationGenerationNumber <= (long)this.MinLogsToPreserve)
			{
				return 0L;
			}
			long num2 = (long)(this.MinLogsToPreserve / 10);
			if (num <= num2)
			{
				return 0L;
			}
			return Math.Min(copyNotificationGenerationNumber - (long)this.MinLogsToPreserve, num - num2);
		}

		private const string RegValName_LooseTruncation_MinDiskFreeSpaceInMB = "LooseTruncation_MinDiskFreeSpaceThresholdInMB";

		private const string RegValName_LooseTruncation_MinCopiesToProtect = "LooseTruncation_MinCopiesToProtect";

		private const string RegValName_LooseTruncation_MinLogsToProtect = "LooseTruncation_MinLogsToProtect";

		private const string RegistryRootName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\BackupInformation\\";

		private static readonly Trace Tracer = ExTraceGlobals.LogTruncaterTracer;
	}
}
