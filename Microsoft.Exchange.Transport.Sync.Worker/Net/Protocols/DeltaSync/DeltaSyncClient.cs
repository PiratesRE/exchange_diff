using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ContentAggregation;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.ContentAggregation;
using Microsoft.Exchange.Net.LiveIDAuthentication;
using Microsoft.Exchange.Net.Logging;
using Microsoft.Exchange.Net.WebApplicationClient;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Worker.Framework;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncClient : ProtocolClient, IDeltaSyncClient, IDisposable
	{
		internal DeltaSyncClient(DeltaSyncUserAccount userAccount, int timeout, IWebProxy proxy, long maxDownloadSizePerMessage, ProtocolLog httpProtocolLog) : this(userAccount, timeout, proxy, maxDownloadSizePerMessage, httpProtocolLog, CommonLoggingHelper.SyncLogSession, null)
		{
		}

		internal DeltaSyncClient(DeltaSyncUserAccount userAccount, int timeout, IWebProxy proxy, long maxDownloadSizePerMessage, ProtocolLog httpProtocolLog, SyncLogSession syncLogSession, EventHandler<RoundtripCompleteEventArgs> roundtripCompleteEventHandler)
		{
			SyncUtilities.ThrowIfArgumentNull("userAccount", userAccount);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			this.userAccount = userAccount;
			this.httpClient = new HttpClient();
			this.httpClient.SendingRequest += DeltaSyncClient.SetupCertificateValidation;
			this.deltaSyncRequestGenerator = new DeltaSyncRequestGenerator();
			this.deltaSyncResponseHandler = new DeltaSyncResponseHandler(syncLogSession);
			if (roundtripCompleteEventHandler != null)
			{
				this.RoundtripComplete += roundtripCompleteEventHandler;
			}
			this.httpSessionConfig = new HttpSessionConfig();
			this.httpSessionConfig.Timeout = timeout;
			this.httpSessionConfig.AllowAutoRedirect = true;
			this.httpSessionConfig.Proxy = proxy;
			this.httpSessionConfig.UserAgent = "ExchangeHostedServices/1.0";
			this.httpSessionConfig.Method = "POST";
			this.httpSessionConfig.KeepAlive = true;
			this.httpSessionConfig.MaximumResponseBodyLength = -1L;
			this.httpSessionConfig.ProtocolLog = httpProtocolLog;
			this.maxDownloadSizePerMessage = maxDownloadSizePerMessage;
			this.requestStream = TemporaryStorage.Create();
			this.syncLogSession = syncLogSession;
		}

		private event EventHandler<RoundtripCompleteEventArgs> RoundtripComplete;

		internal static Uri RstLiveEndpointUri
		{
			get
			{
				return DeltaSyncClient.rstLiveEndpointUri;
			}
			set
			{
				DeltaSyncClient.rstLiveEndpointUri = value;
			}
		}

		internal ExDateTime TimeSent
		{
			get
			{
				base.CheckDisposed();
				return this.timeSent;
			}
		}

		private LiveIDAuthenticationClient AuthenticationClient
		{
			get
			{
				if (this.authenticationClient == null)
				{
					this.authenticationClient = new LiveIDAuthenticationClient(this.httpSessionConfig.Timeout, this.httpSessionConfig.Proxy, DeltaSyncClient.RstLiveEndpointUri);
					this.authenticationClient.SendingRequest += DeltaSyncClient.SetupCertificateValidation;
				}
				return this.authenticationClient;
			}
			set
			{
				this.authenticationClient = value;
			}
		}

		private AlternateWlidEndpointHandler AlternateWlidEndpointHandler
		{
			get
			{
				if (this.alternateWlidEndpointHandler == null)
				{
					this.alternateWlidEndpointHandler = new AlternateWlidEndpointHandler("DeltaSyncAlternateWlidEndpoint", this.syncLogSession, DeltaSyncClient.Tracer);
				}
				return this.alternateWlidEndpointHandler;
			}
		}

		public IAsyncResult BeginGetChanges(AsyncCallback callback, object asyncState, object syncPoisonContext)
		{
			return this.BeginGetChanges(2000, callback, asyncState, syncPoisonContext);
		}

		public IAsyncResult BeginGetChanges(int windowSize, AsyncCallback callback, object asyncState, object syncPoisonContext)
		{
			base.CheckDisposed();
			this.ThrowIfPending();
			this.syncLogSession.LogDebugging((TSLID)647UL, DeltaSyncClient.Tracer, "Begin Get Changes [User:{0}] [Window Size:{1}]", new object[]
			{
				this.userAccount,
				windowSize
			});
			this.sessionClosed = false;
			this.isLatestToken = false;
			this.commandType = DeltaSyncCommandType.Sync;
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult = new AsyncResult<DeltaSyncClient, DeltaSyncResultData>(this, this, callback, asyncState, syncPoisonContext);
			this.SetupGetChangesRequestStream((windowSize > 0) ? windowSize : 2000);
			this.InitializeRequestProperties(DeltaSyncCommon.TextXmlContentType, -1L);
			this.BeginRequest(asyncResult, false);
			return asyncResult;
		}

		public IAsyncResult BeginApplyChanges(List<DeltaSyncOperation> deltaSyncOperations, ConflictResolution conflictResolution, AsyncCallback callback, object asyncState, object syncPoisonContext)
		{
			base.CheckDisposed();
			this.ThrowIfPending();
			SyncUtilities.ThrowIfArgumentNull("deltaSyncOperations", deltaSyncOperations);
			this.syncLogSession.LogDebugging((TSLID)648UL, DeltaSyncClient.Tracer, "Begin Apply Changes [User:{0}]", new object[]
			{
				this.userAccount
			});
			this.sessionClosed = false;
			this.isLatestToken = false;
			this.commandType = DeltaSyncCommandType.Sync;
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult = new AsyncResult<DeltaSyncClient, DeltaSyncResultData>(this, this, callback, asyncState, syncPoisonContext);
			this.SetupApplyChangesRequestStream(deltaSyncOperations, conflictResolution);
			this.InitializeRequestProperties(DeltaSyncCommon.ApplicationXopXmlContentType, -1L);
			this.BeginRequest(asyncResult, false);
			return asyncResult;
		}

		public IAsyncResult BeginSendMessage(DeltaSyncMail deltaSyncEmail, bool saveInSentItems, DeltaSyncRecipients deltaSyncRecipients, AsyncCallback callback, object asyncState, object syncPoisonContext)
		{
			base.CheckDisposed();
			this.ThrowIfPending();
			SyncUtilities.ThrowIfArgumentNull("deltaSyncEmail", deltaSyncEmail);
			SyncUtilities.ThrowIfArgumentNull("deltaSyncRecipients", deltaSyncRecipients);
			if (deltaSyncRecipients.Count < 1)
			{
				throw new ArgumentOutOfRangeException("deltaSyncRecipients", "there must be at least one recipient");
			}
			this.syncLogSession.LogDebugging((TSLID)649UL, DeltaSyncClient.Tracer, "Begin Send Message [User:{0}]", new object[]
			{
				this.userAccount
			});
			this.sessionClosed = false;
			this.isLatestToken = false;
			this.commandType = DeltaSyncCommandType.Send;
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult = new AsyncResult<DeltaSyncClient, DeltaSyncResultData>(this, this, callback, asyncState, syncPoisonContext);
			this.SetupSendMessageRequestStream(deltaSyncEmail, saveInSentItems, deltaSyncRecipients);
			this.InitializeRequestProperties(DeltaSyncCommon.ApplicationXopXmlContentType, -1L);
			this.BeginRequest(asyncResult, false);
			return asyncResult;
		}

		public IAsyncResult BeginFetchMessage(Guid serverId, AsyncCallback callback, object asyncState, object syncPoisonContext)
		{
			base.CheckDisposed();
			this.ThrowIfPending();
			this.syncLogSession.LogDebugging((TSLID)650UL, DeltaSyncClient.Tracer, "Begin Fetch Message [User:{0}] [ServerId: {1}]", new object[]
			{
				this.userAccount,
				serverId
			});
			Microsoft.Exchange.Diagnostics.Components.ContentAggregation.ExTraceGlobals.FaultInjectionTracer.TraceTest(2724605245U);
			this.sessionClosed = false;
			this.isLatestToken = false;
			this.commandType = DeltaSyncCommandType.Fetch;
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult = new AsyncResult<DeltaSyncClient, DeltaSyncResultData>(this, this, callback, asyncState, syncPoisonContext);
			this.SetupFetchMessageRequestStream(serverId);
			this.InitializeRequestProperties(DeltaSyncCommon.TextXmlContentType, this.maxDownloadSizePerMessage);
			this.BeginRequest(asyncResult, false);
			return asyncResult;
		}

		public IAsyncResult BeginVerifyAccount(AsyncCallback callback, object asyncState, object syncPoisonContext)
		{
			base.CheckDisposed();
			this.syncLogSession.LogDebugging((TSLID)651UL, DeltaSyncClient.Tracer, "Begin Verify Account [User:{0}]", new object[]
			{
				this.userAccount
			});
			return this.BeginGetSettings(callback, asyncState, syncPoisonContext);
		}

		public IAsyncResult BeginGetSettings(AsyncCallback callback, object asyncState, object syncPoisonContext)
		{
			return this.BeginRequest(new DeltaSyncClient.SetupGetRequest(this.SetupGetSettingsRequestStream), DeltaSyncCommandType.Settings, callback, asyncState, syncPoisonContext);
		}

		public IAsyncResult BeginGetStatistics(AsyncCallback callback, object asyncState, object syncPoisonContext)
		{
			return this.BeginRequest(new DeltaSyncClient.SetupGetRequest(this.SetupGetStatisticsRequestStream), DeltaSyncCommandType.Stateless, callback, asyncState, syncPoisonContext);
		}

		public AsyncOperationResult<DeltaSyncResultData> EndVerifyAccount(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			this.syncLogSession.LogDebugging((TSLID)653UL, DeltaSyncClient.Tracer, "End Verify Account [User:{0}]", new object[]
			{
				this.userAccount
			});
			return this.EndGetSettings(asyncResult);
		}

		public AsyncOperationResult<DeltaSyncResultData> EndGetChanges(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			this.syncLogSession.LogDebugging((TSLID)654UL, DeltaSyncClient.Tracer, "End Get Changes [User:{0}]", new object[]
			{
				this.userAccount
			});
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult2 = (AsyncResult<DeltaSyncClient, DeltaSyncResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public AsyncOperationResult<DeltaSyncResultData> EndApplyChanges(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			this.syncLogSession.LogDebugging((TSLID)655UL, DeltaSyncClient.Tracer, "End Apply Changes [User:{0}]", new object[]
			{
				this.userAccount
			});
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult2 = (AsyncResult<DeltaSyncClient, DeltaSyncResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public AsyncOperationResult<DeltaSyncResultData> EndSendMessage(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			this.syncLogSession.LogDebugging((TSLID)656UL, DeltaSyncClient.Tracer, "End Send Message [User:{0}]", new object[]
			{
				this.userAccount
			});
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult2 = (AsyncResult<DeltaSyncClient, DeltaSyncResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public AsyncOperationResult<DeltaSyncResultData> EndFetchMessage(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			this.syncLogSession.LogDebugging((TSLID)657UL, DeltaSyncClient.Tracer, "End Fetch Message [User:{0}]", new object[]
			{
				this.userAccount
			});
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult2 = (AsyncResult<DeltaSyncClient, DeltaSyncResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public AsyncOperationResult<DeltaSyncResultData> EndGetSettings(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			this.syncLogSession.LogDebugging((TSLID)658UL, DeltaSyncClient.Tracer, "End Get Settings [User:{0}]", new object[]
			{
				this.userAccount
			});
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult2 = (AsyncResult<DeltaSyncClient, DeltaSyncResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public AsyncOperationResult<DeltaSyncResultData> EndGetStatistics(IAsyncResult asyncResult)
		{
			base.CheckDisposed();
			this.syncLogSession.LogDebugging((TSLID)298UL, DeltaSyncClient.Tracer, "End Get Statistics [User:{0}]", new object[]
			{
				this.userAccount
			});
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult2 = (AsyncResult<DeltaSyncClient, DeltaSyncResultData>)asyncResult;
			return asyncResult2.WaitForCompletion();
		}

		public void SubscribeDownloadCompletedEvent(EventHandler<DownloadCompleteEventArgs> eventHandler)
		{
			base.CheckDisposed();
			this.httpClient.DownloadCompleted += eventHandler;
		}

		public void NotifyRoundtripComplete(object sender, RoundtripCompleteEventArgs roundtripCompleteEventArgs)
		{
			base.CheckDisposed();
			if (this.RoundtripComplete != null)
			{
				this.RoundtripComplete(sender, roundtripCompleteEventArgs);
			}
		}

		internal override bool TryCancel()
		{
			base.CheckDisposed();
			bool result;
			lock (this.syncRoot)
			{
				if (this.TryStopProcess())
				{
					if (this.pendingAsyncResult != null)
					{
						this.pendingAsyncResult.Cancel();
						this.pendingAsyncResult = null;
					}
					this.syncLogSession.LogDebugging((TSLID)659UL, DeltaSyncClient.Tracer, "Pending Operation Cancelled", new object[0]);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
			lock (this.syncRoot)
			{
				if (disposing)
				{
					this.TryCancel();
					if (this.requestStream != null)
					{
						this.requestStream.Dispose();
						this.requestStream = null;
					}
					if (this.httpClient != null)
					{
						this.httpClient.Dispose();
						this.httpClient = null;
					}
					if (this.AuthenticationClient != null)
					{
						this.AuthenticationClient.Dispose();
						this.AuthenticationClient = null;
					}
					this.syncLogSession.LogDebugging((TSLID)660UL, DeltaSyncClient.Tracer, base.GetType().Name + " Disposed", new object[0]);
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DeltaSyncClient>(this);
		}

		private static bool TryLoadServiceEndpoints(out Exception exception, SyncLogSession syncLogSession)
		{
			exception = null;
			if (!DeltaSyncClient.serviceEndpointsLoaded)
			{
				lock (DeltaSyncClient.serviceEndpointSyncLock)
				{
					if (!DeltaSyncClient.serviceEndpointsLoaded)
					{
						try
						{
							Microsoft.Exchange.Diagnostics.Components.ContentAggregation.ExTraceGlobals.FaultInjectionTracer.TraceTest(3739626813U);
							ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 945, "TryLoadServiceEndpoints", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Worker\\Framework\\Provider\\DeltaSync\\Client\\DeltaSyncClient.cs");
							ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
							ServiceEndpoint endpoint = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncPartnerItemOperations);
							ServiceEndpoint endpoint2 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncPartnerSend);
							ServiceEndpoint endpoint3 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncPartnerSettings);
							ServiceEndpoint endpoint4 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncPartnerSync);
							ServiceEndpoint endpoint5 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncPartnerStateless);
							DeltaSyncClient.partnerClientToken = endpoint4.Token;
							syncLogSession.LogDebugging((TSLID)492UL, DeltaSyncClient.Tracer, "Delta Sync Partner Client Token Loaded: {0}", new object[]
							{
								DeltaSyncClient.partnerClientToken
							});
							if (DeltaSyncClient.RstLiveEndpointUri == null)
							{
								ServiceEndpoint endpoint6 = endpointContainer.GetEndpoint(ServiceEndpointId.LiveServiceLogin1);
								DeltaSyncClient.RstLiveEndpointUri = endpoint6.Uri;
							}
							DeltaSyncClient.partnerItemOperationsUri = endpoint.Uri;
							DeltaSyncClient.partnerSendUri = endpoint2.Uri;
							DeltaSyncClient.partnerSettingsUri = endpoint3.Uri;
							DeltaSyncClient.partnerSyncUri = endpoint4.Uri;
							DeltaSyncClient.partnerStatelessUri = endpoint5.Uri;
							ServiceEndpoint endpoint7 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncUserItemOperations);
							ServiceEndpoint endpoint8 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncUserSend);
							ServiceEndpoint endpoint9 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncUserSettings);
							ServiceEndpoint endpoint10 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncUserSync);
							ServiceEndpoint endpoint11 = endpointContainer.GetEndpoint(ServiceEndpointId.DeltaSyncUserStateless);
							DeltaSyncClient.passportItemOperationsUri = endpoint7.Uri;
							DeltaSyncClient.passportSendUri = endpoint8.Uri;
							DeltaSyncClient.passportSettingsUri = endpoint9.Uri;
							DeltaSyncClient.passportSyncUri = endpoint10.Uri;
							DeltaSyncClient.passportStatelessUri = endpoint11.Uri;
							syncLogSession.LogDebugging((TSLID)661UL, DeltaSyncClient.Tracer, "Delta Sync Partner Uris Loaded: Sync:{0}, Send:{1}, ItemOperations:{2}, Settings:{3} , Stateless:{4}", new object[]
							{
								DeltaSyncClient.partnerSyncUri,
								DeltaSyncClient.partnerSendUri,
								DeltaSyncClient.partnerItemOperationsUri,
								DeltaSyncClient.partnerSettingsUri,
								DeltaSyncClient.partnerStatelessUri
							});
							syncLogSession.LogDebugging((TSLID)662UL, DeltaSyncClient.Tracer, "Delta Sync Passport Uris Loaded. Sync:{0}, Send:{1}, ItemOperations:{2}, Settings:{3} , Stateless:{4}", new object[]
							{
								DeltaSyncClient.passportSyncUri,
								DeltaSyncClient.passportSendUri,
								DeltaSyncClient.passportItemOperationsUri,
								DeltaSyncClient.passportSettingsUri,
								DeltaSyncClient.passportStatelessUri
							});
							DeltaSyncClient.serviceEndpointsLoaded = true;
							return true;
						}
						catch (ServiceEndpointNotFoundException innerException)
						{
							exception = new DeltaSyncServiceEndpointsLoadException(innerException);
						}
						catch (EndpointContainerNotFoundException innerException2)
						{
							exception = new DeltaSyncServiceEndpointsLoadException(innerException2);
						}
						catch (ADTransientException innerException3)
						{
							exception = new DeltaSyncServiceEndpointsLoadException(innerException3);
						}
						catch (ADOperationException innerException4)
						{
							exception = new DeltaSyncServiceEndpointsLoadException(innerException4);
						}
						catch (DataValidationException innerException5)
						{
							exception = new DeltaSyncServiceEndpointsLoadException(innerException5);
						}
						syncLogSession.LogError((TSLID)663UL, DeltaSyncClient.Tracer, "Unable to load Service Endpoints for Delta Sync, excpetion: {0}", new object[]
						{
							exception
						});
						return false;
					}
				}
				return true;
			}
			return true;
		}

		private static bool SslCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslError)
		{
			return sslError == SslPolicyErrors.None || sslError == SslPolicyErrors.RemoteCertificateNameMismatch;
		}

		private static void SetupCertificateValidation(object sender, HttpWebRequestEventArgs e)
		{
			CertificateValidationManager.SetComponentId(e.Request, "MicrosoftExchangeServer-DeltaSyncClient");
		}

		private static void CountCommand(AsyncResult<DeltaSyncClient, DeltaSyncResultData> curOp, string remoteServerName, bool successful)
		{
			curOp.State.NotifyRoundtripComplete(null, new RemoteServerRoundtripCompleteEventArgs(remoteServerName, ExDateTime.UtcNow - curOp.State.TimeSent, successful));
		}

		private void BeginRequest(AsyncResult<DeltaSyncClient, DeltaSyncResultData> deltaSyncAsyncResult, bool forceAuth)
		{
			Exception exception = null;
			this.timeSent = ExDateTime.UtcNow;
			if (!DeltaSyncClient.TryLoadServiceEndpoints(out exception, this.syncLogSession))
			{
				this.HandleResult(deltaSyncAsyncResult, null, exception);
				return;
			}
			this.userAccount.PartnerClientToken = DeltaSyncClient.partnerClientToken;
			CertificateValidationManager.RegisterCallback("MicrosoftExchangeServer-DeltaSyncClient", new RemoteCertificateValidationCallback(DeltaSyncClient.SslCertificateValidationCallback));
			if (forceAuth || this.userAccount.NeedsAuthentication)
			{
				this.BeginAuthRequest(deltaSyncAsyncResult);
				this.isLatestToken = true;
				return;
			}
			this.BeginDeltaSyncRequest(deltaSyncAsyncResult);
		}

		private void InitializeRequestProperties(string requestContentType, long maxDownloadLimit)
		{
			this.httpSessionConfig.ContentType = requestContentType;
			this.httpSessionConfig.RequestStream = this.requestStream;
			this.httpSessionConfig.MaximumResponseBodyLength = maxDownloadLimit;
		}

		private void BeginAuthRequest(AsyncResult<DeltaSyncClient, DeltaSyncResultData> deltaSyncAsyncResult)
		{
			this.ThrowIfPending();
			this.AlternateWlidEndpointHandler.SetWlidEndpoint(this.AuthenticationClient);
			ICancelableAsyncResult asyncResult = this.AuthenticationClient.BeginGetToken("MicrosoftExchangeServer-DeltaSyncClient", this.userAccount.Username, this.userAccount.Password, this.userAccount.AuthPolicy, DeltaSyncClient.passportSyncUri.Host, deltaSyncAsyncResult.GetCancelableAsyncCallbackWithPoisonContextAndUnhandledExceptionRedirect(new CancelableAsyncCallback(this.AuthCallback)), deltaSyncAsyncResult);
			this.CachePendingAsyncResult(asyncResult);
		}

		private void CachePendingAsyncResult(ICancelableAsyncResult asyncResult)
		{
			if (!asyncResult.IsCompleted)
			{
				lock (this.syncRoot)
				{
					if (!asyncResult.IsCompleted)
					{
						this.pendingAsyncResult = asyncResult;
					}
				}
			}
		}

		private void BeginDeltaSyncRequest(AsyncResult<DeltaSyncClient, DeltaSyncResultData> deltaSyncAsyncResult)
		{
			this.ThrowIfPending();
			Uri deltaSyncRequestUri = this.GetDeltaSyncRequestUri();
			Uri uri = new UriBuilder(deltaSyncRequestUri)
			{
				Query = string.Empty
			}.Uri;
			this.syncLogSession.LogDebugging((TSLID)667UL, DeltaSyncClient.Tracer, "Using Delta Sync Request Uri:{0} for commandType: {1}", new object[]
			{
				uri,
				this.commandType
			});
			if (this.httpSessionConfig.Headers == null)
			{
				this.httpSessionConfig.Headers = new WebHeaderCollection();
			}
			this.httpSessionConfig.Headers[HttpRequestHeader.Cookie] = this.deltaSyncCookie;
			ICancelableAsyncResult asyncResult = this.httpClient.BeginDownload(deltaSyncRequestUri, this.httpSessionConfig, deltaSyncAsyncResult.GetCancelableAsyncCallbackWithPoisonContextAndUnhandledExceptionRedirect(new CancelableAsyncCallback(this.DeltaSyncResponseCallback)), deltaSyncAsyncResult);
			this.CachePendingAsyncResult(asyncResult);
		}

		private Uri GetDeltaSyncRequestUri()
		{
			Uri uriBasedOnCommandTypeAndAuthType = this.GetUriBasedOnCommandTypeAndAuthType();
			UriBuilder uriBuilder = new UriBuilder(uriBasedOnCommandTypeAndAuthType);
			if (!string.IsNullOrEmpty(this.userAccount.DeltaSyncServer))
			{
				uriBuilder.Host = this.userAccount.DeltaSyncServer;
			}
			uriBuilder.Query = this.userAccount.GetRequestQueryString();
			return uriBuilder.Uri;
		}

		private Uri GetUriBasedOnCommandTypeAndAuthType()
		{
			if (this.userAccount.PassportAuthenticationEnabled)
			{
				switch (this.commandType)
				{
				case DeltaSyncCommandType.Sync:
					return DeltaSyncClient.passportSyncUri;
				case DeltaSyncCommandType.Fetch:
					return DeltaSyncClient.passportItemOperationsUri;
				case DeltaSyncCommandType.Settings:
					return DeltaSyncClient.passportSettingsUri;
				case DeltaSyncCommandType.Send:
					return DeltaSyncClient.passportSendUri;
				case DeltaSyncCommandType.Stateless:
					return DeltaSyncClient.passportStatelessUri;
				default:
					throw new InvalidOperationException("Unknown command type: " + this.commandType);
				}
			}
			else
			{
				switch (this.commandType)
				{
				case DeltaSyncCommandType.Sync:
					return DeltaSyncClient.partnerSyncUri;
				case DeltaSyncCommandType.Fetch:
					return DeltaSyncClient.partnerItemOperationsUri;
				case DeltaSyncCommandType.Settings:
					return DeltaSyncClient.partnerSettingsUri;
				case DeltaSyncCommandType.Send:
					return DeltaSyncClient.partnerSendUri;
				case DeltaSyncCommandType.Stateless:
					return DeltaSyncClient.partnerStatelessUri;
				default:
					throw new InvalidOperationException("Unknown command type: " + this.commandType);
				}
			}
		}

		private void SetupGetChangesRequestStream(int windowSize)
		{
			this.requestStream.Position = 0L;
			this.deltaSyncRequestGenerator.SetupGetChangesRequest(this.userAccount.FolderSyncKey, this.userAccount.EmailSyncKey, windowSize, this.requestStream);
			this.requestStream.SetLength(this.requestStream.Position);
			this.requestStream.Position = 0L;
		}

		private void SetupApplyChangesRequestStream(List<DeltaSyncOperation> operations, ConflictResolution conflictResolution)
		{
			this.requestStream.Position = 0L;
			this.deltaSyncRequestGenerator.SetupApplyChangesRequest(operations, conflictResolution, this.userAccount.FolderSyncKey, this.userAccount.EmailSyncKey, this.requestStream);
			this.requestStream.SetLength(this.requestStream.Position);
			this.requestStream.Position = 0L;
		}

		private void SetupSendMessageRequestStream(DeltaSyncMail deltaSyncEmail, bool saveInSentItems, DeltaSyncRecipients recipients)
		{
			this.requestStream.Position = 0L;
			this.deltaSyncRequestGenerator.SetupSendMessageRequest(deltaSyncEmail, saveInSentItems, recipients, this.requestStream);
			this.requestStream.SetLength(this.requestStream.Position);
			this.requestStream.Position = 0L;
		}

		private void SetupFetchMessageRequestStream(Guid serverId)
		{
			this.requestStream.Position = 0L;
			this.deltaSyncRequestGenerator.SetupFetchMessageRequest(serverId, this.requestStream);
			this.requestStream.SetLength(this.requestStream.Position);
			this.requestStream.Position = 0L;
		}

		private void SetupGetSettingsRequestStream()
		{
			this.SetupRequestStream(new DeltaSyncClient.SetupRequest(this.deltaSyncRequestGenerator.SetupGetSettingsRequest));
		}

		private void SetupGetStatisticsRequestStream()
		{
			this.SetupRequestStream(new DeltaSyncClient.SetupRequest(this.deltaSyncRequestGenerator.SetupGetStatisticsRequest));
		}

		private void SetupRequestStream(DeltaSyncClient.SetupRequest setupRequest)
		{
			this.requestStream.Position = 0L;
			setupRequest(this.requestStream);
			this.requestStream.SetLength(this.requestStream.Position);
			this.requestStream.Position = 0L;
		}

		private IAsyncResult BeginRequest(DeltaSyncClient.SetupGetRequest getRequest, DeltaSyncCommandType cmdType, AsyncCallback callback, object asyncState, object syncPoisonContext)
		{
			base.CheckDisposed();
			this.ThrowIfPending();
			this.syncLogSession.LogDebugging((TSLID)652UL, DeltaSyncClient.Tracer, "Begin Get request [User:{0}]", new object[]
			{
				this.userAccount
			});
			this.sessionClosed = false;
			this.isLatestToken = false;
			this.commandType = cmdType;
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult = new AsyncResult<DeltaSyncClient, DeltaSyncResultData>(this, this, callback, asyncState, syncPoisonContext);
			getRequest();
			this.InitializeRequestProperties(DeltaSyncCommon.TextXmlContentType, -1L);
			this.BeginRequest(asyncResult, false);
			return asyncResult;
		}

		private void AuthCallback(ICancelableAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult2 = null;
			Exception exception = null;
			bool flag = false;
			lock (this.syncRoot)
			{
				if (this.sessionClosed)
				{
					return;
				}
				this.pendingAsyncResult = null;
				asyncResult2 = (AsyncResult<DeltaSyncClient, DeltaSyncResultData>)asyncResult.AsyncState;
				if (asyncResult.CompletedSynchronously)
				{
					asyncResult2.SetCompletedSynchronously();
				}
				AuthenticationResult authenticationResult = this.AuthenticationClient.EndGetToken(asyncResult);
				if (authenticationResult.IsSucceeded)
				{
					this.userAccount.AuthToken = authenticationResult.Token;
					this.userAccount.Puid = authenticationResult.Token.PUID;
					flag = true;
				}
				else
				{
					exception = authenticationResult.Exception;
				}
			}
			if (flag)
			{
				this.BeginDeltaSyncRequest(asyncResult2);
				return;
			}
			this.HandleResult(asyncResult2, null, exception);
		}

		private void DeltaSyncResponseCallback(ICancelableAsyncResult asyncResult)
		{
			AsyncResult<DeltaSyncClient, DeltaSyncResultData> asyncResult2 = null;
			DeltaSyncResultData deltaSyncResultData = null;
			bool flag = false;
			Exception exception = null;
			lock (this.syncRoot)
			{
				if (this.sessionClosed)
				{
					return;
				}
				this.pendingAsyncResult = null;
				asyncResult2 = (AsyncResult<DeltaSyncClient, DeltaSyncResultData>)asyncResult.AsyncState;
				if (asyncResult.CompletedSynchronously)
				{
					asyncResult2.SetCompletedSynchronously();
				}
				DownloadResult deltaSyncResponse = this.httpClient.EndDownload(asyncResult);
				string host = deltaSyncResponse.LastKnownRequestedUri.Host;
				if (deltaSyncResponse.IsSucceeded)
				{
					DeltaSyncClient.CountCommand(asyncResult2, host, true);
					this.userAccount.DeltaSyncServer = host;
					if (!string.IsNullOrEmpty(host))
					{
						EndPointHealth.UpdateDeltaSyncEndPointStatus(host, true, this.syncLogSession);
						this.syncLogSession.LogVerbose((TSLID)1298UL, "Connected to host: {0}", new object[]
						{
							host
						});
					}
					if (deltaSyncResponse.ResponseHeaders != null)
					{
						this.deltaSyncCookie = deltaSyncResponse.ResponseHeaders[HttpResponseHeader.SetCookie];
					}
					try
					{
						deltaSyncResultData = this.deltaSyncResponseHandler.ParseDeltaSyncResponse(deltaSyncResponse, this.commandType);
						flag = (this.userAccount.PassportAuthenticationEnabled && deltaSyncResultData.IsAuthenticationError && !this.isLatestToken);
						goto IL_174;
					}
					catch (InvalidServerResponseException ex)
					{
						exception = ex;
						goto IL_174;
					}
				}
				DeltaSyncClient.CountCommand(asyncResult2, host, false);
				bool isRetryable = deltaSyncResponse.IsRetryable;
				if (isRetryable)
				{
					exception = new DownloadTransientException(deltaSyncResponse.Exception);
				}
				else
				{
					exception = new DownloadPermanentException(deltaSyncResponse.Exception);
				}
				if (!string.IsNullOrEmpty(host))
				{
					EndPointHealth.UpdateDeltaSyncEndPointStatus(host, !isRetryable, this.syncLogSession);
				}
				IL_174:;
			}
			if (flag)
			{
				this.httpSessionConfig.RequestStream.Position = 0L;
				this.BeginRequest(asyncResult2, true);
				return;
			}
			this.HandleResult(asyncResult2, deltaSyncResultData, exception);
		}

		private void HandleResult(AsyncResult<DeltaSyncClient, DeltaSyncResultData> deltaSyncAsyncResult, DeltaSyncResultData deltaSyncResultData, Exception exception)
		{
			if (this.TryStopProcess())
			{
				this.AlternateWlidEndpointHandler.RestoreWlidEndpoint(this.AuthenticationClient);
				if (exception == null)
				{
					if (deltaSyncResultData.IsTopLevelOperationSuccessful)
					{
						this.syncLogSession.LogDebugging((TSLID)668UL, DeltaSyncClient.Tracer, "Delta Sync Request succeeded with Top Level Status Code: {0}", new object[]
						{
							deltaSyncResultData.TopLevelStatusCode
						});
					}
					else
					{
						this.syncLogSession.LogError((TSLID)1399UL, "DeltaSync Request Failed with Top Level Status Code:{0}, FaultCode:{1}, FaultString:{2}, FaultDetail:{3}.", new object[]
						{
							deltaSyncResultData.TopLevelStatusCode,
							deltaSyncResultData.FaultCode,
							deltaSyncResultData.FaultString,
							deltaSyncResultData.FaultDetail
						});
					}
					deltaSyncAsyncResult.ProcessCompleted(deltaSyncResultData);
					return;
				}
				this.syncLogSession.LogError((TSLID)669UL, DeltaSyncClient.Tracer, "Delta Sync Request failed with exception: {0}", new object[]
				{
					exception
				});
				deltaSyncAsyncResult.ProcessCompleted(exception);
			}
		}

		private bool TryStopProcess()
		{
			lock (this.syncRoot)
			{
				if (this.sessionClosed)
				{
					return false;
				}
				this.sessionClosed = true;
			}
			return true;
		}

		private void ThrowIfPending()
		{
			if (this.pendingAsyncResult != null)
			{
				throw new InvalidOperationException("async operation still pending");
			}
		}

		internal const string DeltaSyncAlternateWlidEndpointRegistryName = "DeltaSyncAlternateWlidEndpoint";

		private const string ApplicationID = "MicrosoftExchangeServer-DeltaSyncClient";

		private const string ExchangeHostedServicesUserAgent = "ExchangeHostedServices/1.0";

		private const int DefaultWindowSize = 2000;

		private const long NoDownloadLimit = -1L;

		private static readonly Trace Tracer = Microsoft.Exchange.Diagnostics.Components.Net.ExTraceGlobals.DeltaSyncClientTracer;

		private static readonly object serviceEndpointSyncLock = new object();

		private static volatile bool serviceEndpointsLoaded;

		private static Uri partnerSyncUri;

		private static Uri partnerSendUri;

		private static Uri partnerItemOperationsUri;

		private static Uri partnerSettingsUri;

		private static Uri passportSyncUri;

		private static Uri passportSendUri;

		private static Uri passportItemOperationsUri;

		private static Uri passportSettingsUri;

		private static Uri rstLiveEndpointUri;

		private static Uri partnerStatelessUri;

		private static Uri passportStatelessUri;

		private static string partnerClientToken;

		private readonly SyncLogSession syncLogSession;

		private HttpClient httpClient;

		private LiveIDAuthenticationClient authenticationClient;

		private volatile ICancelableAsyncResult pendingAsyncResult;

		private HttpSessionConfig httpSessionConfig;

		private object syncRoot = new object();

		private bool sessionClosed;

		private DeltaSyncUserAccount userAccount;

		private Stream requestStream;

		private bool isLatestToken;

		private DeltaSyncCommandType commandType;

		private DeltaSyncRequestGenerator deltaSyncRequestGenerator;

		private DeltaSyncResponseHandler deltaSyncResponseHandler;

		private AlternateWlidEndpointHandler alternateWlidEndpointHandler;

		private string deltaSyncCookie;

		private long maxDownloadSizePerMessage;

		private ExDateTime timeSent;

		private delegate void SetupRequest(Stream requestStream);

		private delegate void SetupGetRequest();
	}
}
