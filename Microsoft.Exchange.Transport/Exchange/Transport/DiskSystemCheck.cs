using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal sealed class DiskSystemCheck : ISystemCheck
	{
		public DiskSystemCheck(SystemCheckConfig systemCheckConfig, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration) : this(new DiskSystemCheckUtilsWrapper(), systemCheckConfig, transportAppConfig, transportConfiguration)
		{
		}

		public DiskSystemCheck(IDiskSystemCheckUtilsWrapper diskSystemCheckUtil, SystemCheckConfig systemCheckConfig, TransportAppConfig transportAppConfig, ITransportConfiguration transportConfiguration)
		{
			ArgumentValidator.ThrowIfNull("systemCheckUtil", diskSystemCheckUtil);
			ArgumentValidator.ThrowIfNull("systemCheckConfig", systemCheckConfig);
			ArgumentValidator.ThrowIfNull("transportAppConfig", transportAppConfig);
			ArgumentValidator.ThrowIfNull("transportConfiguration", transportConfiguration);
			this.diskSystemCheckUtil = diskSystemCheckUtil;
			this.systemCheckConfig = systemCheckConfig;
			this.transportAppConfig = transportAppConfig;
			this.transportConfiguration = transportConfiguration;
		}

		public bool Enabled
		{
			get
			{
				return this.systemCheckConfig.IsDiskSystemCheckEnabled;
			}
		}

		public IEnumerable<string> CheckedVolumes
		{
			get
			{
				return this.checkedVolumes;
			}
		}

		public int LastLockedVolumeCheckRetryCount
		{
			get
			{
				return this.lastLockedVolumeCheckRetryCount;
			}
		}

		public string LocalizedLockedVolumeError
		{
			get
			{
				return this.localizedLockedVolumeError;
			}
		}

		public void Check()
		{
			if (this.Enabled)
			{
				List<string> list = new List<string>
				{
					this.transportAppConfig.WorkerProcess.TemporaryStoragePath,
					this.transportAppConfig.QueueDatabase.DatabasePath,
					this.transportAppConfig.QueueDatabase.LogFilePath
				};
				if (this.transportConfiguration != null && this.transportConfiguration.LocalServer != null && this.transportConfiguration.LocalServer.TransportServer != null)
				{
					Server transportServer = this.transportConfiguration.LocalServer.TransportServer;
					list.Add((transportServer.MessageTrackingLogPath != null) ? transportServer.MessageTrackingLogPath.PathName : null);
					list.Add((transportServer.ReceiveProtocolLogPath != null) ? transportServer.ReceiveProtocolLogPath.PathName : null);
					list.Add((transportServer.SendProtocolLogPath != null) ? transportServer.SendProtocolLogPath.PathName : null);
				}
				this.CheckPathsNotOnLockedVolumes(list);
			}
		}

		public void CheckPathsNotOnLockedVolumes(IList<string> transportDataPaths)
		{
			ArgumentValidator.ThrowIfNull("transportDataPaths", transportDataPaths);
			this.checkedVolumes = new List<string>();
			this.lastLockedVolumeCheckRetryCount = 0;
			foreach (string path in transportDataPaths)
			{
				if (Path.IsPathRooted(path) && !this.checkedVolumes.Contains(Path.GetPathRoot(path)))
				{
					this.checkedVolumes.Add(Path.GetPathRoot(path));
					this.lastLockedVolumeCheckRetryCount = 1;
					while (this.IsPathOnLockedVolume(path))
					{
						if (++this.lastLockedVolumeCheckRetryCount > this.systemCheckConfig.LockedVolumeCheckRetryCount)
						{
							throw new TransportComponentLoadFailedException(this.localizedLockedVolumeError, new TransientException(new LocalizedString(this.localizedLockedVolumeError)));
						}
						Thread.Sleep(this.systemCheckConfig.LockedVolumeCheckRetryInterval);
					}
				}
			}
		}

		private bool IsPathOnLockedVolume(string path)
		{
			Exception ex = null;
			this.localizedLockedVolumeError = string.Empty;
			bool flag = this.diskSystemCheckUtil.IsFilePathOnLockedVolume(path, out ex);
			if (flag || ex != null)
			{
				if (ex != null)
				{
					this.localizedLockedVolumeError = Strings.BitlockerQueryFailed(path, ex.ToString(), this.systemCheckConfig.LockedVolumeCheckRetryInterval.Seconds, this.lastLockedVolumeCheckRetryCount, this.systemCheckConfig.LockedVolumeCheckRetryCount);
					ExTraceGlobals.ConfigurationTracer.TraceError(0L, this.localizedLockedVolumeError);
					Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_BitlockerQueryFailed, null, new object[]
					{
						path,
						ex.ToString(),
						this.systemCheckConfig.LockedVolumeCheckRetryInterval.Seconds,
						this.lastLockedVolumeCheckRetryCount,
						this.systemCheckConfig.LockedVolumeCheckRetryCount
					});
				}
				else
				{
					this.localizedLockedVolumeError = Strings.FilePathOnLockedVolume(path, this.systemCheckConfig.LockedVolumeCheckRetryInterval.Seconds, this.lastLockedVolumeCheckRetryCount, this.systemCheckConfig.LockedVolumeCheckRetryCount);
					ExTraceGlobals.ConfigurationTracer.TraceError(0L, this.localizedLockedVolumeError);
					Components.EventLogger.LogEvent(TransportEventLogConstants.Tuple_FilePathOnLockedVolume, null, new object[]
					{
						path,
						this.systemCheckConfig.LockedVolumeCheckRetryInterval.Seconds,
						this.lastLockedVolumeCheckRetryCount,
						this.systemCheckConfig.LockedVolumeCheckRetryCount
					});
				}
			}
			return flag;
		}

		private IDiskSystemCheckUtilsWrapper diskSystemCheckUtil;

		private SystemCheckConfig systemCheckConfig;

		private TransportAppConfig transportAppConfig;

		private ITransportConfiguration transportConfiguration;

		private string localizedLockedVolumeError;

		private List<string> checkedVolumes;

		private int lastLockedVolumeCheckRetryCount;
	}
}
