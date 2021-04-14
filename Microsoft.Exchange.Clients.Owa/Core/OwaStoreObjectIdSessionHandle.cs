using System;
using System.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class OwaStoreObjectIdSessionHandle : DisposeTrackableBase
	{
		private void InitializeDelegateSessionHandle(ExchangePrincipal exchangePrincipal, UserContext userContext)
		{
			try
			{
				this.delegateSessionHandle = userContext.GetDelegateSessionHandle(exchangePrincipal);
			}
			catch (MailboxInSiteFailoverException exception)
			{
				this.HandleOtherMailboxFailover(exception, exchangePrincipal.LegacyDn);
			}
			catch (MailboxCrossSiteFailoverException exception2)
			{
				this.HandleOtherMailboxFailover(exception2, exchangePrincipal.LegacyDn);
			}
			catch (NotSupportedWithServerVersionException innerException)
			{
				throw new OwaSharedFromOlderVersionException(LocalizedStrings.GetHtmlEncoded(1354015881), innerException);
			}
			catch (StoragePermanentException ex)
			{
				string message = string.Format(CultureInfo.InvariantCulture, "OwaStoreObjectIdSessionHandle Ctor. Unable to get GetDelegateSessionHandle with exchange principal from legacy DN {0}. Exception: {1}.", new object[]
				{
					exchangePrincipal.LegacyDn,
					ex.Message
				});
				ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), message);
				userContext.DelegateSessionManager.RemoveInvalidExchangePrincipal(exchangePrincipal.LegacyDn);
			}
			catch (StorageTransientException ex2)
			{
				string message2 = string.Format(CultureInfo.InvariantCulture, "OwaStoreObjectIdSessionHandle Ctor. Unable to get GetDelegateSessionHandle with exchange principal from legacy DN {0}. Exception: {1}.", new object[]
				{
					exchangePrincipal.LegacyDn,
					ex2.Message
				});
				ExTraceGlobals.CoreCallTracer.TraceDebug((long)this.GetHashCode(), message2);
				userContext.DelegateSessionManager.RemoveInvalidExchangePrincipal(exchangePrincipal.LegacyDn);
			}
			if (this.delegateSessionHandle == null || this.Session == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ErrorMissingMailboxOrPermission);
			}
		}

		internal OwaStoreObjectIdSessionHandle(ExchangePrincipal exchangePrincipal, UserContext userContext)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (exchangePrincipal == null)
				{
					throw new ArgumentNullException("exchangePrincipal");
				}
				if (userContext == null)
				{
					throw new ArgumentNullException("userContext");
				}
				if (exchangePrincipal.MailboxInfo.IsArchive)
				{
					throw new ArgumentException("exchangePrincipal is archive mailbox");
				}
				if (exchangePrincipal.MailboxInfo.IsAggregated)
				{
					throw new ArgumentException("exchangePrincipal is aggregated mailbox");
				}
				this.owaStoreObjectId = null;
				this.userContext = userContext;
				this.owaStoreObjectIdType = OwaStoreObjectIdType.OtherUserMailboxObject;
				this.InitializeDelegateSessionHandle(exchangePrincipal, userContext);
				disposeGuard.Success();
			}
		}

		internal OwaStoreObjectIdSessionHandle(OwaStoreObjectId owaStoreObjectId, UserContext userContext)
		{
			using (DisposeGuard disposeGuard = this.Guard())
			{
				if (owaStoreObjectId == null)
				{
					throw new ArgumentNullException("owaStoreObjectId");
				}
				if (userContext == null)
				{
					throw new ArgumentNullException("userContext");
				}
				this.owaStoreObjectId = owaStoreObjectId;
				this.userContext = userContext;
				this.owaStoreObjectIdType = owaStoreObjectId.OwaStoreObjectIdType;
				if (owaStoreObjectId.OwaStoreObjectIdType == OwaStoreObjectIdType.OtherUserMailboxObject || owaStoreObjectId.OwaStoreObjectIdType == OwaStoreObjectIdType.GSCalendar)
				{
					ExchangePrincipal exchangePrincipal = null;
					if (!userContext.DelegateSessionManager.TryGetExchangePrincipal(owaStoreObjectId.MailboxOwnerLegacyDN, out exchangePrincipal))
					{
						throw new ObjectNotFoundException(ServerStrings.CannotFindExchangePrincipal);
					}
					this.InitializeDelegateSessionHandle(exchangePrincipal, userContext);
				}
				disposeGuard.Success();
			}
		}

		public StoreSession Session
		{
			get
			{
				this.CheckDisposed("Session::get.");
				return this.GetCorrelatedSession(false);
			}
		}

		public StoreSession SessionForFolderContent
		{
			get
			{
				this.CheckDisposed("SessionForFolderContent::get.");
				return this.GetSessionForFolderContent();
			}
		}

		public OwaStoreObjectIdType HandleType
		{
			get
			{
				return this.owaStoreObjectIdType;
			}
		}

		private StoreSession GetSessionForFolderContent()
		{
			if (!Folder.IsFolderId(this.owaStoreObjectId.StoreObjectId))
			{
				throw new OwaNotSupportedException("Get session for folder content is not supported for non folder ids");
			}
			return this.GetCorrelatedSession(true);
		}

		private StoreSession GetCorrelatedSession(bool requireContentIfFolder)
		{
			switch (this.owaStoreObjectIdType)
			{
			case OwaStoreObjectIdType.MailBoxObject:
				return this.userContext.MailboxSession;
			case OwaStoreObjectIdType.PublicStoreFolder:
				if (requireContentIfFolder)
				{
					return this.userContext.GetContentAvailableSession(this.owaStoreObjectId.StoreObjectId);
				}
				return this.userContext.DefaultPublicFolderSession;
			case OwaStoreObjectIdType.PublicStoreItem:
				return this.userContext.GetContentAvailableSession(this.owaStoreObjectId.ParentFolderId);
			case OwaStoreObjectIdType.Conversation:
				return this.userContext.MailboxSession;
			case OwaStoreObjectIdType.OtherUserMailboxObject:
			case OwaStoreObjectIdType.GSCalendar:
				Utilities.ReconnectStoreSession(this.delegateSessionHandle.MailboxSession, this.userContext);
				return this.delegateSessionHandle.MailboxSession;
			case OwaStoreObjectIdType.ArchiveMailboxObject:
				return this.userContext.GetArchiveMailboxSession(this.owaStoreObjectId.MailboxOwnerLegacyDN);
			case OwaStoreObjectIdType.ArchiveConversation:
				return this.userContext.GetArchiveMailboxSession(this.owaStoreObjectId.MailboxOwnerLegacyDN);
			default:
				return null;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaStoreObjectIdSessionHandle::InternalDispose");
			if (disposing && !this.isDisposed && this.delegateSessionHandle != null)
			{
				this.delegateSessionHandle.Dispose();
				this.isDisposed = true;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OwaStoreObjectIdSessionHandle>(this);
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaStoreObjectIdSessionHandle::CheckDisposed");
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void HandleOtherMailboxFailover(Exception exception, string mailboxOwnerLegacyDN)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug<string, string>((long)this.GetHashCode(), "OwaStoreObjectIdSessionHandle Ctor. Unable to get GetDelegateSessionHandle with exchange principal from legacy DN {0}. Exception: {1}.", mailboxOwnerLegacyDN, exception.Message);
			this.userContext.DelegateSessionManager.RemoveInvalidExchangePrincipal(mailboxOwnerLegacyDN);
			throw new OwaDelegatorMailboxFailoverException(mailboxOwnerLegacyDN, exception);
		}

		private OwaStoreObjectId owaStoreObjectId;

		private DelegateSessionHandle delegateSessionHandle;

		private UserContext userContext;

		private OwaStoreObjectIdType owaStoreObjectIdType;

		private bool isDisposed;
	}
}
