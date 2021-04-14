using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class MapiContext : LogicalContext
	{
		private MapiContext(ExecutionDiagnostics executionDiagnostics) : base(executionDiagnostics)
		{
		}

		private MapiContext(ExecutionDiagnostics executionDiagnostics, ClientSecurityContext securityContext, ClientType clientType, int lcid) : base(executionDiagnostics, securityContext, clientType, CultureHelper.TranslateLcid(lcid))
		{
		}

		public static TimeSpan MailboxLockTimeout
		{
			get
			{
				if (MapiContext.mailboxLockTimeoutHook != null)
				{
					return MapiContext.mailboxLockTimeoutHook();
				}
				return MapiContext.mailboxLockTimeout;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public MapiSession Session
		{
			get
			{
				return this.session;
			}
		}

		public override ClientSecurityContext SecurityContext
		{
			get
			{
				if (this.Session != null)
				{
					return this.Session.CurrentSecurityContext;
				}
				return base.SecurityContext;
			}
		}

		public bool HasMailboxFullRights
		{
			get
			{
				return this.mailboxFullRightsGranted || (this.logon != null && (this.logon.ExchangeTransportServiceRights || this.logon.SystemRights || this.logon.AdminRights || this.logon.IsOwner));
			}
		}

		internal bool HasInternalAccessRights
		{
			get
			{
				return this.logon == null || this.logon.IsSystemServiceLogon || this.logon.ExchangeTransportServiceRights || this.internalAccessPrivileges;
			}
		}

		public override ClientType ClientType
		{
			get
			{
				if (base.ClientType != ClientType.User)
				{
					return base.ClientType;
				}
				if (this.logon != null)
				{
					if (this.logon.SystemRights)
					{
						return ClientType.System;
					}
					if (this.logon.AdminRights)
					{
						return ClientType.Administrator;
					}
				}
				if (this.Session != null && this.Session.UsingTransportPrivilege)
				{
					return ClientType.Transport;
				}
				return base.ClientType;
			}
		}

		public override IMailboxContext PrimaryMailboxContext
		{
			get
			{
				if (this.logon == null)
				{
					return null;
				}
				return this.logon.MapiMailbox.StoreMailbox;
			}
		}

		public static MapiContext Create(ExecutionDiagnostics executionDiagnostics)
		{
			return new MapiContext(executionDiagnostics, Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.ProcessSecurityContext, ClientType.System, CultureHelper.GetLcidFromCulture(CultureHelper.DefaultCultureInfo));
		}

		public static MapiContext Create(ExecutionDiagnostics executionDiagnostics, ClientType clientType)
		{
			return new MapiContext(executionDiagnostics, Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.ProcessSecurityContext, clientType, CultureHelper.GetLcidFromCulture(CultureHelper.DefaultCultureInfo));
		}

		internal static MapiContext CreateSessionless(ExecutionDiagnostics executionDiagnostics, ClientSecurityContext securityContext, ClientType clientType, int lcid)
		{
			return new MapiContext(executionDiagnostics, securityContext, clientType, lcid);
		}

		internal MapiContext.MapiLogonFrame SetMapiLogonForNotificationContext(MapiLogon logon)
		{
			return new MapiContext.MapiLogonFrame(this, logon);
		}

		public void Configure(MapiSession session)
		{
			this.Configure(session.SessionSecurityContext, session.InternalClientType, session.LcidSort);
			base.UpdateTestCaseId(session.TestCaseId);
			this.session = session;
		}

		internal void Configure(ClientSecurityContext securityContext, ClientType clientType, int lcid)
		{
			base.SetUserInfo(securityContext, clientType, CultureHelper.TranslateLcid(lcid));
		}

		public void Initialize(MapiLogon logon, bool sharedMailboxLock, bool sharedUserLock)
		{
			this.Initialize(logon.MapiMailbox.Database.MdbGuid, logon.Session.SessionSecurityContext, logon.Session.InternalClientType, (logon.LoggedOnUserAddressInfo == null || (logon.IsSystemServiceLogon && base.TestCaseId.IsNull)) ? Guid.Empty : logon.LoggedOnUserAddressInfo.ObjectId, logon.Session.LcidSort);
			this.logon = logon;
			this.SetCulture();
			base.InitializeMailboxOperation(logon.MapiMailbox.SharedState, ExecutionDiagnostics.OperationSource.Mapi, MapiContext.MailboxLockTimeout, sharedMailboxLock, sharedUserLock);
		}

		public void Initialize(Guid databaseGuid, Guid mailboxGuid, bool sharedMailboxLock, bool sharedUserLock)
		{
			ClientSecurityContext securityContext = this.SecurityContext;
			ClientType clientType = this.ClientType;
			int lcid = CultureHelper.GetLcidFromCulture(base.Culture);
			if (this.session != null)
			{
				securityContext = this.session.SessionSecurityContext;
				clientType = this.session.InternalClientType;
				lcid = this.session.LcidSort;
			}
			this.Initialize(databaseGuid, securityContext, clientType, Guid.Empty, lcid);
			base.InitializeMailboxOperation(mailboxGuid, ExecutionDiagnostics.OperationSource.Mapi, MapiContext.MailboxLockTimeout, sharedMailboxLock, sharedUserLock);
		}

		public void Initialize(Guid databaseGuid, int mailboxNumber, bool sharedMailboxLock, bool sharedUserLock)
		{
			ClientSecurityContext securityContext = this.SecurityContext;
			ClientType clientType = this.ClientType;
			int lcid = CultureHelper.GetLcidFromCulture(base.Culture);
			if (this.session != null)
			{
				securityContext = this.session.SessionSecurityContext;
				clientType = this.session.InternalClientType;
				lcid = this.session.LcidSort;
			}
			this.Initialize(databaseGuid, securityContext, clientType, Guid.Empty, lcid);
			base.InitializeMailboxOperation(mailboxNumber, ExecutionDiagnostics.OperationSource.Mapi, MapiContext.MailboxLockTimeout, sharedMailboxLock, sharedUserLock);
		}

		private void Initialize(Guid databaseGuid, ClientSecurityContext securityContext, ClientType clientType, Guid userIdentity, int lcid)
		{
			base.SetUserInfo(securityContext, clientType, CultureHelper.TranslateLcid(lcid));
			base.Diagnostics.DatabaseGuid = databaseGuid;
			this.databaseGuid = databaseGuid;
			this.logon = null;
			base.UserIdentity = userIdentity;
		}

		public void SetMapiLogon(MapiLogon logon)
		{
			this.logon = logon;
			base.UserIdentity = ((logon == null || logon.LoggedOnUserAddressInfo == null || (logon.IsSystemServiceLogon && base.TestCaseId.IsNull)) ? Guid.Empty : logon.LoggedOnUserAddressInfo.ObjectId);
			this.SetCulture();
		}

		public MapiContext.MailboxFullRightsFrame GrantMailboxFullRights()
		{
			return new MapiContext.MailboxFullRightsFrame(this);
		}

		public void GrantInternalAccessPrivileges()
		{
			this.internalAccessPrivileges = true;
		}

		public void RevokeInternalAccessPrivileges()
		{
			this.internalAccessPrivileges = false;
		}

		public MapiContext.DisableNotificationPumpingFrame DisableNotificationPumping()
		{
			return new MapiContext.DisableNotificationPumpingFrame(this);
		}

		protected override void ConnectDatabase()
		{
			base.ConnectDatabase();
			if (base.IsConnected && this.logon == null)
			{
				return;
			}
			StoreDatabase storeDatabase;
			if (this.logon != null)
			{
				storeDatabase = this.logon.MapiMailbox.Database;
			}
			else
			{
				storeDatabase = Storage.FindDatabase(this.DatabaseGuid);
				if (storeDatabase == null)
				{
					using (base.CriticalBlock((LID)57824U, CriticalBlockScope.MailboxSession))
					{
						throw new StoreException((LID)53048U, ErrorCodeValue.MdbNotInitialized);
					}
				}
			}
			if (base.IsConnected && storeDatabase.MdbGuid != base.Database.MdbGuid)
			{
				base.Disconnect();
			}
			if (!base.IsConnected)
			{
				base.Connect(storeDatabase);
			}
			if (!storeDatabase.IsOnlineActive && !storeDatabase.IsOnlinePassiveAttachedReadOnly)
			{
				base.Disconnect();
				using (base.CriticalBlock((LID)33248U, CriticalBlockScope.MailboxSession))
				{
					throw new StoreException((LID)46904U, ErrorCodeValue.MdbNotInitialized);
				}
			}
			if (this.session != null)
			{
				this.session.LastUsedDatabase = base.Database;
			}
		}

		protected override void DisconnectDatabase()
		{
			base.Disconnect();
			base.DisconnectDatabase();
		}

		protected override void ConnectMailboxes()
		{
			base.ConnectMailboxes();
			this.SetCulture();
			if (this.logon != null && this.logon.CannotLogonToInTransitMailbox(this))
			{
				using (base.CriticalBlock((LID)49120U, CriticalBlockScope.MailboxSession))
				{
					throw new StoreException((LID)55096U, ErrorCodeValue.MdbNotInitialized);
				}
			}
			using (base.CriticalBlock((LID)65504U, CriticalBlockScope.MailboxSession))
			{
				if (this.session != null)
				{
					this.session.ConnectMailboxes(this);
				}
				if (this.session != null && !this.notificationPumpingDisabled)
				{
					this.session.PumpPendingNotifications(this, base.LockedMailboxState);
				}
				base.EndCriticalBlock();
			}
			if (this.logon != null && this.logon.IsValid)
			{
				this.logon.EstablishQuotaInfo();
			}
		}

		public override ErrorCode StartMailboxOperation(MailboxCreation mailboxCreation, bool findRemovedMailbox, bool skipQuarantineCheck)
		{
			if (base.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
			{
				throw new InvalidOperationException("critical block failure was ignored");
			}
			if (this.session != null)
			{
				this.session.ThrowIfNotValid(null);
			}
			ErrorCode first = base.StartMailboxOperation(mailboxCreation, findRemovedMailbox, skipQuarantineCheck);
			if (first != ErrorCode.NoError)
			{
				return first.Propagate((LID)61692U);
			}
			return ErrorCode.NoError;
		}

		public override void EndMailboxOperation(bool commit, bool skipDisconnectingDatabase, bool pulseOnly)
		{
			base.EndMailboxOperation(commit, skipDisconnectingDatabase, pulseOnly);
		}

		protected override IMailboxContext CreateMailboxContext(int mailboxNumber)
		{
			if (!base.PartitionFullAccessGranted && this.logon != null && !this.logon.UnifiedLogon)
			{
				throw new StoreException((LID)48540U, ErrorCodeValue.NoAccess);
			}
			if (this.session != null)
			{
				return this.session.GetInternallyReferencedMailboxContext(this, mailboxNumber);
			}
			return base.CreateMailboxContext(mailboxNumber);
		}

		private void SetCulture()
		{
			if (this.logon != null && this.logon.Session.LcidSort == 1024 && this.logon.MapiMailbox.GetLocalized(this) && (this.logon.OpenStoreFlags & OpenStoreFlags.NoLocalization) == OpenStoreFlags.None)
			{
				base.Culture = CultureHelper.TranslateLcid(this.logon.MapiMailbox.Lcid);
			}
		}

		internal bool IsAssociatedWithMailbox(MapiMailbox mailbox)
		{
			return !(this.DatabaseGuid != mailbox.MdbGuid) && mailbox.MailboxNumber == base.LockedMailboxState.MailboxNumber;
		}

		private static readonly TimeSpan mailboxLockTimeout = TimeSpan.FromMinutes(1.0);

		private static Func<TimeSpan> mailboxLockTimeoutHook = null;

		private MapiSession session;

		private Guid databaseGuid;

		private MapiLogon logon;

		private bool internalAccessPrivileges;

		private bool mailboxFullRightsGranted;

		private bool notificationPumpingDisabled;

		public struct MapiLogonFrame : IDisposable
		{
			internal MapiLogonFrame(MapiContext context, MapiLogon newLogon)
			{
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(context.logon == null || context.logon.MapiMailbox.StoreMailbox.SharedState.Equals(newLogon.MapiMailbox.StoreMailbox.SharedState), "Changing mailboxes?");
				this.context = context;
				this.oldLogon = this.context.logon;
				this.context.logon = newLogon;
				this.context.SetCulture();
			}

			public void Dispose()
			{
				this.context.logon = this.oldLogon;
				this.context.SetCulture();
			}

			private MapiContext context;

			private MapiLogon oldLogon;
		}

		public struct MailboxFullRightsFrame : IDisposable
		{
			internal MailboxFullRightsFrame(MapiContext context)
			{
				this.previousMailboxFullRightsGranted = context.mailboxFullRightsGranted;
				context.mailboxFullRightsGranted = true;
				this.context = context;
			}

			public void Dispose()
			{
				this.context.mailboxFullRightsGranted = this.previousMailboxFullRightsGranted;
			}

			private MapiContext context;

			private bool previousMailboxFullRightsGranted;
		}

		public struct DisableNotificationPumpingFrame : IDisposable
		{
			internal DisableNotificationPumpingFrame(MapiContext context)
			{
				this.previousNotificationPumpingDisabled = context.notificationPumpingDisabled;
				context.notificationPumpingDisabled = true;
				this.context = context;
			}

			public void Dispose()
			{
				this.context.notificationPumpingDisabled = this.previousNotificationPumpingDisabled;
			}

			private MapiContext context;

			private bool previousNotificationPumpingDisabled;
		}
	}
}
