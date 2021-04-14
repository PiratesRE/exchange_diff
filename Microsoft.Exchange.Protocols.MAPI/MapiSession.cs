using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.Mapi;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public sealed class MapiSession : DisposableBase, IConnectionInformation, INotificationSession
	{
		public MapiSession()
		{
			this.notificationContext = new NotificationContext(this);
			this.internalClientType = ClientType.User;
			this.testCaseId = TestCaseId.GetInProcessTestCaseId();
			for (int i = 0; i < 13; i++)
			{
				this.objectTracker[i] = new MapiPerSessionObjectCounter((MapiObjectTrackedType)i, this);
			}
		}

		internal ClientSecurityContext SessionSecurityContext
		{
			get
			{
				return this.sessionSecurityContext.SecurityContext;
			}
		}

		internal StoreDatabase LastUsedDatabase
		{
			get
			{
				return this.lastUsedDatabase;
			}
			set
			{
				this.lastUsedDatabase = value;
			}
		}

		internal ClientSecurityContext CurrentSecurityContext
		{
			get
			{
				if (!this.usingDelegatedAuth || !this.CanAcceptROPs)
				{
					return this.sessionSecurityContext.SecurityContext;
				}
				return this.delegatedSecurityContext.SecurityContext;
			}
		}

		internal MapiExMonLogger MapiExMonLogger
		{
			get
			{
				return this.exmonLogger;
			}
			set
			{
				this.exmonLogger = value;
			}
		}

		public Guid SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		internal int MaxRecipients
		{
			get
			{
				return int.MaxValue;
			}
		}

		internal bool ShouldBeCommitted
		{
			get
			{
				return this.shouldBeCommitted;
			}
			set
			{
				this.shouldBeCommitted = value;
			}
		}

		public int RpcContext { get; set; }

		public AddressInfo AddressInfoUser
		{
			get
			{
				return this.addressInfoUser;
			}
		}

		public string UserDN
		{
			get
			{
				if (this.AddressInfoUser == null)
				{
					return this.userDn;
				}
				return this.AddressInfoUser.LegacyExchangeDN;
			}
		}

		public Guid UserGuid
		{
			get
			{
				if (this.AddressInfoUser == null)
				{
					return Guid.Empty;
				}
				return this.AddressInfoUser.ObjectId;
			}
		}

		public SecurityIdentifier UserSid
		{
			get
			{
				return this.CurrentSecurityContext.UserSid;
			}
		}

		internal SecurityContextKey KeySessionSecurityContext
		{
			get
			{
				return this.keySessionSecurityContext;
			}
		}

		internal SecurityContextKey KeyDelegatedSecurityContext
		{
			get
			{
				return this.keyDelegatedSecurityContext;
			}
		}

		internal SecurityContextKey KeyCurrentSecurityContext
		{
			get
			{
				if (!this.usingDelegatedAuth || !this.CanAcceptROPs)
				{
					return this.KeySessionSecurityContext;
				}
				return this.keyDelegatedSecurityContext;
			}
		}

		internal Version ClientVersion
		{
			get
			{
				return this.clientVersion;
			}
			set
			{
				this.clientVersion = value;
			}
		}

		public string ClientMachineName
		{
			get
			{
				return this.clientMachineName;
			}
		}

		public ClientMode ClientMode
		{
			get
			{
				return this.clientMode;
			}
		}

		public string ClientProcessName
		{
			get
			{
				return this.clientProcessName;
			}
		}

		public DateTime LastAccessTime
		{
			get
			{
				return this.lastAccessTime;
			}
			set
			{
				this.lastAccessTime = value;
			}
		}

		public Guid LastClientActivityId
		{
			get
			{
				return this.lastClientActivityId;
			}
			set
			{
				this.lastClientActivityId = value;
			}
		}

		public string LastClientProtocol
		{
			get
			{
				return this.lastClientProtocol;
			}
			set
			{
				this.lastClientProtocol = value;
			}
		}

		public string LastClientComponent
		{
			get
			{
				return this.lastClientComponent;
			}
			set
			{
				this.lastClientComponent = value;
			}
		}

		public string LastClientAction
		{
			get
			{
				return this.lastClientAction;
			}
			set
			{
				this.lastClientAction = value;
			}
		}

		public DateTime ConnectTime
		{
			get
			{
				return this.connectTime;
			}
			set
			{
				this.connectTime = value;
			}
		}

		public CodePage CodePage
		{
			get
			{
				return this.codePage;
			}
			set
			{
				this.codePage = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				CodePage codePage = this.codePage;
				Encoding result;
				if (codePage != CodePage.ReducedUnicode)
				{
					if (codePage == CodePage.None)
					{
						result = CodePageMap.GetEncoding(1252);
					}
					else
					{
						result = CodePageMap.GetEncoding((int)this.codePage);
					}
				}
				else
				{
					result = String8Encodings.ReducedUnicode;
				}
				return result;
			}
		}

		public byte LastUsedLogonIndex
		{
			get
			{
				return this.lastUsedLogonIndex;
			}
			set
			{
				this.lastUsedLogonIndex = value;
			}
		}

		internal int LcidSort
		{
			get
			{
				return this.lcidSort;
			}
			set
			{
				this.lcidSort = value;
			}
		}

		internal int LcidString
		{
			get
			{
				return this.lcidString;
			}
			set
			{
				this.lcidString = value;
			}
		}

		public bool CanConvertCodePage
		{
			get
			{
				return this.canConvertCodePage;
			}
			set
			{
				this.canConvertCodePage = value;
			}
		}

		public bool UsingDelegatedAuth
		{
			get
			{
				return this.usingDelegatedAuth;
			}
		}

		public bool UsingTransportPrivilege
		{
			get
			{
				return this.usingTransportPrivilege;
			}
		}

		public bool UsingAdminPrivilege
		{
			get
			{
				return this.usingAdminPrivilege;
			}
		}

		public bool UsingLogonAdminPrivilege
		{
			get
			{
				return this.usingLogonAdminPrivilege;
			}
			set
			{
				this.usingLogonAdminPrivilege = value;
			}
		}

		public bool CanAcceptROPs
		{
			get
			{
				return this.canAcceptRops;
			}
			set
			{
				this.canAcceptRops = value;
			}
		}

		public bool IsDelegatedContextInitialized
		{
			get
			{
				return this.isDelegatedContextInitialized;
			}
		}

		public int LogonCount
		{
			get
			{
				this.ThrowIfNotValid(null);
				return this.logons.Count;
			}
		}

		public bool NeedToClose
		{
			get
			{
				return this.needToClose;
			}
		}

		internal Dictionary<int, MapiLogon>.ValueCollection Logons
		{
			get
			{
				return this.logons.Values;
			}
		}

		public NotificationContext NotificationContext
		{
			get
			{
				this.ThrowIfNotValid(null);
				return this.notificationContext;
			}
		}

		public bool InRpc { get; set; }

		public bool IsLockHeld
		{
			get
			{
				return LockManager.TestLock(this.sessionLockObject, LockManager.LockType.Session);
			}
		}

		public bool IsValid
		{
			get
			{
				return this.valid;
			}
		}

		internal IRopDriver RopDriver
		{
			get
			{
				this.ThrowIfNotValid(null);
				return this.ropDriver;
			}
		}

		internal ClientType InternalClientType
		{
			get
			{
				this.ThrowIfNotValid(null);
				return this.internalClientType;
			}
			set
			{
				this.ThrowIfNotValid(null);
				this.internalClientType = value;
			}
		}

		public string ApplicationId
		{
			get
			{
				return this.applicationIdString;
			}
		}

		public TestCaseId TestCaseId
		{
			get
			{
				return this.testCaseId;
			}
		}

		internal IConnectionHandler ConnectionHandler
		{
			get
			{
				return this.connectionHandler;
			}
		}

		ushort IConnectionInformation.SessionId
		{
			get
			{
				this.ThrowIfNotValid(null);
				return 0;
			}
		}

		bool IConnectionInformation.ClientSupportsBackoffResult
		{
			get
			{
				this.ThrowIfNotValid(null);
				return true;
			}
		}

		bool IConnectionInformation.ClientSupportsBufferTooSmallBreakup
		{
			get
			{
				this.ThrowIfNotValid(null);
				return false;
			}
		}

		Encoding IConnectionInformation.String8Encoding
		{
			get
			{
				this.ThrowIfNotValid(null);
				return this.Encoding;
			}
		}

		internal static bool CheckCreateSessionRightsOnConnect(SecurityDescriptor serverNTSecurityDescriptor, ClientSecurityContext callerSecurityContext, bool usingDelegatedAuth, bool usingTransportPrivilege, bool usingAdminPrivilege)
		{
			Microsoft.Exchange.Diagnostics.Trace createSessionTracer = ExTraceGlobals.CreateSessionTracer;
			bool flag = false;
			bool flag2 = false;
			if (usingTransportPrivilege && !flag)
			{
				flag2 = SecurityHelper.CheckTransportPrivilege(callerSecurityContext, serverNTSecurityDescriptor);
				if (!flag2)
				{
					flag = true;
					if (createSessionTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						createSessionTracer.TraceDebug(0L, "Transport privilege requested but not granted");
					}
					DiagnosticContext.TraceLocation((LID)33863U);
				}
			}
			bool flag3 = false;
			if (usingDelegatedAuth && !flag)
			{
				flag3 = SecurityHelper.CheckConstrainedDelegationPrivilege(callerSecurityContext, serverNTSecurityDescriptor);
				if (!flag3)
				{
					flag = true;
					if (createSessionTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						createSessionTracer.TraceDebug(0L, "Constrained delegation privilege requested but not granted");
					}
					DiagnosticContext.TraceLocation((LID)50247U);
				}
			}
			bool flag4 = false;
			if (usingAdminPrivilege && !flag)
			{
				flag4 = SecurityHelper.CheckAdministrativeRights(callerSecurityContext, serverNTSecurityDescriptor);
				if (!flag4)
				{
					if (createSessionTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						createSessionTracer.TraceDebug(0L, "Admin privilege requested but not granted on server object");
					}
					DiagnosticContext.TraceLocation((LID)36648U);
				}
			}
			int num = (usingTransportPrivilege ? 1 : 0) + (usingDelegatedAuth ? 1 : 0) + (usingAdminPrivilege ? 1 : 0);
			int num2 = (flag2 ? 1 : 0) + (flag3 ? 1 : 0) + (flag4 ? 1 : 0);
			bool flag5 = num > 0 && num == num2;
			if (!flag5)
			{
				if (createSessionTracer.IsTraceEnabled(TraceType.ErrorTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(64);
					stringBuilder.Append("ACCESS DENIED");
					stringBuilder.Append(". UserSid:[");
					SecurityHelper.AppendToString(stringBuilder, callerSecurityContext.UserSid);
					stringBuilder.Append("].");
					createSessionTracer.TraceError(0L, stringBuilder.ToString());
				}
				DiagnosticContext.TraceLocation((LID)63559U);
			}
			return flag5;
		}

		internal static bool CheckCreateSessionRightsOnLogon(AddressInfo addressInfo, Func<MailboxInfo> mailboxInfoGetter, Func<MailboxInfo, DatabaseInfo> databaseInfoGetter, SecurityDescriptor serverNTSecurityDescriptor, ClientSecurityContext callerSecurityContext, bool claimAdminPrivilegeOnDatabase)
		{
			Microsoft.Exchange.Diagnostics.Trace createSessionTracer = ExTraceGlobals.CreateSessionTracer;
			bool flag = SecurityHelper.CheckMailboxOwnerRights(callerSecurityContext, addressInfo);
			if (flag)
			{
				return true;
			}
			if (createSessionTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				createSessionTracer.TraceDebug(0L, "create session rights check based on AddressInfo failed; fall through for a MailboxInfo based access");
			}
			DiagnosticContext.TraceLocation((LID)58951U);
			MailboxInfo mailboxInfo = mailboxInfoGetter();
			DatabaseInfo databaseInfo = databaseInfoGetter(mailboxInfo);
			bool hasMailbox = addressInfo.HasMailbox;
			if (databaseInfo != null)
			{
				if (claimAdminPrivilegeOnDatabase)
				{
					DiagnosticContext.TraceLocation((LID)35088U);
					flag = SecurityHelper.CheckAdministrativeRights(callerSecurityContext, databaseInfo.NTSecurityDescriptor);
					if (!flag)
					{
						DiagnosticContext.TraceLocation((LID)57616U);
					}
				}
				else if (mailboxInfo != null)
				{
					DiagnosticContext.TraceLocation((LID)51472U);
					flag = SecurityHelper.CheckMailboxOwnerRights(callerSecurityContext, mailboxInfo, databaseInfo);
					if (!flag)
					{
						DiagnosticContext.TraceLocation((LID)55336U);
					}
				}
				else
				{
					DiagnosticContext.TraceLocation((LID)45328U);
					flag = false;
				}
			}
			else if (mailboxInfo != null && mailboxInfo.IsSystemAttendantRecipient)
			{
				DiagnosticContext.TraceLocation((LID)59664U);
				flag = SecurityHelper.CheckAdministrativeRights(callerSecurityContext, serverNTSecurityDescriptor);
				if (!flag)
				{
					DiagnosticContext.TraceLocation((LID)43048U);
				}
			}
			else
			{
				DiagnosticContext.TraceLocation((LID)59432U);
				flag = false;
			}
			if (flag)
			{
				return true;
			}
			if (createSessionTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				StringBuilder stringBuilder = new StringBuilder(64);
				stringBuilder.Append("ACCESS DENIED");
				stringBuilder.Append(". UserSid:[");
				SecurityHelper.AppendToString(stringBuilder, callerSecurityContext.UserSid);
				stringBuilder.Append("]. ConnectingAs:[");
				stringBuilder.Append((addressInfo == null) ? "Local SERVER" : addressInfo.LegacyExchangeDN);
				stringBuilder.Append("].");
				createSessionTracer.TraceError(0L, stringBuilder.ToString());
			}
			DiagnosticContext.TraceLocation((LID)56296U);
			return false;
		}

		internal void HydrateSessionSecurityContext()
		{
			ClientSecurityContext clientSecurityContext = null;
			this.sessionSecurityContext = SecurityContextManager.StartRPCUse(this.keySessionSecurityContext, ref clientSecurityContext);
			this.keySessionSecurityContext = null;
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(null != this.sessionSecurityContext, "this.sessionSecurityContext should not be null post hydration");
		}

		internal void DehydrateSessionSecurityContext()
		{
			if (!this.IsValid)
			{
				return;
			}
			this.keySessionSecurityContext = this.sessionSecurityContext.SecurityContextKey;
			SecurityContextManager.EndRPCUse(ref this.sessionSecurityContext, false);
			this.sessionSecurityContext = null;
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(null == this.sessionSecurityContext, "this.SessionSecurityContext should be null post de-hydration");
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(null != this.keySessionSecurityContext, "this.keySessionSecurityContext should be NOT null post de-hydration");
		}

		internal void HydrateDelegatedSecurityContext()
		{
			if (!this.isDelegatedContextInitialized)
			{
				return;
			}
			ClientSecurityContext clientSecurityContext = null;
			this.delegatedSecurityContext = SecurityContextManager.StartRPCUse(this.keyDelegatedSecurityContext, ref clientSecurityContext);
			this.keyDelegatedSecurityContext = null;
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(null != this.delegatedSecurityContext, "bad refcounting for this.delegatedSecurityContext");
		}

		internal void DehydrateDelegatedSecurityContext()
		{
			if (!this.IsValid)
			{
				return;
			}
			if (!this.isDelegatedContextInitialized)
			{
				return;
			}
			this.keyDelegatedSecurityContext = this.delegatedSecurityContext.SecurityContextKey;
			SecurityContextManager.EndRPCUse(ref this.delegatedSecurityContext, false);
			this.delegatedSecurityContext = null;
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(null == this.delegatedSecurityContext, "this.delegatedSecurityContext should be null post de-hydration");
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(null != this.keyDelegatedSecurityContext, "delegatedSecurityContext should be NOT null post de-hydration");
		}

		internal void ConnectMailboxes(MapiContext context)
		{
			foreach (MapiLogon mapiLogon in this.Logons)
			{
				if (mapiLogon != null && mapiLogon.IsValid && context.IsAssociatedWithMailbox(mapiLogon.MapiMailbox))
				{
					mapiLogon.Connect(context);
				}
			}
			if (this.internalReferencedMailboxes != null)
			{
				foreach (int mailboxNumber in this.internalReferencedMailboxes)
				{
					MapiMailbox mapiMailbox = this.GetMapiMailbox(mailboxNumber);
					if (!mapiMailbox.SharedState.IsValid)
					{
						using (context.CriticalBlock((LID)38300U, CriticalBlockScope.MailboxSession))
						{
							throw new StoreException((LID)36252U, ErrorCodeValue.MdbNotInitialized);
						}
					}
					mapiMailbox.Connect(context);
				}
			}
		}

		internal IMailboxContext GetInternallyReferencedMailboxContext(MapiContext context, int mailboxNumber)
		{
			bool flag = false;
			MapiMailbox mapiMailbox = this.GetMapiMailbox(mailboxNumber);
			if (mapiMailbox == null)
			{
				MailboxState mailboxState = MailboxStateCache.Get(context, mailboxNumber);
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(mailboxState != null, "MailboxState not found. Who is referencing non-existng mailbox?");
				mapiMailbox = MapiMailbox.OpenMailbox(context, mailboxState, MailboxInfo.MailboxType.Private, 0);
				Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(mapiMailbox != null, "MapiMailbox not found. Who is referencing non-existng mailbox?");
				flag = true;
			}
			if (this.internalReferencedMailboxes == null)
			{
				this.internalReferencedMailboxes = new List<int>();
			}
			if (!this.internalReferencedMailboxes.Contains(mailboxNumber))
			{
				this.ReferenceMapiMailbox(mapiMailbox);
				this.internalReferencedMailboxes.Add(mailboxNumber);
				if (!flag)
				{
					mapiMailbox.Connect(context);
				}
			}
			return mapiMailbox.StoreMailbox;
		}

		internal void GetCurrentActivityDetails(out string activityId, out string protocol, out string component, out string action)
		{
			ExecutionDiagnostics currentDiagnostics = this.GetCurrentDiagnostics();
			if (currentDiagnostics != null)
			{
				activityId = currentDiagnostics.ClientActivityId.ToString();
				protocol = currentDiagnostics.ClientProtocolName;
				component = currentDiagnostics.ClientComponentName;
				action = currentDiagnostics.ClientActionString;
				return;
			}
			string empty;
			action = (empty = string.Empty);
			string text;
			component = (text = empty);
			string text2;
			protocol = (text2 = text);
			activityId = text2;
		}

		private ExecutionDiagnostics GetCurrentDiagnostics()
		{
			ExecutionDiagnostics result = null;
			IConnectionHandler connectionHandler = this.ConnectionHandler;
			if (connectionHandler != null)
			{
				IRopHandlerWithContext ropHandlerWithContext = connectionHandler.RopHandler as IRopHandlerWithContext;
				if (ropHandlerWithContext != null)
				{
					MapiContext mapiContext = ropHandlerWithContext.MapiContext;
					if (mapiContext != null)
					{
						result = mapiContext.Diagnostics;
					}
				}
			}
			return result;
		}

		public MapiMailbox GetMapiMailbox(int mailboxNumber)
		{
			KeyValuePair<MapiMailbox, int> keyValuePair;
			if (this.mailboxes.TryGetValue(mailboxNumber, out keyValuePair))
			{
				return keyValuePair.Key;
			}
			return null;
		}

		public void ReferenceMapiMailbox(MapiMailbox mailbox)
		{
			int mailboxNumber = mailbox.MailboxNumber;
			KeyValuePair<MapiMailbox, int> value;
			if (this.mailboxes.TryGetValue(mailboxNumber, out value))
			{
				value = new KeyValuePair<MapiMailbox, int>(value.Key, value.Value + 1);
			}
			else
			{
				value = new KeyValuePair<MapiMailbox, int>(mailbox, 1);
			}
			this.mailboxes[mailboxNumber] = value;
		}

		public void ReleaseMapiMailbox(MapiMailbox mailbox)
		{
			int mailboxNumber = mailbox.MailboxNumber;
			KeyValuePair<MapiMailbox, int> keyValuePair = this.mailboxes[mailboxNumber];
			if (keyValuePair.Value - 1 == 0)
			{
				this.mailboxes.Remove(mailboxNumber);
				keyValuePair.Key.Dispose();
				return;
			}
			this.mailboxes[mailboxNumber] = new KeyValuePair<MapiMailbox, int>(keyValuePair.Key, keyValuePair.Value - 1);
		}

		internal void SetDelegatedAuthInfo(AddressInfo addressInfo, ref ClientSecurityContext callerSecurityContext)
		{
			this.addressInfoUser = addressInfo;
			if (this.isDelegatedContextInitialized || this.keyDelegatedSecurityContext != null)
			{
				throw new ExExceptionLogonFailed((LID)61680U, "Not supported: changing the delegated auth context on a session.");
			}
			this.delegatedSecurityContext = SecurityContextManager.StartRPCUse(null, ref callerSecurityContext);
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(this.delegatedSecurityContext != null, "can't have a null delegatedSecurityContext, we just added it to SecurityContextManager");
			this.isDelegatedContextInitialized = true;
		}

		internal void SetAddressInfo(AddressInfo addressInfo)
		{
			this.addressInfoUser = addressInfo;
			this.userDn = null;
		}

		internal void AddLogon(int logonIndex, MapiLogon logon)
		{
			this.ThrowIfNotValid(null);
			this.logons.Add(logonIndex, logon);
		}

		internal void RemoveLogon(int logonIndex)
		{
			this.ThrowIfNotValid(null);
			if (!base.IsDisposing)
			{
				this.logons.Remove(logonIndex);
			}
			if (this.LogonCount == 0)
			{
				this.DecrementSessionCount(MapiObjectTrackingScope.Service | MapiObjectTrackingScope.User);
			}
		}

		internal void UpdateApplicationId(MapiContext context, string applicationIdString)
		{
			ClientType clientType = ClientType.User;
			if (!ClientTypeHelper.TryGetClientType(applicationIdString, out clientType) && this.ClientVersion > Version.Exchange15MinVersion)
			{
				throw new StoreException((LID)58064U, ErrorCodeValue.InvalidParameter, "Unable to extract known client type from the applicationId:" + applicationIdString);
			}
			this.internalClientType = clientType;
			this.applicationIdString = applicationIdString;
			context.UpdateClientType(clientType);
			this.exmonLogger.ServiceName = applicationIdString;
		}

		public MapiLogon GetLogon(int logonIndex)
		{
			this.ThrowIfNotValid(null);
			MapiLogon result;
			if (logonIndex >= 0 && this.logons.TryGetValue(logonIndex, out result))
			{
				return result;
			}
			return null;
		}

		public void ThrowIfNotValid(string errorMessage)
		{
			if (!this.valid)
			{
				throw new ExExceptionInvalidObject((LID)41784U, (errorMessage == null) ? "This MapiSession object is not valid." : errorMessage);
			}
		}

		public void RequestClose()
		{
			this.ThrowIfNotValid(null);
			this.needToClose = true;
			this.NotificationPending();
		}

		internal void ConfigureMapiSessionForTest(AddressInfo addressInfoUser, ref ClientSecurityContext callerSecurityContext, CodePage codePage, int lcidString, int lcidSort, bool canConvertCodePage, Version clientVersion, bool usingDelegatedAuth, bool usingTransportPrivilege, bool usingAdminPrivilege, Func<MapiSession, IConnectionHandler> connectionHandlerFactory, IDriverFactory driverFactory, Action<int> notificationPendingCallback)
		{
			this.addressInfoUser = addressInfoUser;
			this.ConfigureMapiSessionHelper(ref callerSecurityContext, codePage, lcidString, lcidSort, canConvertCodePage, clientVersion, null, ClientMode.Unknown, null, usingDelegatedAuth, usingTransportPrivilege, usingAdminPrivilege, connectionHandlerFactory, driverFactory, notificationPendingCallback);
		}

		internal void ConfigureMapiSession(string userDn, ref ClientSecurityContext callerSecurityContext, CodePage codePage, int lcidString, int lcidSort, bool canConvertCodePage, Version clientVersion, string clientMachineName, ClientMode clientMode, string clientProcessName, bool usingDelegatedAuth, bool usingTransportPrivilege, bool usingAdminPrivilege, Func<MapiSession, IConnectionHandler> connectionHandlerFactory, IDriverFactory driverFactory, Action<int> notificationPendingCallback)
		{
			this.userDn = userDn;
			this.ConfigureMapiSessionHelper(ref callerSecurityContext, codePage, lcidString, lcidSort, canConvertCodePage, clientVersion, clientMachineName, clientMode, clientProcessName, usingDelegatedAuth, usingTransportPrivilege, usingAdminPrivilege, connectionHandlerFactory, driverFactory, notificationPendingCallback);
		}

		private void ConfigureMapiSessionHelper(ref ClientSecurityContext callerSecurityContext, CodePage codePage, int lcidString, int lcidSort, bool canConvertCodePage, Version clientVersion, string clientMachineName, ClientMode clientMode, string clientProcessName, bool usingDelegatedAuth, bool usingTransportPrivilege, bool usingAdminPrivilege, Func<MapiSession, IConnectionHandler> connectionHandlerFactory, IDriverFactory driverFactory, Action<int> notificationPendingCallback)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				this.sessionSecurityContext = SecurityContextManager.StartRPCUse(null, ref callerSecurityContext);
				this.keySessionSecurityContext = null;
				this.delegatedSecurityContext = null;
				this.keyDelegatedSecurityContext = null;
				this.ClientVersion = clientVersion;
				this.clientMachineName = clientMachineName;
				this.clientMode = clientMode;
				this.clientProcessName = clientProcessName;
				this.LcidString = lcidString;
				this.LcidSort = lcidSort;
				this.ConnectTime = DateTime.UtcNow;
				this.CodePage = codePage;
				this.CanConvertCodePage = canConvertCodePage;
				this.usingDelegatedAuth = usingDelegatedAuth;
				this.usingTransportPrivilege = usingTransportPrivilege;
				this.usingAdminPrivilege = usingAdminPrivilege;
				this.connectionHandler = connectionHandlerFactory(this);
				this.ropDriver = driverFactory.CreateIRopDriver(this.connectionHandler, this);
				this.canAcceptRops = !usingDelegatedAuth;
				this.notificationPendingCallback = notificationPendingCallback;
				disposeGuard.Success();
				this.valid = true;
			}
		}

		public void LockSession(bool inRpc)
		{
			LockManager.GetLock(this.sessionLockObject, LockManager.LockType.Session, this.GetCurrentDiagnostics());
			if (this.IsValid)
			{
				NotificationContext.Current = this.NotificationContext;
				this.InRpc = inRpc;
			}
		}

		public bool TryLockSession(bool inRpc)
		{
			if (!LockManager.TryGetLock(this.sessionLockObject, LockManager.LockType.Session, this.GetCurrentDiagnostics()))
			{
				return false;
			}
			if (this.IsValid)
			{
				NotificationContext.Current = this.NotificationContext;
				this.InRpc = inRpc;
			}
			return true;
		}

		public void UnlockSession()
		{
			this.InRpc = false;
			NotificationContext.Current = null;
			LockManager.ReleaseAnyLock(LockManager.LockType.Session);
		}

		[Conditional("DEBUG")]
		public void AssertLockHeld()
		{
		}

		[Conditional("DEBUG")]
		public void AssertAllMailboxesAreDisconnected()
		{
			foreach (KeyValuePair<int, KeyValuePair<MapiMailbox, int>> keyValuePair in this.mailboxes)
			{
			}
		}

		public void PumpPendingNotifications(MapiContext context, MailboxState mailboxState)
		{
			NotificationEvent nev;
			while ((nev = this.notificationContext.DequeueEvent(context.Database.MdbGuid, mailboxState.MailboxNumber)) != null)
			{
				NotificationSubscription.PumpOneNotificationInCurrentContext(context, nev);
			}
		}

		public IMapiObjectCounter GetPerSessionObjectCounter(MapiObjectTrackedType trackedType)
		{
			if (trackedType >= MapiObjectTrackedType.UntrackedObject)
			{
				return UnlimitedObjectCounter.Instance;
			}
			if (this.UsingTransportPrivilege)
			{
				return UnlimitedObjectCounter.Instance;
			}
			return this.objectTracker[(int)trackedType];
		}

		internal void NotificationPending()
		{
			if (this.notificationPendingCallback != null)
			{
				this.notificationPendingCallback(this.RpcContext);
			}
		}

		internal void IncrementSessionCount(MapiObjectTrackingScope scope)
		{
			if ((scope & MapiObjectTrackingScope.Service) != (MapiObjectTrackingScope)0U)
			{
				if (this.serviceSessionCounter != null)
				{
					this.serviceSessionCounter.DecrementCount();
				}
				this.serviceSessionCounter = MapiSessionPerServiceCounter.GetObjectCounter(this.ServiceClientType());
				this.serviceSessionCounter.IncrementCount();
				this.serviceSessionCounter.CheckObjectQuota(false);
			}
			if ((scope & MapiObjectTrackingScope.User) != (MapiObjectTrackingScope)0U)
			{
				if (this.userSessionCounter != null)
				{
					this.userSessionCounter.DecrementCount();
				}
				ClientType clientType = (this.UsingAdminPrivilege || this.UsingLogonAdminPrivilege) ? ClientType.Administrator : this.internalClientType;
				this.userSessionCounter = MapiSessionPerUserCounter.GetObjectCounter(this.UserDN, this.CurrentSecurityContext.UserSid, clientType);
				this.userSessionCounter.IncrementCount();
				this.userSessionCounter.CheckObjectQuota(false);
			}
		}

		internal void DecrementSessionCount(MapiObjectTrackingScope scope)
		{
			if ((scope & MapiObjectTrackingScope.Service) != (MapiObjectTrackingScope)0U && this.serviceSessionCounter != null)
			{
				this.serviceSessionCounter.DecrementCount();
				this.serviceSessionCounter = null;
			}
			if ((scope & MapiObjectTrackingScope.User) != (MapiObjectTrackingScope)0U && this.userSessionCounter != null)
			{
				this.userSessionCounter.DecrementCount();
				this.userSessionCounter = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiSession>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				foreach (KeyValuePair<int, MapiLogon> keyValuePair in this.logons)
				{
					MapiLogon value = keyValuePair.Value;
					if (value != null)
					{
						MailboxState sharedState = value.MapiMailbox.SharedState;
						bool assertCondition = sharedState.TryGetMailboxLock(false, LockManager.CrashingThresholdTimeout, this.GetCurrentDiagnostics());
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "Since we lock with crashing timeout we should either succeed or crash.");
						try
						{
							value.Dispose();
						}
						finally
						{
							sharedState.ReleaseMailboxLock(false);
						}
					}
				}
				if (this.internalReferencedMailboxes != null)
				{
					foreach (int mailboxNumber in this.internalReferencedMailboxes)
					{
						MapiMailbox mapiMailbox = this.GetMapiMailbox(mailboxNumber);
						MailboxState sharedState2 = mapiMailbox.SharedState;
						bool assertCondition2 = sharedState2.TryGetMailboxLock(false, LockManager.CrashingThresholdTimeout, this.GetCurrentDiagnostics());
						Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition2, "Since we lock with crashing timeout we should either succeed or crash.");
						try
						{
							this.ReleaseMapiMailbox(mapiMailbox);
						}
						finally
						{
							sharedState2.ReleaseMailboxLock(false);
						}
					}
				}
				this.DecrementSessionCount(MapiObjectTrackingScope.Service | MapiObjectTrackingScope.User);
				foreach (KeyValuePair<int, KeyValuePair<MapiMailbox, int>> keyValuePair2 in this.mailboxes)
				{
					keyValuePair2.Value.Key.SharedState.GetMailboxLock(false, this.GetCurrentDiagnostics());
					try
					{
						keyValuePair2.Value.Key.Dispose();
					}
					finally
					{
						keyValuePair2.Value.Key.SharedState.ReleaseMailboxLock(false);
					}
				}
				this.notificationContext.Dispose();
				if (this.ropDriver != null)
				{
					this.ropDriver.Dispose();
				}
				if (this.connectionHandler != null)
				{
					this.connectionHandler.Dispose();
				}
				if (this.sessionSecurityContext != null)
				{
					SecurityContextManager.EndRPCUse(ref this.sessionSecurityContext, true);
					this.sessionSecurityContext = null;
				}
				if (this.delegatedSecurityContext != null)
				{
					SecurityContextManager.EndRPCUse(ref this.delegatedSecurityContext, true);
					this.delegatedSecurityContext = null;
				}
				if (this.exmonLogger != null)
				{
					this.exmonLogger.Dispose();
					this.exmonLogger = null;
				}
			}
			this.valid = false;
			this.logons = null;
			this.mailboxes = null;
			this.notificationContext = null;
			this.ropDriver = null;
			this.connectionHandler = null;
			this.sessionSecurityContext = null;
			this.keySessionSecurityContext = null;
			this.delegatedSecurityContext = null;
			this.keyDelegatedSecurityContext = null;
		}

		internal void RememberLastException(Exception e)
		{
			this.e1 = this.e2;
			this.e2 = this.e3;
			this.e3 = e;
		}

		internal Exception LastRememberedException
		{
			get
			{
				return this.e3;
			}
		}

		internal MapiServiceType ServiceClientType()
		{
			if (this.internalClientType == ClientType.AvailabilityService)
			{
				return MapiServiceType.Availability;
			}
			if (this.internalClientType == ClientType.EventBasedAssistants || this.internalClientType == ClientType.TimeBasedAssistants)
			{
				return MapiServiceType.Assistants;
			}
			if (ClientTypeHelper.IsContentIndexing(this.internalClientType))
			{
				return MapiServiceType.ContentIndex;
			}
			if (this.internalClientType == ClientType.Inference)
			{
				return MapiServiceType.Inference;
			}
			if (this.internalClientType == ClientType.ELC)
			{
				return MapiServiceType.ELC;
			}
			if (this.internalClientType == ClientType.SMS)
			{
				return MapiServiceType.SMS;
			}
			if (this.usingTransportPrivilege)
			{
				return MapiServiceType.Transport;
			}
			if (this.usingAdminPrivilege)
			{
				return MapiServiceType.Admin;
			}
			return MapiServiceType.UnknownServiceType;
		}

		private readonly Guid sessionId = Guid.NewGuid();

		private readonly object sessionLockObject = new object();

		private bool valid;

		private bool needToClose;

		private StoreDatabase lastUsedDatabase;

		private Dictionary<int, MapiLogon> logons = new Dictionary<int, MapiLogon>();

		private Dictionary<int, KeyValuePair<MapiMailbox, int>> mailboxes = new Dictionary<int, KeyValuePair<MapiMailbox, int>>();

		private List<int> internalReferencedMailboxes;

		private CountedClientSecurityContext sessionSecurityContext;

		private SecurityContextKey keySessionSecurityContext;

		private CountedClientSecurityContext delegatedSecurityContext;

		private SecurityContextKey keyDelegatedSecurityContext;

		private CodePage codePage = CodePage.None;

		private AddressInfo addressInfoUser;

		private string userDn = string.Empty;

		private int lcidString;

		private int lcidSort;

		private bool canConvertCodePage;

		private DateTime connectTime;

		private Version clientVersion;

		private string clientMachineName;

		private ClientMode clientMode;

		private string clientProcessName;

		private DateTime lastAccessTime;

		private Guid lastClientActivityId;

		private string lastClientProtocol;

		private string lastClientComponent;

		private string lastClientAction;

		private bool usingDelegatedAuth;

		private bool usingTransportPrivilege;

		private bool usingAdminPrivilege;

		private bool usingLogonAdminPrivilege;

		private bool canAcceptRops;

		private bool isDelegatedContextInitialized;

		private bool shouldBeCommitted;

		private NotificationContext notificationContext;

		private string applicationIdString;

		private ClientType internalClientType;

		private TestCaseId testCaseId;

		private IConnectionHandler connectionHandler;

		private IRopDriver ropDriver;

		private byte lastUsedLogonIndex = byte.MaxValue;

		private Action<int> notificationPendingCallback;

		private MapiExMonLogger exmonLogger;

		private IMapiObjectCounter[] objectTracker = new MapiPerSessionObjectCounter[13];

		private IMapiObjectCounter serviceSessionCounter;

		private IMapiObjectCounter userSessionCounter;

		private Exception e1;

		private Exception e2;

		private Exception e3;
	}
}
