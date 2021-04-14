using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class SessionAndAuthZ : IDisposable
	{
		public SessionAndAuthZ(StoreSession session, AuthZClientInfo authZClientInfo, CultureInfo clientCultureInfo)
		{
			this.session = session;
			this.authZClientInfo = authZClientInfo;
			this.cultureInfo = clientCultureInfo;
			this.authZClientInfo.AddRef();
			ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "[SessionAndAuthZ::ctor]  AuthZClientInfo for '{0}' is being associated with Session '{1}'.  Adding reference count.", (this.authZClientInfo.ClientSecurityContext == null) ? "Application" : this.authZClientInfo.ClientSecurityContext.UserSid.ToString(), this.session.MailboxGuid);
		}

		public StoreSession Session
		{
			get
			{
				return this.session;
			}
		}

		public AuthZClientInfo ClientInfo
		{
			get
			{
				return this.authZClientInfo;
			}
		}

		public CultureInfo CultureInfo
		{
			get
			{
				return this.cultureInfo;
			}
		}

		public bool IsFromBackingCache
		{
			get
			{
				return this.isFromBackingCache;
			}
			set
			{
				this.isFromBackingCache = value;
				if (this.isFromBackingCache)
				{
					this.refreshedDefaultFolders.Clear();
				}
			}
		}

		public StoreObjectId GetRefreshedDefaultFolderId(DefaultFolderType defaultFolderType)
		{
			MailboxSession mailboxSession = this.session as MailboxSession;
			StoreObjectId storeObjectId;
			if (defaultFolderType == DefaultFolderType.AdminAuditLogs)
			{
				storeObjectId = mailboxSession.GetAdminAuditLogsFolderId();
			}
			else
			{
				storeObjectId = mailboxSession.GetDefaultFolderId(defaultFolderType);
			}
			if (storeObjectId == null && defaultFolderType == DefaultFolderType.RecoverableItemsDeletions)
			{
				DumpsterFolderHelper.CheckAndCreateFolder(mailboxSession);
				storeObjectId = mailboxSession.GetDefaultFolderId(defaultFolderType);
			}
			if (storeObjectId == null && mailboxSession.LogonType == LogonType.Delegated && this.isFromBackingCache && !this.refreshedDefaultFolders.ContainsKey(defaultFolderType))
			{
				storeObjectId = mailboxSession.RefreshDefaultFolder(defaultFolderType);
				this.refreshedDefaultFolders[defaultFolderType] = true;
			}
			if (storeObjectId == null)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<DefaultFolderType>(0L, "GetDefaultFolderId returned null for DefaultFolderType: '{0}'. FolderNotFoundException will be thrown.", defaultFolderType);
				throw new FolderNotFoundException();
			}
			return storeObjectId;
		}

		public void Dispose()
		{
			if (this.session != null)
			{
				this.session.Dispose();
				this.session = null;
			}
			if (this.authZClientInfo != null)
			{
				ExTraceGlobals.ServerToServerAuthZTracer.TraceDebug((long)this.GetHashCode(), "[SessionAndAuthZ::Dispose] AuthZClientInfo member is being disposed of");
				this.authZClientInfo.Dispose();
				this.authZClientInfo = null;
			}
			GC.SuppressFinalize(this);
		}

		private StoreSession session;

		private AuthZClientInfo authZClientInfo;

		private Dictionary<DefaultFolderType, bool> refreshedDefaultFolders = new Dictionary<DefaultFolderType, bool>();

		private bool isFromBackingCache;

		private CultureInfo cultureInfo;
	}
}
