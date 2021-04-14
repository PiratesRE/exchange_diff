using System;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal class QueryableSession
	{
		private QueryableSession(Guid sessionId, string applicationId, int maxRecipients, bool shouldBeCommitted, int rpcContext, string userDn, Guid userGuid, string userSid, string clientVersion, string clientMachineName, string clientMode, string clientProcessName, Guid lastClientActivityId, string lastClientProtocol, string lastClientComponent, string lastClientAction, string lastUsedDatabase, byte lastUsedLogonIndex, string lastRememberedExceptionType, string lastRememberedExceptionMessage, string lastRememberedExceptionStack, DateTime connectTime, DateTime lastAccessTime, int codePage, int lcidSort, int lcidString, bool canConvertCodePage, bool usingDelegatedAuth, bool usingTransportPrivilege, bool usingAdminPrivilege, bool usingLogonAdminPrivilege, bool canAcceptRops, bool inRpc, bool isValid, int logonObjectCount, long messageObjectCount, long attachmentObjectCount, long folderObjectCount, long notifyObjectCount, long streamObjectCount, long messageViewCount, long folderViewCount, long attachmentViewCount, long permissionViewCount, long fastTransferSourceObjectCount, long fastTransferDestinationObjectCount, long untrackedObjectCount)
		{
			this.sessionId = sessionId;
			this.applicationId = applicationId;
			this.maxRecipients = maxRecipients;
			this.shouldBeCommitted = shouldBeCommitted;
			this.rpcContext = rpcContext;
			this.userDn = userDn;
			this.userGuid = userGuid;
			this.userSid = userSid;
			this.clientVersion = clientVersion;
			this.clientMachineName = clientMachineName;
			this.clientMode = clientMode;
			this.clientProcessName = clientProcessName;
			this.lastClientActivityId = lastClientActivityId;
			this.lastClientProtocol = lastClientProtocol;
			this.lastClientComponent = lastClientComponent;
			this.lastClientAction = lastClientAction;
			this.lastUsedDatabase = lastUsedDatabase;
			this.lastUsedLogonIndex = lastUsedLogonIndex;
			this.lastRememberedExceptionType = lastRememberedExceptionType;
			this.lastRememberedExceptionMessage = lastRememberedExceptionMessage;
			this.lastRememberedExceptionStack = lastRememberedExceptionStack;
			this.connectTime = connectTime;
			this.lastAccessTime = lastAccessTime;
			this.codePage = codePage;
			this.lcidSort = lcidSort;
			this.lcidString = lcidString;
			this.canConvertCodePage = canConvertCodePage;
			this.usingDelegatedAuth = usingDelegatedAuth;
			this.usingTransportPrivilege = usingTransportPrivilege;
			this.usingAdminPrivilege = usingAdminPrivilege;
			this.usingLogonAdminPrivilege = usingLogonAdminPrivilege;
			this.canAcceptRops = canAcceptRops;
			this.inRpc = inRpc;
			this.isValid = isValid;
			this.logonObjectCount = logonObjectCount;
			this.messageObjectCount = messageObjectCount;
			this.attachmentObjectCount = attachmentObjectCount;
			this.folderObjectCount = folderObjectCount;
			this.notifyObjectCount = notifyObjectCount;
			this.streamObjectCount = streamObjectCount;
			this.messageViewCount = messageViewCount;
			this.folderViewCount = folderViewCount;
			this.attachmentViewCount = attachmentViewCount;
			this.permissionViewCount = permissionViewCount;
			this.fastTransferSourceObjectCount = fastTransferSourceObjectCount;
			this.fastTransferDestinationObjectCount = fastTransferDestinationObjectCount;
			this.untrackedObjectCount = untrackedObjectCount;
		}

		[Queryable(Index = 0)]
		public Guid SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		[Queryable]
		public string ApplicationId
		{
			get
			{
				return this.applicationId;
			}
		}

		[Queryable]
		public int MaxRecipients
		{
			get
			{
				return this.maxRecipients;
			}
		}

		[Queryable]
		public bool ShouldBeCommitted
		{
			get
			{
				return this.shouldBeCommitted;
			}
		}

		[Queryable]
		public int RpcContext
		{
			get
			{
				return this.rpcContext;
			}
		}

		[Queryable]
		public string UserDN
		{
			get
			{
				return this.userDn;
			}
		}

		[Queryable]
		public Guid UserGuid
		{
			get
			{
				return this.userGuid;
			}
		}

		[Queryable]
		public string UserSid
		{
			get
			{
				return this.userSid;
			}
		}

		[Queryable]
		public string ClientVersion
		{
			get
			{
				return this.clientVersion;
			}
		}

		[Queryable]
		public string ClientMachineName
		{
			get
			{
				return this.clientMachineName;
			}
		}

		[Queryable]
		public string ClientMode
		{
			get
			{
				return this.clientMode;
			}
		}

		[Queryable]
		public string ClientProcessName
		{
			get
			{
				return this.clientProcessName;
			}
		}

		[Queryable]
		public Guid LastClientActivityId
		{
			get
			{
				return this.lastClientActivityId;
			}
		}

		[Queryable]
		public string LastClientProtocol
		{
			get
			{
				return this.lastClientProtocol;
			}
		}

		[Queryable]
		public string LastClientComponent
		{
			get
			{
				return this.lastClientComponent;
			}
		}

		[Queryable]
		public string LastClientAction
		{
			get
			{
				return this.lastClientAction;
			}
		}

		[Queryable]
		public string LastUsedDatabase
		{
			get
			{
				return this.lastUsedDatabase;
			}
		}

		[Queryable]
		public byte LastUsedLogonIndex
		{
			get
			{
				return this.lastUsedLogonIndex;
			}
		}

		[Queryable]
		public string LastRememberedExceptionType
		{
			get
			{
				return this.lastRememberedExceptionType;
			}
		}

		[Queryable]
		public string LastRememberedExceptionMessage
		{
			get
			{
				return this.lastRememberedExceptionMessage;
			}
		}

		[Queryable]
		public string LastRememberedExceptionStack
		{
			get
			{
				return this.lastRememberedExceptionStack;
			}
		}

		[Queryable]
		public DateTime ConnectTime
		{
			get
			{
				return this.connectTime;
			}
		}

		[Queryable]
		public DateTime LastAccessTime
		{
			get
			{
				return this.lastAccessTime;
			}
		}

		[Queryable]
		public int CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		[Queryable]
		public int LcidSort
		{
			get
			{
				return this.lcidSort;
			}
		}

		[Queryable]
		public int LcidString
		{
			get
			{
				return this.lcidString;
			}
		}

		[Queryable]
		public bool CanConvertCodePage
		{
			get
			{
				return this.canConvertCodePage;
			}
		}

		[Queryable]
		public bool UsingDelegatedAuth
		{
			get
			{
				return this.usingDelegatedAuth;
			}
		}

		[Queryable]
		public bool UsingTransportPrivilege
		{
			get
			{
				return this.usingTransportPrivilege;
			}
		}

		[Queryable]
		public bool UsingAdminPrivilege
		{
			get
			{
				return this.usingAdminPrivilege;
			}
		}

		[Queryable]
		public bool UsingLogonAdminPrivilege
		{
			get
			{
				return this.usingLogonAdminPrivilege;
			}
		}

		[Queryable]
		public bool CanAcceptRops
		{
			get
			{
				return this.canAcceptRops;
			}
		}

		[Queryable]
		public bool InRpc
		{
			get
			{
				return this.inRpc;
			}
		}

		[Queryable]
		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		[Queryable]
		public int LogonObjectCount
		{
			get
			{
				return this.logonObjectCount;
			}
		}

		[Queryable]
		public long MessageObjectCount
		{
			get
			{
				return this.messageObjectCount;
			}
		}

		[Queryable]
		public long AttachmentObjectCount
		{
			get
			{
				return this.attachmentObjectCount;
			}
		}

		[Queryable]
		public long FolderObjectCount
		{
			get
			{
				return this.folderObjectCount;
			}
		}

		[Queryable]
		public long NotifyObjectCount
		{
			get
			{
				return this.notifyObjectCount;
			}
		}

		[Queryable]
		public long StreamObjectCount
		{
			get
			{
				return this.streamObjectCount;
			}
		}

		[Queryable]
		public long MessageViewCount
		{
			get
			{
				return this.messageViewCount;
			}
		}

		[Queryable]
		public long FolderViewCount
		{
			get
			{
				return this.folderViewCount;
			}
		}

		[Queryable]
		public long AttachmentViewCount
		{
			get
			{
				return this.attachmentViewCount;
			}
		}

		[Queryable]
		public long PermissionViewCount
		{
			get
			{
				return this.permissionViewCount;
			}
		}

		[Queryable]
		public long FastTransferSourceObjectCount
		{
			get
			{
				return this.fastTransferSourceObjectCount;
			}
		}

		[Queryable]
		public long FastTransferDestinationObjectCount
		{
			get
			{
				return this.fastTransferDestinationObjectCount;
			}
		}

		[Queryable]
		public long UntrackedObjectCount
		{
			get
			{
				return this.untrackedObjectCount;
			}
		}

		public static QueryableSession Create(MapiSession session)
		{
			string text;
			string text2;
			string text3;
			ErrorHelper.GetExceptionSummary(session.LastRememberedException, out text, out text2, out text3);
			return QueryableSession.Create(session.SessionId, session.ApplicationId, session.MaxRecipients, session.ShouldBeCommitted, session.RpcContext, session.UserDN, session.UserGuid, session.KeyCurrentSecurityContext.ToString(), string.Format("{0}.{1:00}.{2:0000}.{3:000}", new object[]
			{
				session.ClientVersion.ProductMajor,
				session.ClientVersion.ProductMinor,
				session.ClientVersion.BuildMajor,
				session.ClientVersion.BuildMinor
			}), session.ClientMachineName, session.ClientMode.ToString(), session.ClientProcessName, session.LastClientActivityId, session.LastClientProtocol, session.LastClientComponent, session.LastClientAction, (session.LastUsedDatabase != null) ? session.LastUsedDatabase.MdbName : string.Empty, session.LastUsedLogonIndex, text, text2, text3, session.ConnectTime, session.LastAccessTime, (int)session.CodePage, session.LcidSort, session.LcidString, session.CanConvertCodePage, session.UsingDelegatedAuth, session.UsingTransportPrivilege, session.UsingAdminPrivilege, session.UsingLogonAdminPrivilege, session.CanAcceptROPs, session.InRpc, session.IsValid, session.LogonCount, session.GetPerSessionObjectCounter(MapiObjectTrackedType.Message).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.Attachment).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.Folder).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.Notify).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.Stream).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.MessageView).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.FolderView).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.AttachmentView).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.PermissionView).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.FastTransferSource).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.FastTransferDestination).GetCount(), session.GetPerSessionObjectCounter(MapiObjectTrackedType.UntrackedObject).GetCount());
		}

		public static QueryableSession Create(Guid sessionId, string applicationId, int maxRecipients, bool shouldBeCommitted, int rpcContext, string userDn, Guid userGuid, string userSid, string clientVersion, string clientMachineName, string clientMode, string clientProcessName, Guid lastClientActivityId, string lastClientProtocol, string lastClientComponent, string lastClientAction, string lastUsedDatabase, byte lastUsedLogonIndex, string lastRememberedExceptionType, string lastRememberedExceptionMessage, string lastRememberedExceptionStack, DateTime connectTime, DateTime lastAccessTime, int codePage, int lcidSort, int lcidString, bool canConvertCodePage, bool usingDelegatedAuth, bool usingTransportPrivilege, bool usingAdminPrivilege, bool usingLogonAdminPrivilege, bool canAcceptRops, bool inRpc, bool isValid, int logonObjectCount, long messageObjectCount, long attachmentObjectCount, long folderObjectCount, long notifyObjectCount, long streamObjectCount, long messageViewCount, long folderViewCount, long attachmentViewCount, long permissionViewCount, long fastTransferSourceObjectCount, long fastTransferDestinationObjectCount, long untrackedObjectCount)
		{
			return new QueryableSession(sessionId, applicationId, maxRecipients, shouldBeCommitted, rpcContext, userDn, userGuid, userSid, clientVersion, clientMachineName, clientMode, clientProcessName, lastClientActivityId, lastClientProtocol, lastClientComponent, lastClientAction, lastUsedDatabase, lastUsedLogonIndex, lastRememberedExceptionType, lastRememberedExceptionMessage, lastRememberedExceptionStack, connectTime, lastAccessTime, codePage, lcidSort, lcidString, canConvertCodePage, usingDelegatedAuth, usingTransportPrivilege, usingAdminPrivilege, usingLogonAdminPrivilege, canAcceptRops, inRpc, isValid, logonObjectCount, messageObjectCount, attachmentObjectCount, folderObjectCount, notifyObjectCount, streamObjectCount, messageViewCount, folderViewCount, attachmentViewCount, permissionViewCount, fastTransferSourceObjectCount, fastTransferDestinationObjectCount, untrackedObjectCount);
		}

		private readonly Guid sessionId;

		private readonly string applicationId;

		private readonly int maxRecipients;

		private readonly bool shouldBeCommitted;

		private readonly int rpcContext;

		private readonly string userDn;

		private readonly Guid userGuid;

		private readonly string userSid;

		private readonly string clientVersion;

		private readonly string clientMachineName;

		private readonly string clientMode;

		private readonly string clientProcessName;

		private readonly Guid lastClientActivityId;

		private readonly string lastClientProtocol;

		private readonly string lastClientComponent;

		private readonly string lastClientAction;

		private readonly string lastUsedDatabase;

		private readonly byte lastUsedLogonIndex;

		private readonly string lastRememberedExceptionType;

		private readonly string lastRememberedExceptionMessage;

		private readonly string lastRememberedExceptionStack;

		private readonly DateTime connectTime;

		private readonly DateTime lastAccessTime;

		private readonly int codePage;

		private readonly int lcidSort;

		private readonly int lcidString;

		private readonly bool canConvertCodePage;

		private readonly bool usingDelegatedAuth;

		private readonly bool usingTransportPrivilege;

		private readonly bool usingAdminPrivilege;

		private readonly bool usingLogonAdminPrivilege;

		private readonly bool canAcceptRops;

		private readonly bool inRpc;

		private readonly bool isValid;

		private readonly int logonObjectCount;

		private readonly long messageObjectCount;

		private readonly long attachmentObjectCount;

		private readonly long folderObjectCount;

		private readonly long notifyObjectCount;

		private readonly long streamObjectCount;

		private readonly long messageViewCount;

		private readonly long folderViewCount;

		private readonly long attachmentViewCount;

		private readonly long permissionViewCount;

		private readonly long fastTransferSourceObjectCount;

		private readonly long fastTransferDestinationObjectCount;

		private readonly long untrackedObjectCount;
	}
}
