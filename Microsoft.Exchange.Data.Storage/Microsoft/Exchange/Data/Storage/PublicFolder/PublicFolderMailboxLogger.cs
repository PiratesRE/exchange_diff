using System;
using System.Collections;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.IO.Compression;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Data.Storage.PublicFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PublicFolderMailboxLogger : PublicFolderMailboxLoggerBase
	{
		public PublicFolderMailboxLogger(IPublicFolderSession publicFolderSession, string configurationName, string lastCycleLogConfigurationName, Guid? correlationId = null) : base(publicFolderSession, correlationId)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				IXSOFactory ixsofactory = new XSOFactory();
				using (Folder folder = ixsofactory.BindToFolder(publicFolderSession, publicFolderSession.GetTombstonesRootFolderId()) as Folder)
				{
					this.diagnosticsMetadata = UserConfiguration.GetConfiguration(folder, new UserConfigurationName(lastCycleLogConfigurationName, ConfigurationNameKind.Name), UserConfigurationTypes.Stream);
					this.lastCycleLogMetadata = UserConfiguration.GetConfiguration(folder, new UserConfigurationName(configurationName, ConfigurationNameKind.Name), UserConfigurationTypes.Dictionary);
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
					this.SetSyncMetadataValue("LastSyncFailure", PublicFolderMailboxLoggerBase.GetExceptionLogString(this.LastError, PublicFolderMailboxLoggerBase.ExceptionLogOption.All));
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
				PublicFolderMailboxLoggerBase.LogOnServer(exception, this.logComponent, this.logSuffixName);
			}
			catch (StoragePermanentException exception2)
			{
				PublicFolderMailboxLoggerBase.LogOnServer(exception2, this.logComponent, this.logSuffixName);
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
					propertyValue = (T)((object)obj);
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
			LogRowFormatter logRowFormatter = null;
			base.LogEvent(eventType, data, out logRowFormatter);
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
			if (numberofAttemptsAfterLastSuccess >= PublicFolderMailboxLogger.MinNumberOfFailedSyncAttemptsForAlert && t >= PublicFolderMailboxLogger.MinDurationOfSyncFailureForAlert)
			{
				string name = ExchangeComponent.PublicFolders.Name;
				string component = "PublicFolderMailboxSync";
				string empty = string.Empty;
				EventNotificationItem eventNotificationItem = new EventNotificationItem(name, component, empty, ResultSeverityLevel.Error);
				eventNotificationItem.StateAttribute1 = this.pfSession.MailboxGuid.ToString();
				eventNotificationItem.StateAttribute2 = this.pfSession.MdbGuid.ToString();
				eventNotificationItem.StateAttribute3 = ((this.pfSession.OrganizationId.OrganizationalUnit != null) ? this.pfSession.OrganizationId.OrganizationalUnit.Name.ToString() : string.Empty);
				if (this.LastError == null)
				{
					eventNotificationItem.Message = "No LastError but failing for at least this long: " + PublicFolderMailboxLogger.MinDurationOfSyncFailureForAlert;
				}
				else
				{
					eventNotificationItem.Message = PublicFolderMailboxLoggerBase.GetExceptionLogString(this.LastError, PublicFolderMailboxLoggerBase.ExceptionLogOption.All);
				}
				try
				{
					eventNotificationItem.Publish(false);
				}
				catch (UnauthorizedAccessException e)
				{
					this.LogEvent(LogEventType.Warning, string.Format("PublishMonitoringResult: Failed with exception {0}", PublicFolderMailboxLoggerBase.GetExceptionLogString(e, PublicFolderMailboxLoggerBase.ExceptionLogOption.All)));
				}
				catch (EventLogNotFoundException e2)
				{
					this.LogEvent(LogEventType.Warning, string.Format("PublishMonitoringResult: Failed with exception {0}", PublicFolderMailboxLoggerBase.GetExceptionLogString(e2, PublicFolderMailboxLoggerBase.ExceptionLogOption.All)));
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
