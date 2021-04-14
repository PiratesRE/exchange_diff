using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Handler.StorageObjects;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class Logon : PropertyServerObject
	{
		protected Logon(StoreSession session, ClientSecurityContext delegatedClientSecurityContext, ConnectionHandler connectionHandler, NotificationHandler notificationHandler, OpenFlags openFlags, byte logonId, ProtocolLogLogonType protocolLogLogonType) : this(session, delegatedClientSecurityContext, connectionHandler, notificationHandler, openFlags, logonId, protocolLogLogonType, ClientSideProperties.LogonInstance, PropertyConverter.Logon)
		{
		}

		protected Logon(StoreSession session, ClientSecurityContext delegatedClientSecurityContext, ConnectionHandler connectionHandler, NotificationHandler notificationHandler, OpenFlags openFlags, byte logonId, ProtocolLogLogonType protocolLogLogonType, ClientSideProperties clientSideProperties, PropertyConverter converter) : base(clientSideProperties, converter)
		{
			this.connectionHandler = connectionHandler;
			this.logonId = logonId;
			this.notificationHandler = notificationHandler;
			this.protocolLogLogonType = protocolLogLogonType;
			this.delegatedClientSecurityContext = delegatedClientSecurityContext;
			this.propertyDefinitionFactory = new CoreObjectPropertyDefinitionFactory(session, session.Mailbox.CoreObject.PropertyBag);
			this.storageObjectProperties = new CoreObjectProperties(session.Mailbox.CoreObject.PropertyBag);
			this.supportProgressForSetReadFlags = true;
			if (connectionHandler.Connection.ClientInformation.Version >= MapiVersion.Outlook14 && (openFlags & OpenFlags.SupportProgress) != OpenFlags.SupportProgress)
			{
				this.supportProgressForSetReadFlags = false;
			}
			this.connectionDropSubscription = Subscription.CreateMailboxSubscription(session, new NotificationHandler(this.BackEndConnectionDropNotificationHandler), NotificationType.ConnectionDropped);
			this.fastTransferActivityThrottle = PerServerActivityThrottle<RopFastTransferSourceGetBuffer>.GetPerServerActivityThrottle(session.ServerFullyQualifiedDomainName);
			if (!String8Encodings.TryGetEncoding(this.Connection.CodePageId, out this.string8Encoding))
			{
				string message = string.Format("Cannot resolve code page: {0}", this.Connection.CultureInfo.TextInfo.ANSICodePage);
				throw new RopExecutionException(message, ErrorCode.UnknownCodepage);
			}
		}

		public override Schema Schema
		{
			get
			{
				return MailboxSchema.Instance;
			}
		}

		internal IConnection Connection
		{
			get
			{
				return this.connectionHandler.Connection;
			}
		}

		internal byte LogonId
		{
			get
			{
				return this.logonId;
			}
		}

		internal Encoding LogonString8Encoding
		{
			get
			{
				return this.string8Encoding;
			}
		}

		internal IResourceTracker ResourceTracker
		{
			get
			{
				if (this.resourceTracker == null)
				{
					this.resourceTracker = this.CreateResourceTracker();
				}
				return this.resourceTracker;
			}
		}

		internal bool HasActiveAsyncOperation
		{
			get
			{
				return this.hasActiveAsyncOperation;
			}
		}

		internal bool SupportProgressForSetReadFlags
		{
			get
			{
				return this.supportProgressForSetReadFlags;
			}
		}

		protected ClientSecurityContext DelegatedClientSecurityContext
		{
			get
			{
				return this.delegatedClientSecurityContext;
			}
		}

		internal static void ValidateLogonSettings(LogonFlags logonFlags, OpenFlags openFlags, MailboxId? mailboxId)
		{
			if (!Logon.AreSupportedOpenFlags(openFlags))
			{
				throw new RopExecutionException("Unsupported openFlags.", (ErrorCode)2147746050U);
			}
		}

		internal static ExchangePrincipal FindExchangePrincipal(IConnection connection, MailboxId mailboxId)
		{
			ExchangePrincipal result;
			try
			{
				TestInterceptor.Intercept(TestInterceptorLocation.Logon_FindExchangePrincipal, new object[0]);
				if (mailboxId.IsLegacyDn)
				{
					result = connection.FindExchangePrincipalByLegacyDN(mailboxId.LegacyDn);
				}
				else
				{
					result = ExchangePrincipal.FromLocalServerMailboxGuid(connection.OrganizationId.ToADSessionSettings(), mailboxId.DatabaseGuid, mailboxId.MailboxGuid);
				}
			}
			catch (ObjectNotFoundException innerException)
			{
				throw new RopExecutionException(string.Format("Couldn't find a mailbox for {0} in the current forest. The client should attempt to re-discover it.", mailboxId), ErrorCode.UnknownUser, innerException);
			}
			return result;
		}

		internal static LegacyDN CreatePersonalizedServerRedirectLegacyDN(ExchangePrincipal exchangePrincipal)
		{
			LegacyDN legacyDN;
			if (!LegacyDN.TryParse(exchangePrincipal.MailboxInfo.Location.RpcClientAccessServerLegacyDn, out legacyDN))
			{
				throw new RopExecutionException(string.Format("Invalid RpcClientAccessServerLegacyDistinguishedName property {0}", exchangePrincipal.MailboxInfo.Location.RpcClientAccessServerLegacyDn), ErrorCode.ADPropertyError);
			}
			return ExchangeRpcClientAccess.CreatePersonalizedServerRedirectLegacyDN(legacyDN, exchangePrincipal.MailboxInfo.MailboxGuid, exchangePrincipal.MailboxInfo.PrimarySmtpAddress.Domain);
		}

		internal abstract StoreId[] GetDefaultFolderIds();

		public void AbortSubmit(StoreId folderId, StoreId messageId)
		{
			this.Session.AbortSubmit(this.Session.IdConverter.CreateMessageId(folderId, messageId));
		}

		internal NotificationQueue NotificationQueue
		{
			get
			{
				if (this.notificationQueue == null)
				{
					this.notificationQueue = new NotificationQueue(this);
				}
				return this.notificationQueue;
			}
		}

		internal void LogLogoff()
		{
			ProtocolLog.LogLogoff(this.protocolLogLogonType, (int)this.LogonId);
		}

		internal IDisposable GetFastTransferActivityLock(RopId rop)
		{
			IDisposable result = null;
			ExDateTime utcNow = ExDateTime.UtcNow;
			uint num;
			if (this.fastTransferRejectionEnd > utcNow)
			{
				num = (uint)(this.fastTransferRejectionEnd - utcNow).TotalMilliseconds;
				throw new ClientBackoffException("This logon is already under a Fast Transfer get buffer backoff, so continue the backoff.", this.LogonId, 0U, new BackoffRopData[]
				{
					new BackoffRopData(rop, num)
				}, Array<byte>.Empty);
			}
			if (this.fastTransferActivityThrottle.TryGetActivityLock(this.fastTransferRejectionCount >= Configuration.ServiceConfiguration.FastTransferBackoffRetryCount || this.Connection.ClientInformation.Mode == ClientMode.ExchangeServer, Configuration.ServiceConfiguration.FastTransferMaxRequests, out result))
			{
				this.fastTransferRejectionCount = 0;
				return result;
			}
			this.fastTransferRejectionCount++;
			num = (uint)(this.fastTransferRejectionCount * Configuration.ServiceConfiguration.FastTransferBackoffInterval);
			this.fastTransferRejectionEnd = utcNow.AddMilliseconds(num);
			throw new ClientBackoffException("Too many active Fast Transfer get buffer requests to same backend.", this.LogonId, 0U, new BackoffRopData[]
			{
				new BackoffRopData(rop, num)
			}, Array<byte>.Empty);
		}

		internal Rights GetEffectiveRights(byte[] addressBookId, StoreId folderId)
		{
			StoreObjectId folderId2 = this.Session.IdConverter.CreateFolderId(folderId);
			return (Rights)this.Session.GetEffectiveRights(addressBookId, folderId2);
		}

		internal IAsyncOperationExecutor AsyncOperationExecutor
		{
			get
			{
				return this.asyncOperationExecutor;
			}
		}

		internal IAsyncOperationExecutor CreateAsyncOperationExecutor(SegmentedRopOperation segmentedRopOperation, object progressToken)
		{
			AsyncOperationExecutor asyncOperationExecutor = this.asyncOperationExecutor;
			if (asyncOperationExecutor != null)
			{
				if (!asyncOperationExecutor.IsCompleted)
				{
					string message = string.Format("This Logon has an AsyncOperationExecutor: {0}. It's not clear for creating a new async operation executor", asyncOperationExecutor.ToString());
					throw new RopExecutionException(message, ErrorCode.InvalidOperation);
				}
				this.RemoveAsyncExecutor();
			}
			this.asyncOperationExecutor = new AsyncOperationExecutor(segmentedRopOperation, progressToken, new Action(this.AsyncOperationDoneCallback));
			this.hasActiveAsyncOperation = true;
			return this.asyncOperationExecutor;
		}

		internal void RemoveAsyncExecutor()
		{
			AsyncOperationExecutor asyncOperationExecutor = this.asyncOperationExecutor;
			if (asyncOperationExecutor != null)
			{
				asyncOperationExecutor.Dispose();
				this.asyncOperationExecutor = null;
				this.hasActiveAsyncOperation = false;
			}
		}

		private static bool AreSupportedOpenFlags(OpenFlags openFlags)
		{
			return (openFlags & OpenFlags.Transport) != OpenFlags.Transport && (openFlags & OpenFlags.InternetAnonymous) != OpenFlags.InternetAnonymous && (openFlags & OpenFlags.CallbackLogon) != OpenFlags.CallbackLogon && (openFlags & OpenFlags.Local) != OpenFlags.Local && (openFlags & OpenFlags.DeliverNormalMessage) != OpenFlags.DeliverNormalMessage && (openFlags & OpenFlags.DeliverQuotaMessage) != OpenFlags.DeliverQuotaMessage && (openFlags & OpenFlags.DeliverSpecialMessage) != OpenFlags.DeliverSpecialMessage;
		}

		private void BackEndConnectionDropNotificationHandler(Notification notification)
		{
			this.Connection.ExecuteInContext<Notification>(notification, delegate(Notification innerNotification)
			{
				ExTraceGlobals.NotificationHandlerTracer.TraceDebug<IConnection, NotificationType>(Activity.TraceId, "BackEndConnectionDropNotificationHandler. Connection = {0}.", this.Connection, innerNotification.Type);
				this.Connection.MarkAsDeadAndDropAllAsyncCalls();
			});
		}

		protected static T CreateStoreSession<T>(IConnection connection, OpenFlags openFlags, LogonExtendedRequestFlags extendedFlags, ExchangePrincipal exchangeMailboxPrincipal, ClientSecurityContext delegatedClientSecurityContext, string applicationId, Func<ExchangePrincipal, ClientSecurityContext, OpenFlags, IConnection, string, T> createSessionDelegate) where T : StoreSession
		{
			string arg = ((extendedFlags & LogonExtendedRequestFlags.ApplicationId) == LogonExtendedRequestFlags.ApplicationId) ? string.Format("{0};{1}", "Client=MSExchangeRPC", applicationId) : "Client=MSExchangeRPC";
			T result = default(T);
			try
			{
				TestInterceptor.Intercept(TestInterceptorLocation.Logon_CreateStoreSession, new object[0]);
				result = createSessionDelegate(exchangeMailboxPrincipal, delegatedClientSecurityContext, openFlags, connection, arg);
			}
			catch (AccessDeniedException innerException)
			{
				throw new RopExecutionException(string.Format("User '{0}' acting as '{1}' doesn't have permissions to logon to/act as mailbox '{2}'.", connection.AccessingClientSecurityContext.UserSid, connection.ActAsLegacyDN, exchangeMailboxPrincipal.LegacyDn), ErrorCode.LoginPerm, innerException);
			}
			catch (WrongServerException innerException2)
			{
				connection.InvalidateCachedUserInfo();
				throw new ClientBackoffException("Mailbox was moved to a different mailbox server. A client needs to retry.", innerException2);
			}
			catch (ConnectionFailedPermanentException innerException3)
			{
				connection.InvalidateCachedUserInfo();
				throw new RopExecutionException("Mailbox is not available", ErrorCode.MdbOffline, innerException3);
			}
			catch (ConnectionFailedTransientException ex)
			{
				connection.InvalidateCachedUserInfo();
				if (!ExceptionTranslator.IsConnectionToStoreDead(ex))
				{
					throw;
				}
				throw new RopExecutionException("Mailbox is temporarily unavailable", ErrorCode.MdbOffline, ex);
			}
			return result;
		}

		private void AsyncOperationDoneCallback()
		{
			this.hasActiveAsyncOperation = false;
			if (this.notificationQueue != null && !this.notificationQueue.IsEmpty)
			{
				this.notificationHandler.InvokeCallback();
			}
		}

		protected abstract IResourceTracker CreateResourceTracker();

		protected override IPropertyDefinitionFactory PropertyDefinitionFactory
		{
			get
			{
				return this.propertyDefinitionFactory;
			}
		}

		protected override IStorageObjectProperties StorageObjectProperties
		{
			get
			{
				return this.storageObjectProperties;
			}
		}

		protected ICorePropertyBag PropertyBag
		{
			get
			{
				return this.Session.Mailbox.CoreObject.PropertyBag;
			}
		}

		public override void ClearCacheIfNeededForGetProperties()
		{
			this.PropertyBag.Clear();
		}

		public override PropertyProblem[] SaveAndGetPropertyProblems(NativeStorePropertyDefinition[] propertyDefinitions, PropertyTag[] propertyTags)
		{
			FolderSaveResult folderSaveResult = ((CoreMailboxObject)this.Session.Mailbox.CoreObject).Save();
			return Folder.ConvertFolderSaveResultToProblems(folderSaveResult, propertyDefinitions, propertyTags);
		}

		protected override void InternalDispose()
		{
			Util.DisposeIfPresent(this.notificationQueue);
			Util.DisposeIfPresent(this.connectionDropSubscription);
			Util.DisposeIfPresent(this.asyncOperationExecutor);
			Util.DisposeIfPresent(this.delegatedClientSecurityContext);
			base.InternalDispose();
		}

		private const string RpcClientAccessApplicationId = "Client=MSExchangeRPC";

		private const string CompositeApplicationId = "{0};{1}";

		internal const LogonFlags LogonFlagsResultMask = LogonFlags.Private | LogonFlags.Undercover | LogonFlags.Ghosted;

		protected const int DefaultMaxUserMessageSizeInKBytes = 10240;

		private readonly byte logonId;

		private readonly ConnectionHandler connectionHandler;

		private readonly Encoding string8Encoding;

		private readonly ProtocolLogLogonType protocolLogLogonType;

		private readonly PerServerActivityThrottle<RopFastTransferSourceGetBuffer> fastTransferActivityThrottle;

		private readonly bool supportProgressForSetReadFlags;

		private AsyncOperationExecutor asyncOperationExecutor;

		private bool hasActiveAsyncOperation;

		private Subscription connectionDropSubscription;

		private NotificationQueue notificationQueue;

		private NotificationHandler notificationHandler;

		private ClientSecurityContext delegatedClientSecurityContext;

		private int fastTransferRejectionCount;

		private ExDateTime fastTransferRejectionEnd = ExDateTime.UtcNow;

		private IResourceTracker resourceTracker;

		private readonly CoreObjectPropertyDefinitionFactory propertyDefinitionFactory;

		private readonly CoreObjectProperties storageObjectProperties;
	}
}
