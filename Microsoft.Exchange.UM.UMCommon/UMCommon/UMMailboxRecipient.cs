using System;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class UMMailboxRecipient : UMRecipient
	{
		public UMMailboxRecipient(ADRecipient adrecipient)
		{
			this.Initialize(adrecipient, true);
		}

		public UMMailboxRecipient(ADRecipient adrecipient, MailboxSession mbxSession)
		{
			this.Initialize(adrecipient, mbxSession, true);
		}

		protected UMMailboxRecipient()
		{
		}

		public ADUser ADUser
		{
			get
			{
				return this.aduser;
			}
		}

		public IConfigurationFolder ConfigFolder
		{
			get
			{
				return this.configFolder;
			}
		}

		public virtual CultureInfo TelephonyCulture
		{
			get
			{
				return null;
			}
		}

		public virtual CultureInfo MessageSubmissionCulture
		{
			get
			{
				return this.messageSubmissionCulture ?? CommonConstants.DefaultCulture;
			}
		}

		public string HomeServerLegacyDN
		{
			get
			{
				return this.ADUser.ServerLegacyDN;
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return this.ADUser.LegacyExchangeDN;
			}
		}

		public ExchangePrincipal ExchangePrincipal
		{
			get
			{
				return base.InternalExchangePrincipal;
			}
		}

		public bool HasContactsFolder
		{
			get
			{
				this.EnsureLazyMailboxInitialization();
				return this.lazyHasContactsFolder;
			}
		}

		public bool HasCalendarFolder
		{
			get
			{
				this.EnsureLazyMailboxInitialization();
				return this.lazyHasCalendarFolder;
			}
		}

		public bool HasDraftsFolder
		{
			get
			{
				this.EnsureLazyMailboxInitialization();
				return this.lazyHasDraftsFolder;
			}
		}

		public ExDateTime Now
		{
			get
			{
				this.EnsureLazyMailboxInitialization();
				return ExDateTime.GetNow(this.lazyTimeZone);
			}
		}

		public ExTimeZone TimeZone
		{
			get
			{
				this.EnsureLazyMailboxInitialization();
				return this.lazyTimeZone;
			}
		}

		public CultureInfo[] PreferredCultures
		{
			get
			{
				return this.preferredCultures;
			}
		}

		private MailboxSession UnsafeMailboxSession
		{
			get
			{
				base.CheckDisposed();
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "UmUser(#{0})::UnsafeMailboxSession()", new object[]
				{
					this.GetHashCode()
				});
				if (!CommonUtil.IsServerCompatible(this.ExchangePrincipal.MailboxInfo.Location.ServerVersion))
				{
					throw new LegacyUmUserException(this.ExchangeLegacyDN);
				}
				this.EnsureLazyMailboxInitialization();
				return this.lazyMailboxSession;
			}
		}

		public new static bool TryCreate(ADRecipient adrecipient, out UMRecipient umrecipient)
		{
			UMMailboxRecipient ummailboxRecipient = new UMMailboxRecipient();
			if (ummailboxRecipient.Initialize(adrecipient, false))
			{
				umrecipient = ummailboxRecipient;
				return true;
			}
			ummailboxRecipient.Dispose();
			umrecipient = null;
			return false;
		}

		public bool IsMailboxQuotaExceeded()
		{
			bool result = false;
			try
			{
				result = XsoUtil.IsMailboxQuotaExceeded(this, 2097152U);
			}
			catch (LocalizedException ex)
			{
				PIIMessage data = PIIMessage.Create(PIIType._User, this.ToString());
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, data, "Exception trying to find out if the mailbox quota was exceeded for user=_User. e={0}", new object[]
				{
					ex
				});
			}
			return result;
		}

		public UMMailboxRecipient.MailboxSessionLock CreateSessionLock()
		{
			return new UMMailboxRecipient.MailboxSessionLock(this);
		}

		public UMMailboxRecipient.MailboxConnectionGuard CreateConnectionGuard()
		{
			return new UMMailboxRecipient.MailboxConnectionGuard(this);
		}

		protected override bool Initialize(ADRecipient recipient, bool throwOnFailure)
		{
			bool flag = false;
			try
			{
				if (!base.Initialize(recipient, throwOnFailure))
				{
					return flag;
				}
				this.aduser = (recipient as ADUser);
				if (!base.CheckField(this.aduser, "aduser", UMRecipient.FieldMissingCheck, throwOnFailure))
				{
					return flag;
				}
				if (!base.CheckField(this.ExchangeLegacyDN, "ExchangeLegacyDN", UMRecipient.FieldMissingCheck, throwOnFailure))
				{
					return flag;
				}
				if (!base.CheckField(this.HomeServerLegacyDN, "HomeServerLegacyDN", UMRecipient.FieldMissingCheck, throwOnFailure))
				{
					return flag;
				}
				if (!CommonConstants.UseDataCenterCallRouting)
				{
					if (!base.CheckField(base.InternalIsIncompatibleMailboxUser, "IncompatibleMailboxUser", (object o) => !base.InternalIsIncompatibleMailboxUser, throwOnFailure))
					{
						return flag;
					}
				}
				if (!base.CheckField(this.MailAddress, "MailAddress", UMRecipient.FieldMissingCheck, throwOnFailure))
				{
					return flag;
				}
				if (!base.CheckField(base.DisplayName, "DisplayName", UMRecipient.FieldMissingCheck, throwOnFailure))
				{
					return flag;
				}
				this.configFolder = new XsoConfigurationFolder(this);
				if (!base.CheckField(this.configFolder, "ConfigurationFolder", UMRecipient.FieldMissingCheck, throwOnFailure))
				{
					return flag;
				}
				if (this.aduser.Languages == null)
				{
					this.preferredCultures = new CultureInfo[0];
				}
				else
				{
					this.preferredCultures = this.aduser.Languages.ToArray();
				}
				this.InitMessageSubmissionCulture();
				if (!base.CheckField(this.MessageSubmissionCulture, "MessageSubmissionCulture", UMRecipient.FieldMissingCheck, throwOnFailure))
				{
					return flag;
				}
				this.clientOwnsMailboxSession = false;
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
			return flag;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UMMailboxRecipient>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "UmUser(#{0})::Dispose()", new object[]
					{
						this.GetHashCode()
					});
					if (!this.clientOwnsMailboxSession)
					{
						lock (this.sessionLock)
						{
							if (this.lazyMailboxSession != null)
							{
								CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "UmUser(#{0})::Dispose() disposing Mailbox Connection", new object[]
								{
									this.GetHashCode()
								});
								this.lazyMailboxSession.Dispose();
								this.lazyMailboxSession = null;
							}
						}
					}
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected virtual bool Initialize(ADRecipient recipient, MailboxSession mbxSession, bool throwOnFailure)
		{
			bool flag = false;
			if (!this.Initialize(recipient, throwOnFailure))
			{
				return flag;
			}
			try
			{
				this.clientOwnsMailboxSession = true;
				this.InitializeLazyMailboxSession(mbxSession);
				if (!base.CheckField(this.lazyMailboxSession, "lazyMailboxSession", UMRecipient.FieldMissingCheck, throwOnFailure))
				{
					return flag;
				}
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
			return flag;
		}

		private void InitMessageSubmissionCulture()
		{
			if (this.PreferredCultures == null || this.PreferredCultures.Length < 1)
			{
				return;
			}
			this.messageSubmissionCulture = UmCultures.GetPreferredClientCulture(this.PreferredCultures);
			if (this.messageSubmissionCulture == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Could not find a suitable client culture, falling back to default.", new object[0]);
				return;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "Initialized message submission Culture ={0}.", new object[]
			{
				this.messageSubmissionCulture
			});
		}

		private void EnsureLazyMailboxInitialization()
		{
			if (this.lazyMailboxSession == null)
			{
				lock (this.sessionLock)
				{
					if (this.lazyMailboxSession == null)
					{
						this.InitializeLazyMailboxSession(MailboxSessionEstablisher.OpenAsAdmin(this.ExchangePrincipal, this.TelephonyCulture ?? this.MessageSubmissionCulture, "Client=UM"));
					}
				}
			}
		}

		private void InitializeLazyMailboxSession(MailboxSession mbxSession)
		{
			this.lazyMailboxSession = mbxSession;
			this.lazyTimeZone = CommonUtil.GetOwaTimeZone(this.lazyMailboxSession);
			this.lazyMailboxSession.ExTimeZone = this.lazyTimeZone;
			if (!this.MessageSubmissionCulture.IsNeutralCulture)
			{
				this.MessageSubmissionCulture.DateTimeFormat.ShortTimePattern = CommonUtil.GetOwaTimeFormat(this.lazyMailboxSession);
			}
			this.lazyHasContactsFolder = (this.lazyMailboxSession.GetDefaultFolderId(DefaultFolderType.Contacts) != null);
			this.lazyHasCalendarFolder = (this.lazyMailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar) != null);
			this.lazyHasDraftsFolder = (this.lazyMailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts) != null);
		}

		private void AcquireMailboxSession(bool withLock)
		{
			PIIMessage data = PIIMessage.Create(PIIType._User, this.ExchangeLegacyDN);
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, data, "UmUser(#{0})::AcquireMailboxSession called for user=_User, lock={1}, threadId={2}, guardCount={3}", new object[]
			{
				this.GetHashCode(),
				withLock,
				Thread.CurrentThread.ManagedThreadId,
				this.sessionGuardCount
			});
			lock (this.sessionGuardCountLock)
			{
				this.sessionGuardCount++;
			}
			if (withLock)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "UmUser(#{0})::AcquireMailboxSession Acquiring Lock", new object[]
				{
					this.GetHashCode()
				});
				Monitor.Enter(this.sessionLock);
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "UmUser(#{0})::AcquireMailboxSession Acquired Lock", new object[]
				{
					this.GetHashCode()
				});
			}
		}

		private void ReleaseMailboxSession(bool withLock)
		{
			PIIMessage data = PIIMessage.Create(PIIType._User, this.ExchangeLegacyDN);
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, data, "UmUser(#{0})::ReleaseMailboxSession called for user=_User, lock={1}, threadId={2}, guardCount={3}", new object[]
			{
				this.GetHashCode(),
				withLock,
				Thread.CurrentThread.ManagedThreadId,
				this.sessionGuardCount
			});
			try
			{
				lock (this.sessionGuardCountLock)
				{
					this.sessionGuardCount--;
					if (this.sessionGuardCount == 0 && this.lazyMailboxSession != null && this.lazyMailboxSession.IsConnected && !this.clientOwnsMailboxSession)
					{
						this.lazyMailboxSession.Disconnect();
					}
				}
			}
			finally
			{
				if (withLock)
				{
					Monitor.Exit(this.sessionLock);
				}
			}
		}

		private ADUser aduser;

		private MailboxSession lazyMailboxSession;

		private object sessionLock = new object();

		private int sessionGuardCount;

		private object sessionGuardCountLock = new object();

		private ExTimeZone lazyTimeZone;

		private bool lazyHasContactsFolder;

		private bool lazyHasCalendarFolder;

		private bool lazyHasDraftsFolder;

		private CultureInfo messageSubmissionCulture;

		private IConfigurationFolder configFolder;

		private bool clientOwnsMailboxSession;

		private CultureInfo[] preferredCultures;

		internal class MailboxConnectionGuard : DisposableBase
		{
			internal MailboxConnectionGuard(UMMailboxRecipient u) : this(u, false)
			{
			}

			protected MailboxConnectionGuard(UMMailboxRecipient u, bool withLock)
			{
				using (DisposeGuard disposeGuard = this.Guard())
				{
					this.user = u;
					this.withLock = withLock;
					this.user.AcquireMailboxSession(withLock);
					disposeGuard.Success();
				}
			}

			protected UMMailboxRecipient User
			{
				get
				{
					return this.user;
				}
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					this.user.ReleaseMailboxSession(this.withLock);
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UMMailboxRecipient.MailboxConnectionGuard>(this);
			}

			private UMMailboxRecipient user;

			private bool withLock;
		}

		internal class MailboxSessionLock : UMMailboxRecipient.MailboxConnectionGuard
		{
			internal MailboxSessionLock(UMMailboxRecipient u) : base(u, true)
			{
			}

			internal MailboxSession Session
			{
				get
				{
					MailboxSession unsafeMailboxSession = base.User.UnsafeMailboxSession;
					if (unsafeMailboxSession != null && !unsafeMailboxSession.IsConnected)
					{
						this.underlyingStoreRPCSessionDisconnected = MailboxSessionEstablisher.ConnectWithStatus(unsafeMailboxSession);
					}
					return unsafeMailboxSession;
				}
			}

			internal bool UnderlyingStoreRPCSessionDisconnected
			{
				get
				{
					return this.underlyingStoreRPCSessionDisconnected;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<UMMailboxRecipient.MailboxSessionLock>(this);
			}

			private bool underlyingStoreRPCSessionDisconnected;
		}
	}
}
