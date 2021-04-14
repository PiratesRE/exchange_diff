using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class SessionCache : IDisposable
	{
		internal SessionCache(AppWideStoreSessionCache storeSessionBackingCache, CallContext callContext)
		{
			this.callContext = callContext;
			if (callContext.IsExternalUser)
			{
				this.storeSessionCache = new MethodWideStoreSessionCache(null, callContext);
				return;
			}
			this.storeSessionCache = new MethodWideStoreSessionCache(storeSessionBackingCache, callContext);
		}

		internal MailboxSession GetMailboxSessionBySmtpAddress(string smtpAddress)
		{
			return this.storeSessionCache.GetCachedMailboxSessionBySmtpAddress(smtpAddress, false);
		}

		internal MailboxSession GetMailboxSessionBySmtpAddress(string smtpAddress, bool archiveMailbox)
		{
			return this.storeSessionCache.GetCachedMailboxSessionBySmtpAddress(smtpAddress, archiveMailbox);
		}

		internal MailboxSession GetMailboxSessionByMailboxId(MailboxId mailboxId)
		{
			return this.GetMailboxSessionByMailboxId(mailboxId, false);
		}

		internal MailboxSession GetMailboxSessionByMailboxId(MailboxId mailboxId, bool unifiedLogon)
		{
			return this.storeSessionCache.GetCachedMailboxSession(mailboxId, unifiedLogon) as MailboxSession;
		}

		internal MailboxSession GetMailboxIdentityMailboxSession()
		{
			return this.GetMailboxIdentityMailboxSession(false);
		}

		internal MailboxSession GetMailboxIdentityMailboxSession(bool archiveMailbox)
		{
			return this.storeSessionCache.GetCachedMailboxSessionBySmtpAddress(string.IsNullOrEmpty(this.callContext.OwaExplicitLogonUser) ? null : this.callContext.OwaExplicitLogonUser, archiveMailbox);
		}

		internal SessionAndAuthZ GetSessionAndAuthZ(Guid mailboxGuid, bool unifiedLogon)
		{
			return this.storeSessionCache.GetSessionAndAuthZ(mailboxGuid, unifiedLogon);
		}

		internal SessionAndAuthZ GetSystemMailboxSessionAndAuthZ(MailboxId mailboxId)
		{
			return this.GetSystemMailboxSessionAndAuthZ(mailboxId, false);
		}

		internal SessionAndAuthZ GetSystemMailboxSessionAndAuthZ(MailboxId mailboxId, bool unifiedLogon)
		{
			if (mailboxId.MailboxGuid != null)
			{
				return this.storeSessionCache.GetCachedSystemMailboxSessionByMailboxGuid(new Guid(mailboxId.MailboxGuid), unifiedLogon);
			}
			ExTraceGlobals.SessionCacheTracer.TraceError<string>((long)this.GetHashCode(), "SessionCache.GetSystemMailboxSessionAndAuthZ. MailboxId for external user doesn't contain MailboxGuid. MailboxId.SmtpAddress: {0}", mailboxId.SmtpAddress ?? "<Null>");
			throw new ServiceAccessDeniedException();
		}

		internal PublicFolderSession GetPublicFolderSession(Guid publicFolderMailboxGuid)
		{
			this.VerifyAccessingPrincipalIsNotNull();
			return this.storeSessionCache.GetCachedStoreSessionByMailboxGuid(publicFolderMailboxGuid) as PublicFolderSession;
		}

		internal PublicFolderSession GetPublicFolderSession(StoreId folderId)
		{
			this.VerifyAccessingPrincipalIsNotNull();
			return this.storeSessionCache.GetCachedPublicFolderSessionByStoreId(folderId) as PublicFolderSession;
		}

		internal bool ReturnStoreSessionsToCache
		{
			get
			{
				return this.storeSessionCache.ReturnStoreSessionsToCache;
			}
			set
			{
				this.storeSessionCache.ReturnStoreSessionsToCache = value;
			}
		}

		private void VerifyAccessingPrincipalIsNotNull()
		{
			if (this.callContext.AccessingPrincipal == null)
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "[SessionCache::VerifyAccessingPrincipalIsNotNull] Cannot obtain the organizationId whenwhen the accessing principal does not represent an Exchange Principal. (DCR E12:120711)");
				throw new ServiceInvalidOperationException(CoreResources.IDs.ErrorPublicFolderUserMustHaveMailbox);
			}
		}

		public void Dispose()
		{
			ExTraceGlobals.SessionCacheTracer.TraceDebug<bool>((long)this.GetHashCode(), "SessionCache.Dispose() called, isDisposed: {0}", this.isDisposed);
			if (!this.isDisposed)
			{
				this.storeSessionCache.Dispose();
				this.isDisposed = true;
				ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "SessionCache.Dispose(), Cache disposed.");
			}
		}

		private bool isDisposed;

		private MethodWideStoreSessionCache storeSessionCache;

		private CallContext callContext;
	}
}
