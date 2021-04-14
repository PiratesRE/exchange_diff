using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Caching;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class MailboxContextBase : DisposeTrackableBase, IMailboxContext, IDisposable, IEWSPartnerRequestContext
	{
		internal MailboxContextBase(UserContextKey key, string userAgent)
		{
			ExAssert.RetailAssert(key != null, "[MailboxContextBase::ctor] key is null");
			ExAssert.RetailAssert(!string.IsNullOrEmpty(userAgent), "[MailboxContextBase::ctor] userAgent is null");
			this.key = key;
			this.UserAgent = userAgent;
			this.syncRoot = new object();
			if (!Globals.Owa2ServerUnitTestsHook && !Globals.DisableBreadcrumbs)
			{
				this.breadcrumbBuffer = new BreadcrumbBuffer(Globals.MaxBreadcrumbs);
			}
		}

		public string UserAgent { get; private set; }

		public UserContextState State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		public UserContextKey Key
		{
			get
			{
				return this.key;
			}
		}

		public ExchangePrincipal ExchangePrincipal
		{
			get
			{
				return this.exchangePrincipal;
			}
			set
			{
				this.exchangePrincipal = value;
			}
		}

		public MailboxSession MailboxSession
		{
			get
			{
				if (!this.MailboxSessionLockedByCurrentThread())
				{
					throw new OwaInvalidOperationException("Operation is not allowed if mailbox lock is not held");
				}
				this.CreateMailboxSessionIfNeeded();
				return this.mailboxSession;
			}
		}

		public string UserPrincipalName
		{
			get
			{
				if (this.mailboxIdentity != null)
				{
					this.userPrincipalName = this.mailboxIdentity.GetOWAMiniRecipient().UserPrincipalName;
				}
				return this.userPrincipalName;
			}
		}

		public SmtpAddress PrimarySmtpAddress
		{
			get
			{
				if (this.mailboxIdentity != null)
				{
					return this.mailboxIdentity.GetOWAMiniRecipient().PrimarySmtpAddress;
				}
				return SmtpAddress.Empty;
			}
		}

		public OwaIdentity LogonIdentity
		{
			get
			{
				return this.logonIdentity;
			}
		}

		public OwaIdentity MailboxIdentity
		{
			get
			{
				return this.mailboxIdentity;
			}
		}

		public virtual AuthZClientInfo CallerClientInfo
		{
			get
			{
				SidBasedIdentity sidBasedIdentity = HttpContext.Current.User.Identity as SidBasedIdentity;
				ExAssert.RetailAssert(sidBasedIdentity != null, "identity is null");
				AuthZClientInfo result = null;
				if (this.callerClientInfo == null)
				{
					try
					{
						this.callerClientInfo = CallContextUtilities.GetCallerClientInfo();
					}
					catch (AuthzException ex)
					{
						ExTraceGlobals.CoreTracer.TraceError<string, string, AuthzException>(0L, "MailboxContextBase.CallerClientInfo could not be fetched for Name={0} AuthenticationType={1}. Exception: {2}", sidBasedIdentity.Name, sidBasedIdentity.AuthenticationType, ex);
						if (ex.InnerException is Win32Exception)
						{
							throw new OwaIdentityException("There was a problem getting the caller AuthZClientInfo", ex);
						}
						throw;
					}
				}
				if (this.callerClientInfo != null && this.callerClientInfo.UserIdentity.Sid.ToString() == sidBasedIdentity.Sid.ToString())
				{
					result = this.callerClientInfo;
				}
				return result;
			}
		}

		public virtual SessionDataCache SessionDataCache
		{
			get
			{
				throw new NotImplementedException("Session data cache is only supported on the primary user context");
			}
		}

		public INotificationManager NotificationManager
		{
			get
			{
				return this.notificationManager;
			}
		}

		public PendingRequestManager PendingRequestManager
		{
			get
			{
				return this.pendingRequestManager;
			}
		}

		public CacheItemRemovedReason AbandonedReason
		{
			get
			{
				return this.abandonedReason;
			}
			set
			{
				this.abandonedReason = value;
			}
		}

		public UserContextTerminationStatus TerminationStatus { get; set; }

		public bool IsExplicitLogon
		{
			get
			{
				return this.isExplicitLogon;
			}
		}

		public bool IsMailboxSessionCreated
		{
			get
			{
				return this.isMailboxSessionCreated;
			}
		}

		protected StringBuilder UserContextDiposeGraph
		{
			get
			{
				return this.userContextDiposeGraph;
			}
		}

		public bool LockAndReconnectMailboxSession(int timeout)
		{
			if (this.mailboxSessionLock.LockWriterElastic(timeout))
			{
				return UserContextUtilities.ReconnectStoreSession(this.MailboxSession, this);
			}
			throw new OwaLockTimeoutException("User context could not acquire the mailbox session writer lock", null, this);
		}

		public void UnlockAndDisconnectMailboxSession()
		{
			if (this.mailboxSessionLock.IsWriterLockHeld)
			{
				try
				{
					if (this.mailboxSession != null)
					{
						UserContextUtilities.DisconnectStoreSession(this.mailboxSession);
					}
				}
				finally
				{
					this.mailboxSessionLock.ReleaseWriterLock();
				}
			}
		}

		public bool MailboxSessionLockedByCurrentThread()
		{
			return this.mailboxSessionLock.IsWriterLockHeld;
		}

		public void DisconnectMailboxSession()
		{
			if (this.mailboxSession != null)
			{
				try
				{
					if (this.mailboxSessionLock.LockWriterElastic(3000))
					{
						UserContextUtilities.DisconnectStoreSession(this.mailboxSession);
					}
				}
				finally
				{
					if (this.mailboxSessionLock.IsWriterLockHeld)
					{
						this.mailboxSessionLock.ReleaseWriterLock();
					}
				}
			}
		}

		public override string ToString()
		{
			return this.GetHashCode().ToString();
		}

		public void Load(OwaIdentity logonIdentity, OwaIdentity mailboxIdentity, UserContextStatistics stats)
		{
			this.LogTrace("UserContextBase.Load", "starting");
			lock (this.syncRoot)
			{
				this.DoLoad(logonIdentity, mailboxIdentity, stats);
				this.AfterLoad();
			}
			this.LogTrace("UserContextBase.Load", "method finished");
		}

		protected virtual void DoLoad(OwaIdentity logonIdentity, OwaIdentity mailboxIdentity, UserContextStatistics stats)
		{
			if (logonIdentity == null)
			{
				throw new ArgumentNullException("logonIdentity");
			}
			this.logonIdentity = logonIdentity;
			if (mailboxIdentity != null)
			{
				this.isExplicitLogon = true;
				this.mailboxIdentity = mailboxIdentity;
			}
			else
			{
				this.mailboxIdentity = logonIdentity;
			}
			if (this.IsExplicitLogon)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Created partial mailbox identity from SMTP address={0}", mailboxIdentity.SafeGetRenderableName());
				OwaMiniRecipientIdentity owaMiniRecipientIdentity = this.mailboxIdentity as OwaMiniRecipientIdentity;
				try
				{
					owaMiniRecipientIdentity.UpgradePartialIdentity();
				}
				catch (DataValidationException ex)
				{
					PropertyValidationError propertyValidationError = ex.Error as PropertyValidationError;
					if (propertyValidationError == null || propertyValidationError.PropertyDefinition != MiniRecipientSchema.Languages)
					{
						throw;
					}
					OWAMiniRecipient owaminiRecipient = this.MailboxIdentity.FixCorruptOWAMiniRecipientCultureEntry();
					if (owaminiRecipient != null)
					{
						this.mailboxIdentity = OwaMiniRecipientIdentity.CreateFromOWAMiniRecipient(owaminiRecipient);
					}
				}
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.exchangePrincipal = this.mailboxIdentity.CreateExchangePrincipal();
			stats.ExchangePrincipalCreationTime = (int)stopwatch.ElapsedMilliseconds;
			this.LogTrace("UserContextBase.Load", "CreateExchangePrincipal finished");
			this.pendingRequestManager = new PendingRequestManager(this, ListenerChannelsManager.Instance);
		}

		private void AfterLoad()
		{
			this.notificationManager = this.GetNotificationManager(this);
		}

		protected virtual INotificationManager GetNotificationManager(MailboxContextBase mailboxContext)
		{
			return new OwaMapiNotificationManager(mailboxContext);
		}

		public void LogBreadcrumb(string message)
		{
			if (Globals.DisableBreadcrumbs)
			{
				return;
			}
			if (this.breadcrumbBuffer == null)
			{
				return;
			}
			this.breadcrumbBuffer.Add(new Breadcrumb(ExDateTime.UtcNow, (message != null) ? message : "<null>"));
		}

		public string DumpBreadcrumbs()
		{
			if (Globals.DisableBreadcrumbs)
			{
				return string.Empty;
			}
			if (this.breadcrumbBuffer == null)
			{
				return "<Breadcrumb buffer is null>";
			}
			StringBuilder stringBuilder = new StringBuilder(Globals.MaxBreadcrumbs * 128);
			stringBuilder.Append("OWA breadcrumbs:\r\n");
			this.breadcrumbBuffer.DumpTo(stringBuilder);
			return stringBuilder.ToString();
		}

		internal bool GetPendingGetManagerLock()
		{
			if (this.pendingGetManagerLock.IsWriterLockHeld)
			{
				throw new OwaInvalidOperationException("GetPendingGetManagerLock lock is already held by this thread");
			}
			return this.pendingGetManagerLock.LockWriterElastic(3000);
		}

		internal void ReleasePendingGetManagerLock()
		{
			if (this.pendingGetManagerLock.IsWriterLockHeld)
			{
				this.pendingGetManagerLock.ReleaseWriterLock();
			}
		}

		public MailboxSession CloneMailboxSession(string mailboxKey, ExchangePrincipal exchangePrincipal, IADOrgPerson person, ClientSecurityContext clientSecurityContext, GenericIdentity genericIdentity, bool unifiedLogon)
		{
			MailboxSession result = null;
			if (!string.IsNullOrEmpty(mailboxKey) && !base.IsDisposed && this.mailboxSession != null)
			{
				string text = AccessingPrincipalTiedCache.BuildKeyCacheKey(this.clonedMailboxProperties.MailboxGuid, this.clonedMailboxProperties.MailboxCulture, this.clonedMailboxProperties.LogonType);
				if (mailboxKey.Equals(text, StringComparison.InvariantCulture))
				{
					if (this.ExchangePrincipal != null && object.Equals(this.ExchangePrincipal.ObjectId, exchangePrincipal.ObjectId))
					{
						result = this.mailboxSession.CloneWithBestAccess(exchangePrincipal, person, clientSecurityContext, this.clonedMailboxProperties.ClientInfoString, genericIdentity, unifiedLogon);
					}
					else
					{
						ExTraceGlobals.UserContextCallTracer.TraceWarning(0L, string.Format("CloneMailboxSession: Mailbox owner not same, mailboxkey:{0}", text));
					}
				}
			}
			return result;
		}

		public abstract void ValidateLogonPermissionIfNecessary();

		protected void LogTrace(string methodName, string message)
		{
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("UserContext", this, methodName, message));
		}

		protected override void InternalDispose(bool isDisposing)
		{
			this.LogBreadcrumb("Entering InternalDispose");
			this.UserContextDiposeGraph.Append("1");
			this.LogMailboxContextDisposeTrace(OwaServerLogger.LoggerData.UserContextDipose1, this.UserContextDiposeGraph.ToString());
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "UserContext.InternalDispose Start");
			if (this.isInProcessOfDisposing)
			{
				this.UserContextDiposeGraph.Append(".2");
				return;
			}
			this.isInProcessOfDisposing = true;
			this.UserContextDiposeGraph.Append(".3");
			this.LogMailboxContextDisposeTrace(OwaServerLogger.LoggerData.UserContextDipose2, this.UserContextDiposeGraph.ToString());
			lock (this.syncRoot)
			{
				this.LogMailboxContextDisposeTrace(OwaServerLogger.LoggerData.MailboxSessionDuration, this.mailboxSessionCreationTime.ToString() + "--" + DateTime.UtcNow.ToString());
				this.UserContextDiposeGraph.Append(".4");
				ExTraceGlobals.UserContextCallTracer.TraceDebug<bool, MailboxContextBase>(0L, "UserContext.Dispose. IsDisposing: {0}, User context instance={1}", isDisposing, this);
				if (isDisposing && !this.isDisposed)
				{
					this.UserContextDiposeGraph.Append(".5");
					try
					{
						try
						{
							RemoteNotificationManager.Instance.CleanUpSubscriptions(this.Key.ToString());
							Stopwatch stopwatch = Stopwatch.StartNew();
							this.mailboxSessionLock.LockWriterElastic(15000);
							stopwatch.Stop();
							this.LogMailboxContextDisposeTrace(OwaServerLogger.LoggerData.MailboxSessionLockTime, stopwatch.ElapsedMilliseconds.ToString());
							this.UserContextDiposeGraph.Append(".6");
							this.DisposeMailboxSessionReferencingObjects();
						}
						catch (StoragePermanentException ex)
						{
							this.UserContextDiposeGraph.Append(".7");
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to dispose notification objects.  exception {0}", ex.Message);
						}
						catch (StorageTransientException ex2)
						{
							this.UserContextDiposeGraph.Append(".8");
							ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to dispose notification objects.  exception {0}", ex2.Message);
						}
						catch (Exception ex3)
						{
							this.UserContextDiposeGraph.Append(".9");
							ExTraceGlobals.UserContextTracer.TraceError<string>(0L, "Unable to dispose notification objects.  exception {0}", ex3.Message);
							throw;
						}
						finally
						{
							try
							{
								if (this.mailboxSession != null)
								{
									this.UserContextDiposeGraph.Append(".10");
									if (this.MailboxSessionLockedByCurrentThread() || this.mailboxSessionLock.LockWriterElastic(15000))
									{
										this.UserContextDiposeGraph.Append(".11");
										this.DisposeMailboxSession();
									}
									else
									{
										this.UserContextDiposeGraph.Append(".12");
										ExTraceGlobals.UserContextTracer.TraceError(0L, "Outlook Web App failed to obtain mailbox session lock to dispose of the mailbox session.");
									}
								}
							}
							finally
							{
								if (this.mailboxSessionLock.IsWriterLockHeld)
								{
									this.UserContextDiposeGraph.Append(".13");
									this.mailboxSessionLock.ReleaseWriterLock();
								}
							}
						}
					}
					finally
					{
						this.UserContextDiposeGraph.Append(".14");
						this.isDisposed = true;
						this.DisposeNonMailboxSessionReferencingObjects();
						this.LogMailboxContextDisposeTrace(OwaServerLogger.LoggerData.UserContextDipose3, this.UserContextDiposeGraph.ToString());
						this.UserContextDiposeGraph.Clear();
					}
				}
			}
			this.LogMailboxContextDisposeTrace(OwaServerLogger.LoggerData.UserContextDipose4, this.UserContextDiposeGraph.ToString());
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "UserContext.InternalDispose End");
		}

		protected virtual void DisposeMailboxSessionReferencingObjects()
		{
			if (this.notificationManager != null)
			{
				this.UserContextDiposeGraph.Append(".c1");
				this.notificationManager.Dispose();
				this.notificationManager = null;
			}
		}

		protected virtual void DisposeNonMailboxSessionReferencingObjects()
		{
			this.UserContextDiposeGraph.Append(".b1");
			try
			{
				if (this.GetPendingGetManagerLock())
				{
					this.UserContextDiposeGraph.Append(".b2");
					if (this.pendingRequestManager != null)
					{
						this.UserContextDiposeGraph.Append(".b3");
						this.pendingRequestManager.Dispose();
						this.pendingRequestManager = null;
					}
				}
				else
				{
					this.UserContextDiposeGraph.Append(".b4");
					this.pendingRequestManager.ShouldDispose = true;
				}
			}
			finally
			{
				this.ReleasePendingGetManagerLock();
			}
			if (this.logonIdentity != null)
			{
				this.UserContextDiposeGraph.Append(".b5");
				this.logonIdentity.Dispose();
				this.logonIdentity = null;
			}
			if (this.mailboxIdentity != null)
			{
				this.UserContextDiposeGraph.Append(".b6");
				this.mailboxIdentity.Dispose();
				this.mailboxIdentity = null;
			}
			if (this.callerClientInfo != null)
			{
				this.UserContextDiposeGraph.Append(".b7");
				this.callerClientInfo.Dispose();
				this.callerClientInfo = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MailboxContextBase>(this);
		}

		protected abstract MailboxSession CreateMailboxSession();

		protected void InternalRetireMailboxSession()
		{
			ExTraceGlobals.UserContextTracer.TraceDebug((long)this.GetHashCode(), "InternalRetireMailboxSession: Start");
			if (this.mailboxSessionLock.LockWriterElastic(3000))
			{
				try
				{
					try
					{
						if (this.mailboxSession != null)
						{
							this.mailboxSession.Dispose();
							this.mailboxSession = null;
							this.isMailboxSessionCreated = false;
							this.mailboxSessionCreationTime = DateTime.MinValue;
							if (this.notificationManager != null)
							{
								this.notificationManager.HandleConnectionDroppedNotification();
							}
							ExTraceGlobals.UserContextTracer.TraceDebug((long)this.GetHashCode(), "InternalRetireMailboxSession: Disposed mailbox session");
						}
					}
					catch (Exception ex)
					{
						ExTraceGlobals.UserContextTracer.TraceError<string>((long)this.GetHashCode(), "InternalRetireMailboxSession: Failed to retire mailbox. exception {0}", ex.Message);
						throw;
					}
					return;
				}
				finally
				{
					this.mailboxSessionLock.ReleaseWriterLock();
				}
			}
			throw new OwaLockTimeoutException("UserContext::InternalRetireMailboxSession could not acquire the mailbox session writer lock", null, this);
		}

		protected void CreateMailboxSessionIfNeeded()
		{
			if (this.isMailboxSessionCreated)
			{
				return;
			}
			bool flag = false;
			try
			{
				if (!this.mailboxSessionLock.IsWriterLockHeld)
				{
					flag = this.mailboxSessionLock.LockWriterElastic(3000);
					if (!flag)
					{
						throw new OwaLockTimeoutException("UserContext::CreateMailboxSessionIfNeeded could not acquire the mailbox session writer lock", null, this);
					}
				}
				if (!this.isMailboxSessionCreated)
				{
					this.mailboxSession = this.CreateMailboxSession();
					this.isMailboxSessionCreated = true;
					this.mailboxSessionCreationTime = DateTime.UtcNow;
					this.clonedMailboxProperties = new MailboxContextBase.ClonedMailboxProperties(this.mailboxSession.LogonType, this.mailboxSession.MailboxGuid, this.mailboxSession.Culture, this.mailboxSession.ClientInfoString);
				}
			}
			finally
			{
				if (flag)
				{
					this.mailboxSessionLock.ReleaseWriterLock();
				}
			}
		}

		protected void DisposeMailboxSession()
		{
			this.UserContextDiposeGraph.Append(".a1");
			if (this.mailboxSession != null)
			{
				this.UserContextDiposeGraph.Append(".a2");
				try
				{
					UserContextUtilities.DisconnectStoreSession(this.mailboxSession);
					this.mailboxSession.Dispose();
				}
				catch (StoragePermanentException ex)
				{
					this.UserContextDiposeGraph.Append(".a3");
					ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to disconnect mailbox session.  exception {0}", ex.Message);
				}
				catch (StorageTransientException ex2)
				{
					this.UserContextDiposeGraph.Append(".a4");
					ExTraceGlobals.UserContextTracer.TraceDebug<string>(0L, "Unable to disconnect mailbox session.  exception {0}", ex2.Message);
				}
				catch (Exception ex3)
				{
					this.UserContextDiposeGraph.Append(".a5");
					ExTraceGlobals.UserContextTracer.TraceError<string>(0L, "Unable to to disconnect mailbox session.  exception {0}", ex3.Message);
					throw;
				}
				finally
				{
					this.UserContextDiposeGraph.Append(".a6");
					this.mailboxSession = null;
				}
			}
		}

		protected void LogMailboxContextDisposeTrace(OwaServerLogger.LoggerData traceId, string trace)
		{
			OwaServerTraceLogger.AppendToLog(new TraceLogEvent("MailboxContextDispose-V1", this, null, traceId + "=" + trace));
		}

		internal const string UserContextEventId = "UserContext";

		protected object syncRoot;

		protected MailboxSession mailboxSession;

		protected OwaRWLockWrapper mailboxSessionLock = new OwaRWLockWrapper();

		protected bool isMailboxSessionCreated;

		protected bool isInProcessOfDisposing;

		private bool isDisposed;

		private UserContextState state;

		private CacheItemRemovedReason abandonedReason;

		private OwaRWLockWrapper pendingGetManagerLock = new OwaRWLockWrapper();

		private UserContextKey key;

		private ExchangePrincipal exchangePrincipal;

		private INotificationManager notificationManager;

		private PendingRequestManager pendingRequestManager;

		private OwaIdentity logonIdentity;

		private OwaIdentity mailboxIdentity;

		private string userPrincipalName = string.Empty;

		private bool isExplicitLogon;

		private DateTime mailboxSessionCreationTime;

		private StringBuilder userContextDiposeGraph = new StringBuilder(256);

		private BreadcrumbBuffer breadcrumbBuffer;

		private AuthZClientInfo callerClientInfo;

		private MailboxContextBase.ClonedMailboxProperties clonedMailboxProperties;

		internal class ClonedMailboxProperties
		{
			public LogonType LogonType { get; private set; }

			public Guid MailboxGuid { get; private set; }

			public CultureInfo MailboxCulture { get; private set; }

			public string ClientInfoString { get; private set; }

			public ClonedMailboxProperties(LogonType logonType, Guid mailboxGuid, CultureInfo mailboxCulture, string clientInfoString)
			{
				this.LogonType = logonType;
				this.MailboxGuid = mailboxGuid;
				this.MailboxCulture = mailboxCulture;
				this.ClientInfoString = clientInfoString;
			}
		}
	}
}
