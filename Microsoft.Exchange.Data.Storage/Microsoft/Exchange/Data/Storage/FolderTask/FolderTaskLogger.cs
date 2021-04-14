using System;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;
using Microsoft.Exchange.Data.Storage.PublicFolder;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Data.Storage.FolderTask
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class FolderTaskLogger : FolderTaskLoggerBase
	{
		public FolderTaskLogger(IStoreSession storeSession, string configurationName, string lastCycleLogConfigurationName, string logComponent, string logSuffixName, Guid? correlationId = null) : base(storeSession, logComponent, logSuffixName, correlationId)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				using (Folder logFolder = this.GetLogFolder())
				{
					this.diagnosticsMetadata = UserConfiguration.GetConfiguration(logFolder, new UserConfigurationName(lastCycleLogConfigurationName, ConfigurationNameKind.Name), UserConfigurationTypes.Stream);
					this.lastCycleLogMetadata = UserConfiguration.GetConfiguration(logFolder, new UserConfigurationName(configurationName, ConfigurationNameKind.Name), UserConfigurationTypes.Dictionary);
					this.loggingStream = this.diagnosticsMetadata.GetStream();
					disposeGuard.Add<Stream>(this.loggingStream);
					this.loggingStream.SetLength(0L);
					this.loggingStream.Flush();
					this.gZipLoggingStream = new GZipStream(this.loggingStream, CompressionMode.Compress, true);
				}
				this.SetSyncMetadataValue("LastAttemptedSyncTime", ExDateTime.UtcNow);
				disposeGuard.Success();
			}
		}

		public Exception LastError { get; set; }

		public string LastSyncInfo { get; set; }

		public virtual Folder GetLogFolder()
		{
			return Folder.Bind((StoreSession)this.storeSession, DefaultFolderType.Root);
		}

		public bool TrySave()
		{
			try
			{
				this.LogFinalFoldersStats();
				if (this.LastError == null)
				{
					this.SetSyncMetadataValue("NumberofAttemptsAfterLastSuccess", 0);
					this.SetSyncMetadataValue("FirstFailedSyncTimeAfterLastSuccess", null);
					this.LogEvent(LogEventType.Success, "Diagnostics for monitoring is successfully completed");
					this.SetSyncMetadataValue("LastSuccessfulSyncTime", ExDateTime.UtcNow);
				}
				else
				{
					this.SetSyncMetadataValue("LastSyncFailure", FolderTaskLoggerBase.GetExceptionLogString(this.LastError, FolderTaskLoggerBase.ExceptionLogOption.All));
					int num;
					this.TryGetSyncMetadataValue<int>("NumberofAttemptsAfterLastSuccess", out num);
					num++;
					this.SetSyncMetadataValue("NumberofAttemptsAfterLastSuccess", num);
					this.LogEvent(LogEventType.Error, "Diagnostics for monitoring is failed");
					ExDateTime utcNow = ExDateTime.UtcNow;
					this.SetSyncMetadataValue("LastFailedSyncTime", utcNow);
					ExDateTime exDateTime = default(ExDateTime);
					this.TryGetSyncMetadataValue<ExDateTime>("FirstFailedSyncTimeAfterLastSuccess", out exDateTime);
					if (num == 1 || exDateTime == default(ExDateTime))
					{
						exDateTime = utcNow;
						this.SetSyncMetadataValue("FirstFailedSyncTimeAfterLastSuccess", exDateTime);
					}
					this.PublishMonitoringResult(num, exDateTime, utcNow);
				}
				this.gZipLoggingStream.Dispose();
				this.gZipLoggingStream = null;
				this.loggingStream.Dispose();
				this.loggingStream = null;
				this.diagnosticsMetadata.Save();
				this.lastCycleLogMetadata.Save();
				return true;
			}
			catch (StorageTransientException exception)
			{
				FolderTaskLoggerBase.LogOnServer(exception, this.logComponent, this.logSuffixName);
			}
			catch (StoragePermanentException exception2)
			{
				FolderTaskLoggerBase.LogOnServer(exception2, this.logComponent, this.logSuffixName);
			}
			return false;
		}

		public void SaveCheckPoint()
		{
			this.lastCycleLogMetadata.Save();
		}

		public void SetSyncMetadataValue(string name, object value)
		{
			IDictionary dictionary = this.lastCycleLogMetadata.GetDictionary();
			dictionary[name] = value;
		}

		public bool TryGetSyncMetadataValue<T>(string name, out T propertyValue)
		{
			propertyValue = default(T);
			IDictionary dictionary = this.lastCycleLogMetadata.GetDictionary();
			if (dictionary.Contains(name))
			{
				object obj = dictionary[name];
				if (obj != null)
				{
					try
					{
						propertyValue = (T)((object)obj);
					}
					catch (InvalidCastException arg)
					{
						this.LogEvent(LogEventType.Error, string.Format("TryGetSyncMetadataValue: Got InvalidCastException: {0}", arg));
						return false;
					}
					return true;
				}
			}
			return false;
		}

		public override void ReportError(string errorContextMessage, Exception syncException)
		{
			base.ReportError(errorContextMessage, syncException);
			this.LastError = syncException;
		}

		public new void LogEvent(LogEventType eventType, string data)
		{
			LogRowFormatter logRowFormatter = base.LogEvent(eventType, data, FolderTaskLoggerBase.LogType.Folder);
			if (this.gZipLoggingStream != null)
			{
				logRowFormatter.Write(this.gZipLoggingStream);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				base.InternalDispose(disposing);
				if (this.gZipLoggingStream != null)
				{
					this.gZipLoggingStream.Dispose();
					this.gZipLoggingStream = null;
				}
				if (this.loggingStream != null)
				{
					this.loggingStream.Dispose();
					this.loggingStream = null;
				}
				if (this.diagnosticsMetadata != null)
				{
					this.diagnosticsMetadata.Dispose();
					this.diagnosticsMetadata = null;
				}
				if (this.lastCycleLogMetadata != null)
				{
					this.lastCycleLogMetadata.Dispose();
					this.lastCycleLogMetadata = null;
				}
			}
		}

		protected void PublishMonitoringResult(int numberofAttemptsAfterLastSuccess, ExDateTime firstFailedSyncTimeAfterLastSuccess, ExDateTime lastFailedSyncTime)
		{
			TimeSpan t = lastFailedSyncTime - firstFailedSyncTimeAfterLastSuccess;
			if (numberofAttemptsAfterLastSuccess >= FolderTaskLogger.MinNumberOfFailedSyncAttemptsForAlert && t >= FolderTaskLogger.MinDurationOfSyncFailureForAlert)
			{
				string name = ExchangeComponent.PublicFolders.Name;
				string component = "PublicFolderMailboxSync";
				string empty = string.Empty;
				EventNotificationItem eventNotificationItem = new EventNotificationItem(name, component, empty, ResultSeverityLevel.Error);
				eventNotificationItem.StateAttribute1 = this.storeSession.MailboxGuid.ToString();
				eventNotificationItem.StateAttribute2 = this.storeSession.MdbGuid.ToString();
				eventNotificationItem.StateAttribute3 = ((this.storeSession.OrganizationId != null && this.storeSession.OrganizationId.OrganizationalUnit != null) ? this.storeSession.OrganizationId.OrganizationalUnit.Name.ToString() : string.Empty);
				if (this.LastError == null)
				{
					eventNotificationItem.Message = "No LastError but failing for at least this long: " + FolderTaskLogger.MinDurationOfSyncFailureForAlert;
				}
				else
				{
					eventNotificationItem.Message = FolderTaskLoggerBase.GetExceptionLogString(this.LastError, FolderTaskLoggerBase.ExceptionLogOption.All);
				}
				try
				{
					eventNotificationItem.Publish(false);
				}
				catch (UnauthorizedAccessException exception)
				{
					this.LogEvent(LogEventType.Warning, string.Format("PublishMonitoringResult: Failed with exception {0}", FolderTaskLoggerBase.GetExceptionLogString(exception, FolderTaskLoggerBase.ExceptionLogOption.All)));
				}
				catch (EventLogNotFoundException exception2)
				{
					this.LogEvent(LogEventType.Warning, string.Format("PublishMonitoringResult: Failed with exception {0}", FolderTaskLoggerBase.GetExceptionLogString(exception2, FolderTaskLoggerBase.ExceptionLogOption.All)));
				}
			}
		}

		protected abstract void LogFinalFoldersStats();

		private static readonly TimeSpan MinDurationOfSyncFailureForAlert = EnhancedTimeSpan.FromHours(6.0);

		private static readonly int MinNumberOfFailedSyncAttemptsForAlert = 7;

		private UserConfiguration diagnosticsMetadata;

		private UserConfiguration lastCycleLogMetadata;

		private Stream gZipLoggingStream;

		private Stream loggingStream;
	}
}
