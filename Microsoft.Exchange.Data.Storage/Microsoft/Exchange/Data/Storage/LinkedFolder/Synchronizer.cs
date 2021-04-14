using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class Synchronizer : DisposableObject
	{
		public object SyncResult { get; protected set; }

		public Exception LastError { get; protected set; }

		public SyncOption SyncOption
		{
			get
			{
				if (this.Job == null)
				{
					return SyncOption.Default;
				}
				return this.Job.SyncOption;
			}
		}

		public Synchronizer(TeamMailboxSyncJob job, MailboxSession mailboxSession, IResourceMonitor resourceMonitor, string siteUrl, ICredentials credential, bool isOAuthCredential, bool enableHttpDebugProxy, Stream syncCycleLogStream)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (resourceMonitor == null)
			{
				throw new ArgumentNullException("resourceMonitor");
			}
			if (string.IsNullOrEmpty(siteUrl))
			{
				throw new ArgumentNullException("siteUrl");
			}
			try
			{
				this.siteUri = new Uri(siteUrl);
			}
			catch (UriFormatException innerException)
			{
				throw new ArgumentException(string.Format("Invalid format for siteUrl: {0}", siteUrl), innerException);
			}
			if (!this.siteUri.IsAbsoluteUri)
			{
				throw new ArgumentException(string.Format("Expect siteUrl: {0} to be absolute Uri", siteUrl));
			}
			this.mailboxSession = mailboxSession;
			this.Job = job;
			this.credential = credential;
			this.isOAuthCredential = isOAuthCredential;
			this.resourceMonitor = resourceMonitor;
			this.enableHttpDebugProxy = enableHttpDebugProxy;
			this.loggingContext = new LoggingContext(this.mailboxSession.MailboxGuid, this.siteUri.ToString(), (job != null) ? job.ClientString : string.Empty, syncCycleLogStream);
		}

		protected AsyncCallback WrapCallbackWithUnhandledExceptionAndSendReport(AsyncCallback callback)
		{
			if (callback == null)
			{
				return null;
			}
			return delegate(IAsyncResult asyncResult)
			{
				this.ProtectedExecution(callback, asyncResult);
			};
		}

		protected CancelableAsyncCallback WrapCallbackWithUnhandledExceptionAndSendReportEx(AsyncCallback callback)
		{
			if (callback == null)
			{
				return null;
			}
			return delegate(ICancelableAsyncResult asyncResult)
			{
				this.ProtectedExecution(callback, asyncResult);
			};
		}

		protected abstract LocalizedString GetSyncIssueEmailErrorString(string error, out LocalizedString body);

		protected abstract void InitializeSyncMetadata();

		protected void InitializeHttpClient(string method)
		{
			if (this.httpClient == null)
			{
				this.httpClient = new HttpClient();
				this.httpSessionConfig = new HttpSessionConfig();
				this.httpSessionConfig.Method = method;
				this.httpSessionConfig.Credentials = this.credential;
				this.httpSessionConfig.UserAgent = Utils.GetUserAgentStringForSiteMailboxRequests();
				if (this.enableHttpDebugProxy)
				{
					this.httpSessionConfig.Proxy = new WebProxy("127.0.0.1", 8888);
				}
			}
		}

		protected void SetCommonOauthRequestHeaders()
		{
			this.httpSessionConfig.PreAuthenticate = true;
			this.httpSessionConfig.Headers = new WebHeaderCollection();
			this.httpSessionConfig.Headers["Authorization"] = "Bearer";
			this.httpSessionConfig.Headers["X-RequestForceAuthentication"] = "true";
			this.httpSessionConfig.Headers["client-request-id"] = Guid.NewGuid().ToString();
			this.httpSessionConfig.Headers["return-client-request-id"] = "true";
		}

		protected void PublishMonitoringResult()
		{
			string name = ExchangeComponent.SiteMailbox.Name;
			string name2 = base.GetType().Name;
			string notificationReason = this.siteUri.AbsoluteUri.Replace('/', '\\');
			ResultSeverityLevel severity = ResultSeverityLevel.Informational;
			string message = string.Empty;
			if (this.LastError != null)
			{
				severity = ResultSeverityLevel.Error;
				message = this.LastError.Message;
			}
			EventNotificationItem eventNotificationItem = new EventNotificationItem(name, name2, notificationReason, severity);
			eventNotificationItem.Message = message;
			try
			{
				eventNotificationItem.Publish(false);
			}
			catch (UnauthorizedAccessException exception)
			{
				ProtocolLog.LogError(ProtocolLog.Component.Monitor, this.loggingContext, "PublishMonitoringResult failed with UnauthorizedAccessException", exception);
			}
			catch (EventLogNotFoundException exception2)
			{
				ProtocolLog.LogError(ProtocolLog.Component.Monitor, this.loggingContext, "PublishMonitoringResult failed with EventLogNotFoundException", exception2);
			}
		}

		protected void SaveSyncMetadata()
		{
			if (this.syncMetadata != null)
			{
				try
				{
					if (this.LastError != null)
					{
						this.UpdateSyncMetadataOnSyncFailure();
					}
					else
					{
						this.UpdateSyncMetadataOnSuccessfulSync();
					}
					this.syncMetadata.Save();
				}
				catch (StorageTransientException exception)
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SaveSyncMetadata: Failed with StorageTransientException", exception);
				}
				catch (StoragePermanentException exception2)
				{
					ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "SaveSyncMetadata: Failed with StoragePermanentException", exception2);
				}
			}
		}

		protected void SetSyncMetadataValue(string name, object value)
		{
			if (this.syncMetadata != null)
			{
				IDictionary dictionary = this.syncMetadata.GetDictionary();
				dictionary[name] = value;
			}
		}

		protected object GetSyncMetadataValue(string name)
		{
			if (this.syncMetadata == null)
			{
				return null;
			}
			IDictionary dictionary = this.syncMetadata.GetDictionary();
			if (dictionary.Contains(name))
			{
				return dictionary[name];
			}
			return null;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.syncMetadata != null)
			{
				this.syncMetadata.Dispose();
				this.syncMetadata = null;
			}
			base.InternalDispose(disposing);
		}

		protected void UpdateSyncMetadataOnBeginSync()
		{
			if (!(this.GetSyncMetadataValue("FirstAttemptedSyncTime") is ExDateTime?))
			{
				this.SetSyncMetadataValue("FirstAttemptedSyncTime", ExDateTime.UtcNow);
			}
			this.lastAttemptedSyncTime = ExDateTime.UtcNow;
			this.SetSyncMetadataValue("LastAttemptedSyncTime", this.lastAttemptedSyncTime);
		}

		private void UpdateSyncMetadataOnSuccessfulSync()
		{
			this.SetSyncMetadataValue("LastSuccessfulSyncTime", ExDateTime.UtcNow);
			this.SetSyncMetadataValue("LastSyncFailure", null);
			this.SetSyncMetadataValue("LastFailedSyncTime", null);
			this.SetSyncMetadataValue("LastFailedSyncEmailTime", null);
		}

		private void UpdateSyncMetadataOnSyncFailure()
		{
			try
			{
				ExDateTime? exDateTime = this.GetSyncMetadataValue("LastSuccessfulSyncTime") as ExDateTime?;
				ExDateTime? exDateTime2 = this.GetSyncMetadataValue("FirstAttemptedSyncTime") as ExDateTime?;
				ExDateTime? exDateTime3 = this.GetSyncMetadataValue("LastFailedSyncEmailTime") as ExDateTime?;
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(ProtocolLog.GetExceptionLogString(this.LastError));
				if (this.LastError is SharePointException)
				{
					stringBuilder.AppendLine("SharePointException Diagnostic Info:");
					stringBuilder.AppendLine(((SharePointException)this.LastError).DiagnosticInfo);
				}
				this.SetSyncMetadataValue("LastSyncFailure", stringBuilder.ToString());
				this.SetSyncMetadataValue("LastFailedSyncTime", ExDateTime.UtcNow);
				ExDateTime? exDateTime4 = exDateTime ?? exDateTime2;
				if ((exDateTime3 == null || ExDateTime.UtcNow - exDateTime3 > TimeSpan.FromHours(24.0)) && exDateTime4 != null && ExDateTime.UtcNow - exDateTime4 > TimeSpan.FromHours(12.0))
				{
					StoreObjectId destFolderId = Utils.EnsureSyncIssueFolder(this.mailboxSession);
					using (MessageItem messageItem = MessageItem.Create(this.mailboxSession, destFolderId))
					{
						LocalizedString empty = LocalizedString.Empty;
						messageItem.From = new Participant(this.Job.SyncInfoEntry.MailboxPrincipal);
						messageItem.Subject = this.GetSyncIssueEmailErrorString(this.LastError.Message, out empty);
						using (TextWriter textWriter = messageItem.Body.OpenTextWriter(BodyFormat.TextHtml))
						{
							messageItem.Body.Reset();
							using (HtmlWriter htmlWriter = new HtmlWriter(textWriter))
							{
								htmlWriter.WriteStartTag(HtmlTagId.P);
								htmlWriter.WriteText(empty);
								htmlWriter.WriteEndTag(HtmlTagId.P);
							}
						}
						messageItem.IsDraft = false;
						messageItem.MarkAsUnread(true);
						messageItem.Save(SaveMode.NoConflictResolutionForceSave);
						this.SetSyncMetadataValue("LastFailedSyncEmailTime", ExDateTime.UtcNow);
					}
				}
			}
			catch (StorageTransientException exception)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "UpdateSyncMetadataOnSyncFailure: Failed with StorageTransientException", exception);
			}
			catch (StoragePermanentException exception2)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "UpdateSyncMetadataOnSyncFailure: Failed with StoragePermanentException", exception2);
			}
		}

		private void ProtectedExecution(AsyncCallback callback, IAsyncResult asyncResult)
		{
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					callback(asyncResult);
				});
			}
			catch (GrayException ex)
			{
				ProtocolLog.LogError(this.loggingComponent, this.loggingContext, "WrapCallbackWithUnhandledExceptionAndSendReport: Failed with unexpected exception", ex);
				this.executionAsyncResult.InvokeCallback(ex);
			}
		}

		public abstract IAsyncResult BeginExecute(AsyncCallback executeCallback, object state);

		public abstract void EndExecute(IAsyncResult asyncResult);

		protected bool HandleShutDown()
		{
			if (this.Job != null && this.Job.IsShuttingdown)
			{
				this.resourceMonitor.ResetBudget();
				ProtocolLog.LogInformation(this.loggingComponent, this.loggingContext, "HandleShutDown: Shutdown the job by invoke callback");
				this.executionAsyncResult.InvokeCallback(null);
				return true;
			}
			return false;
		}

		protected readonly TeamMailboxSyncJob Job;

		protected MailboxSession mailboxSession;

		protected Uri siteUri;

		protected LoggingContext loggingContext;

		protected bool enableHttpDebugProxy;

		protected Stopwatch executeStopwatch;

		protected LazyAsyncResult executionAsyncResult;

		protected LazyAsyncResult exchangeOperationsAsyncResult;

		protected ICredentials credential;

		protected bool isOAuthCredential;

		protected IResourceMonitor resourceMonitor;

		protected UserConfiguration syncMetadata;

		protected ExDateTime lastAttemptedSyncTime;

		protected HttpClient httpClient;

		protected ProtocolLog.Component loggingComponent;

		protected HttpSessionConfig httpSessionConfig;
	}
}
