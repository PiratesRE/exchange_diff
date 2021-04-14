using System;
using Microsoft.Exchange.Cluster.EseBack;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.HA.FailureItem;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogShipContextWrapper : SafeRefCountedTimeoutWrapper
	{
		public LogShipContextWrapper(string sourceMachineFqdn, string serverName, Guid identityGuid, string logFilePrefix, string destLogPath, bool circularLogging, TimeSpan timeout, ManualOneShotEvent cancelEvent) : base("LogShipContextWrapper", cancelEvent)
		{
			this.m_config = new LogShipContextWrapper.TruncationConfiguration
			{
				SourceMachine = sourceMachineFqdn,
				ServerName = serverName,
				IdentityGuid = identityGuid,
				LogFilePrefix = logFilePrefix,
				DestinationLogPath = destLogPath,
				CircularLoggingEnabled = circularLogging
			};
			this.m_timeout = timeout;
		}

		public LogShipContextWrapper(ITruncationConfiguration config, TimeSpan timeout, ManualOneShotEvent cancelEvent) : base("LogShipContextWrapper", cancelEvent)
		{
			this.m_config = config;
			this.m_timeout = timeout;
		}

		private CLogShipContext LogShipContext
		{
			get
			{
				CLogShipContext logShipContext;
				lock (this.m_lockObj)
				{
					if (this.m_logShipContext == null || this.m_logShipContext.IsInvalid)
					{
						this.Open();
					}
					logShipContext = this.m_logShipContext;
				}
				return logShipContext;
			}
		}

		public void Truncate(long lgenReplicated, ref long lgenTruncatedLocally)
		{
			uint hr = 0U;
			uint tmpDwExtError = 0U;
			long tempLgenTruncatedLocally = 0L;
			try
			{
				base.ProtectedCall("LogShipTruncate", delegate
				{
					hr = this.LogShipContext.LogShipTruncate(lgenReplicated, ref tempLgenTruncatedLocally, out tmpDwExtError);
				});
			}
			catch (TimeoutException ex)
			{
				throw new FailedToTruncateLocallyException(258U, ReplayStrings.ReplayTestStoreConnectivityTimedoutException("LogShipTruncate", ex.Message), ex);
			}
			catch (OperationAbortedException ex2)
			{
				throw new FailedToTruncateLocallyException(1003U, ex2.Message, ex2);
			}
			if (hr == 0U)
			{
				lgenTruncatedLocally = tempLgenTruncatedLocally;
				return;
			}
			string optionalFriendlyError = SeedHelper.TranslateEsebackErrorCode((long)((ulong)hr), (long)((ulong)tmpDwExtError)) ?? ReplayStrings.SeederEcUndefined((int)hr);
			throw new FailedToTruncateLocallyException(hr, optionalFriendlyError);
		}

		public void Notify(long lgenReplicated, ref long lgenLowestRequiredGlobally)
		{
			uint hr = 0U;
			uint tmpDwExtError = 0U;
			long tempLgenLowestRequiredGlobally = 0L;
			try
			{
				base.ProtectedCallWithTimeout("LogShipNotify", this.m_timeout, delegate
				{
					hr = this.LogShipContext.LogShipNotify(lgenReplicated, ref tempLgenLowestRequiredGlobally, out tmpDwExtError);
				});
			}
			catch (TimeoutException ex)
			{
				throw new FailedToNotifySourceLogTruncException(this.m_config.SourceMachine, 258U, ReplayStrings.ReplayTestStoreConnectivityTimedoutException("LogShipNotify", ex.Message), ex);
			}
			catch (OperationAbortedException ex2)
			{
				throw new FailedToNotifySourceLogTruncException(this.m_config.SourceMachine, 1003U, ex2.Message, ex2);
			}
			if (hr == 0U)
			{
				lgenLowestRequiredGlobally = tempLgenLowestRequiredGlobally;
				return;
			}
			string optionalFriendlyError = SeedHelper.TranslateEsebackErrorCode((long)((ulong)hr), (long)((ulong)tmpDwExtError)) ?? ReplayStrings.SeederEcUndefined((int)hr);
			throw new FailedToNotifySourceLogTruncException(this.m_config.SourceMachine, hr, optionalFriendlyError);
		}

		protected override void InternalProtectedDispose()
		{
			if (this.m_logShipContext != null && !this.m_logShipContext.IsInvalid)
			{
				this.m_logShipContext.Dispose();
				this.m_logShipContext = null;
			}
		}

		private void Open()
		{
			if (this.m_logShipContext != null && !this.m_logShipContext.IsInvalid)
			{
				return;
			}
			uint hr = 0U;
			uint tmpDwExtError = 0U;
			CLogShipContext tmpLogShipContext = null;
			uint rpcTimeoutMsec = (uint)this.m_timeout.TotalMilliseconds;
			string guid = this.m_config.IdentityGuid.ToString();
			Action operation = delegate()
			{
				hr = CLogShipContext.Open(this.m_config.SourceMachine, guid, this.m_config.ServerName, this.m_config.LogFilePrefix, this.m_config.DestinationLogPath, this.m_config.CircularLoggingEnabled, ReplicaType.StandbyReplica, rpcTimeoutMsec, out tmpDwExtError, out tmpLogShipContext);
				if (hr == 0U)
				{
					this.m_logShipContext = tmpLogShipContext;
				}
			};
			try
			{
				base.ProtectedCallWithTimeout("LogShipOpenContext", this.m_timeout, operation);
				if (hr != 0U || tmpLogShipContext == null || tmpLogShipContext.IsInvalid)
				{
					if (hr == 3355379665U)
					{
						throw new CopyUnknownToActiveLogTruncationException(guid, this.m_config.SourceMachine, this.m_config.ServerName, hr);
					}
					string text;
					if (hr == 3355444321U)
					{
						text = ReplayStrings.FileSystemCorruptionDetected(this.m_config.DestinationLogPath);
						ReplayEventLogConstants.Tuple_FilesystemCorrupt.LogEvent(this.m_config.IdentityGuid.ToString(), new object[]
						{
							this.m_config.DestinationLogPath
						});
						FailureItemPublisherHelper.PublishAction(FailureTag.FileSystemCorruption, this.m_config.IdentityGuid, this.m_config.IdentityGuid.ToString());
					}
					else
					{
						text = SeedHelper.TranslateEsebackErrorCode((long)((ulong)hr), (long)((ulong)tmpDwExtError));
						if (text == null)
						{
							text = ReplayStrings.SeederEcUndefined((int)hr);
						}
					}
					throw new FailedToOpenLogTruncContextException(this.m_config.SourceMachine, hr, text);
				}
			}
			catch (TimeoutException ex)
			{
				throw new FailedToOpenLogTruncContextException(this.m_config.SourceMachine, 258U, ReplayStrings.ReplayTestStoreConnectivityTimedoutException("LogShipOpenContext", ex.Message), ex);
			}
			catch (OperationAbortedException ex2)
			{
				throw new FailedToOpenLogTruncContextException(this.m_config.SourceMachine, 1003U, ex2.Message, ex2);
			}
		}

		private readonly object m_lockObj = new object();

		private readonly ITruncationConfiguration m_config;

		private readonly TimeSpan m_timeout;

		private CLogShipContext m_logShipContext;

		private class TruncationConfiguration : ITruncationConfiguration
		{
			public string SourceMachine { get; set; }

			public string ServerName { get; set; }

			public Guid IdentityGuid { get; set; }

			public string LogFilePrefix { get; set; }

			public string DestinationLogPath { get; set; }

			public bool CircularLoggingEnabled { get; set; }
		}
	}
}
