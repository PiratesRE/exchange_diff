using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.EventLogs;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MethodWideStoreSessionCache : StoreSessionCacheBase, IDisposable
	{
		public MethodWideStoreSessionCache(AppWideStoreSessionCache backingCache, CallContext callContext)
		{
			this.backingCache = backingCache;
			this.callContext = callContext;
			this.isForExternalUser = callContext.IsExternalUser;
		}

		public bool ReturnStoreSessionsToCache
		{
			get
			{
				return this.returnStoreSessionsToCache;
			}
			set
			{
				this.returnStoreSessionsToCache = value;
			}
		}

		private string BuildCacheKey(Guid mailboxGuid, bool unifiedLogon)
		{
			CultureInfo clientCulture = this.callContext.ClientCulture;
			LogonType logonType = this.callContext.LogonType;
			string text = string.Empty;
			if (logonType == LogonType.Admin)
			{
				text = "_a";
			}
			else if (logonType == LogonType.SystemService)
			{
				text = "_s";
			}
			string text2 = unifiedLogon ? "_unified" : string.Empty;
			return string.Format(CultureInfo.InvariantCulture, "{0}_{1}{2}{3}", new object[]
			{
				mailboxGuid,
				clientCulture.Name,
				text,
				text2
			});
		}

		public StoreSession GetCachedMailboxSession(MailboxId mailboxId)
		{
			return this.GetCachedMailboxSession(mailboxId, false);
		}

		public StoreSession GetCachedMailboxSession(MailboxId mailboxId, bool unifiedLogon)
		{
			if (!this.isForExternalUser)
			{
				if (mailboxId.MailboxGuid != null)
				{
					return this.GetCachedStoreSessionByMailboxGuid(new Guid(mailboxId.MailboxGuid), unifiedLogon);
				}
				return this.GetCachedMailboxSessionBySmtpAddress(mailboxId.SmtpAddress, false, unifiedLogon);
			}
			else
			{
				if (mailboxId.MailboxGuid != null)
				{
					SessionAndAuthZ cachedSystemMailboxSessionByMailboxGuid = this.GetCachedSystemMailboxSessionByMailboxGuid(new Guid(mailboxId.MailboxGuid));
					return cachedSystemMailboxSessionByMailboxGuid.Session;
				}
				ExTraceGlobals.SessionCacheTracer.TraceError<string>((long)this.GetHashCode(), "MethodWideMailboxSessionCache.GetCachedMailboxSession. MailboxId for external user doesn't contain MailboxGuid. MailboxId.SmtpAddress: {0}", mailboxId.SmtpAddress ?? "<Null>");
				throw new ServiceAccessDeniedException();
			}
		}

		public MailboxSession GetCachedMailboxSessionBySmtpAddress(string mailboxSmtpAddress, bool archiveMailbox)
		{
			return this.GetCachedMailboxSessionBySmtpAddress(mailboxSmtpAddress, archiveMailbox, false);
		}

		public MailboxSession GetCachedMailboxSessionBySmtpAddress(string mailboxSmtpAddress, bool archiveMailbox, bool unifiedLogon)
		{
			string effectiveAccessingSmtpAddress = this.callContext.GetEffectiveAccessingSmtpAddress();
			string text;
			if (!string.IsNullOrEmpty(mailboxSmtpAddress))
			{
				text = mailboxSmtpAddress;
			}
			else
			{
				text = effectiveAccessingSmtpAddress;
				if (string.IsNullOrEmpty(text))
				{
					throw new MissingEmailAddressForDistinguishedFolderException();
				}
			}
			ExTraceGlobals.SessionCacheTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "MethodWideMailboxSessionCache.GetCachedMailboxSessionBySmtpAddress() mailboxPrincipalSmtpAddress: {0}, archive {1} ", text, archiveMailbox);
			if (this.callContext.IsRequestProxiedFromDifferentResourceForest)
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug<string>((long)this.GetHashCode(), "MethodWideMailboxSessionCache.GetCachedMailboxSessionBySmtpAddress() This request was proxied from a different resource forest. Returning NULL mailbox session for: {0} since the accessing principal is NULL.", text);
				return null;
			}
			ExchangePrincipal exchangePrincipal = ExchangePrincipalCache.GetFromCache(text, this.callContext.ADRecipientSessionContext);
			if (archiveMailbox)
			{
				IMailboxInfo archiveMailbox2 = exchangePrincipal.GetArchiveMailbox();
				if (archiveMailbox2 == null)
				{
					throw new FolderNotFoundException();
				}
				exchangePrincipal = ExchangePrincipalCache.GetFromCacheByGuid(archiveMailbox2.MailboxGuid, this.callContext.ADRecipientSessionContext);
			}
			if (archiveMailbox ^ exchangePrincipal.MailboxInfo.IsArchive)
			{
				throw new FolderNotFoundException();
			}
			MailboxSession result;
			try
			{
				result = (this.GetCachedStoreSessionByMailboxGuid(exchangePrincipal.MailboxInfo.MailboxGuid, unifiedLogon) as MailboxSession);
			}
			catch (WrongServerException)
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "MethodWideMailboxSessionCache.GetCachedMailboxSessionBySmtpAddress() Removing extra userIdentity entries");
				ADIdentityInformationCache.Singleton.RemoveExtraUserIdentity(text, this.callContext.ADRecipientSessionContext.OrganizationPrefix);
				throw;
			}
			return result;
		}

		public StoreSession GetCachedPublicFolderSessionByStoreId(StoreId folderId)
		{
			PublicFolderSession result;
			if (this.firstPublicFolderSession != null && this.TryGetPublicFolderSessionForTheStoreId(this.firstPublicFolderSession, folderId, out result))
			{
				return result;
			}
			Guid empty = Guid.Empty;
			if (!PublicFolderSession.TryGetHierarchyMailboxGuidForUser(this.callContext.AccessingPrincipal.MailboxInfo.OrganizationId, this.callContext.AccessingPrincipal.MailboxInfo.MailboxGuid, this.callContext.AccessingPrincipal.DefaultPublicFolderMailbox, out empty))
			{
				throw new ObjectNotFoundException(PublicFolderSession.GetNoPublicFoldersProvisionedError(this.callContext.AccessingPrincipal.MailboxInfo.OrganizationId));
			}
			PublicFolderSession publicFolderSession = (PublicFolderSession)this.GetCachedStoreSessionByMailboxGuid(empty);
			if (this.TryGetPublicFolderSessionForTheStoreId(publicFolderSession, folderId, out result))
			{
				return result;
			}
			throw new ObjectNotFoundException(ServerStrings.ExItemNotFound);
		}

		private bool TryGetPublicFolderSessionForTheStoreId(PublicFolderSession publicFolderSession, StoreId folderId, out PublicFolderSession publicFolderSessionForStoreId)
		{
			publicFolderSessionForStoreId = null;
			try
			{
				using (Folder folder = Folder.Bind(publicFolderSession, folderId))
				{
					if (!folder.IsContentAvailable())
					{
						PublicFolderContentMailboxInfo contentMailboxInfo = folder.GetContentMailboxInfo();
						if (!contentMailboxInfo.IsValid)
						{
							throw new InvalidOperationException(string.Format("IsContentAvailable() should have returned true if content mailbox property (value={0}) was not parse-able as a guid", contentMailboxInfo));
						}
						publicFolderSessionForStoreId = (PublicFolderSession)this.GetCachedStoreSessionByMailboxGuid(contentMailboxInfo.MailboxGuid);
					}
					else
					{
						publicFolderSessionForStoreId = publicFolderSession;
					}
				}
			}
			catch (ObjectNotFoundException)
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "StoreId {0} was not found in the mailbox {1}", StoreId.GetStoreObjectId(folderId).ToHexEntryId(), publicFolderSession.MailboxGuid);
			}
			return publicFolderSessionForStoreId != null;
		}

		public StoreSession GetCachedStoreSessionByMailboxGuid(Guid mailboxGuid)
		{
			return this.GetCachedStoreSessionByMailboxGuid(mailboxGuid, false);
		}

		public StoreSession GetCachedStoreSessionByMailboxGuid(Guid mailboxGuid, bool unifiedLogon)
		{
			ExchangePrincipal exchangePrincipal = null;
			SessionAndAuthZ sessionAndAuthZ = null;
			string text = this.BuildCacheKey(mailboxGuid, unifiedLogon);
			if (this.cache.TryGetValue(text, out sessionAndAuthZ))
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug<string, int>((long)this.GetHashCode(), "MethodWideMailboxSessionCache.GetCachedMailboxSessionByMailboxGuid() Cache Hit! Cache Hit! CacheKey: {0} CacheSize: {1}", text, this.cache.Count);
			}
			else
			{
				if (this.UseBackingCache)
				{
					try
					{
						sessionAndAuthZ = this.backingCache.GetCachedMailboxSessionByGuid(mailboxGuid, this.callContext, unifiedLogon);
						goto IL_F4;
					}
					catch (MapiExceptionLogonFailed)
					{
						ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_UnableToOpenCachedMailbox, null, new object[]
						{
							exchangePrincipal.MailboxInfo.DisplayName
						});
						ExTraceGlobals.SessionCacheTracer.TraceDebug<string>((long)this.GetHashCode(), "MethodWideMailboxSessionCache.GetCachedMailboxSessionByMailboxGuid() Unable to open cached mailbox for {0}a ", exchangePrincipal.MailboxInfo.DisplayName);
						throw;
					}
				}
				exchangePrincipal = ExchangePrincipalCache.GetFromCacheByGuid(mailboxGuid, this.callContext.ADRecipientSessionContext);
				if (exchangePrincipal == null)
				{
					throw new NonExistentMailboxException((CoreResources.IDs)3279543955U, mailboxGuid.ToString());
				}
				sessionAndAuthZ = base.CreateMailboxSessionBasedOnAccessType(exchangePrincipal, this.callContext, unifiedLogon);
				IL_F4:
				ExTraceGlobals.SessionCacheTracer.TraceDebug<Guid>((long)this.GetHashCode(), "MethodWideMailboxSessionCache.GetCachedMailboxSessionByMailboxGuid() Cache MISS. MailboxGuid {0}", mailboxGuid);
				this.cache.Add(text, sessionAndAuthZ);
			}
			StoreSession session = sessionAndAuthZ.Session;
			base.SafeConnect(session);
			this.SetClientIPEndpoint(session, this.callContext.MailboxAccessType == MailboxAccessType.ServerToServer);
			if (session.IsPublicFolderSession && this.firstPublicFolderSession == null)
			{
				this.firstPublicFolderSession = (PublicFolderSession)session;
			}
			return session;
		}

		public SessionAndAuthZ GetCachedSystemMailboxSessionByMailboxGuid(Guid mailboxGuid)
		{
			return this.GetCachedSystemMailboxSessionByMailboxGuid(mailboxGuid, false);
		}

		public SessionAndAuthZ GetCachedSystemMailboxSessionByMailboxGuid(Guid mailboxGuid, bool unifiedLogon)
		{
			string text = this.BuildCacheKey(mailboxGuid, unifiedLogon);
			SessionAndAuthZ sessionAndAuthZ;
			if (this.cache.TryGetValue(text, out sessionAndAuthZ))
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug<string, int>((long)this.GetHashCode(), "MethodWideMailboxSessionCache.GetCachedSystemMailboxSessionBySmtpAddress() Cache Hit! Cache Hit! CacheKey: {0} CacheSize: {1}", text, this.cache.Count);
			}
			else
			{
				ExTraceGlobals.SessionCacheTracer.TraceDebug<Guid>((long)this.GetHashCode(), "MethodWideMailboxSessionCache.GetCachedSystemMailboxSessionBySmtpAddress() Cache MISS. MailboxGuid {0}", mailboxGuid);
				ExchangePrincipal fromCacheByGuid = ExchangePrincipalCache.GetFromCacheByGuid(mailboxGuid, this.callContext.ADRecipientSessionContext);
				if (fromCacheByGuid == null)
				{
					throw new ServiceAccessDeniedException();
				}
				sessionAndAuthZ = base.ConnectOnceAsExternalPrincipal(fromCacheByGuid, (ExternalCallContext)this.callContext);
				this.cache.Add(text, sessionAndAuthZ);
			}
			base.SafeConnect((MailboxSession)sessionAndAuthZ.Session);
			this.SetClientIPEndpoint(sessionAndAuthZ.Session);
			return sessionAndAuthZ;
		}

		internal SessionAndAuthZ GetSessionAndAuthZ(Guid mailboxGuid, bool unifiedLogon)
		{
			string text = this.BuildCacheKey(mailboxGuid, unifiedLogon);
			ExTraceGlobals.SessionCacheTracer.TraceDebug<string>((long)this.GetHashCode(), "MethodWideMailboxSessionCache.GetSessionAndAuthZ called for key: {0}", text);
			return this.cache[text];
		}

		protected override void Dispose(bool isDisposing)
		{
			ExTraceGlobals.SessionCacheTracer.TraceDebug<int, bool>((long)this.GetHashCode(), "MethodWideMailboxSessionCache.Dispose. Hashcode: {0}. IsDisposing: {1}", this.GetHashCode(), isDisposing);
			if (!this.isDisposed)
			{
				if (this.UseBackingCache && this.ReturnStoreSessionsToCache)
				{
					using (Dictionary<string, SessionAndAuthZ>.Enumerator enumerator = this.cache.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, SessionAndAuthZ> keyValuePair = enumerator.Current;
							keyValuePair.Value.Session.AccountingObject = null;
							this.backingCache.ReleaseMailboxSession(keyValuePair.Value, this.callContext);
						}
						goto IL_106;
					}
				}
				if (!this.ReturnStoreSessionsToCache)
				{
					ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "[MethodWideMailboxSessionCache::Dispose] Encountered a Cross-Site failover.  Discarding all MailboxSessions.");
				}
				try
				{
					ExTraceGlobals.SessionCacheTracer.TraceDebug((long)this.GetHashCode(), "[MethodWideMailboxSessionCache::Dispose] Disposing pairs in cache.");
				}
				finally
				{
					foreach (KeyValuePair<string, SessionAndAuthZ> keyValuePair2 in this.cache)
					{
						keyValuePair2.Value.Dispose();
					}
				}
				IL_106:
				this.cache.Clear();
				this.isDisposed = true;
			}
		}

		private bool UseBackingCache
		{
			get
			{
				return !this.isForExternalUser && this.backingCache != null;
			}
		}

		private void SetClientIPEndpoint(StoreSession session, bool serverToServer)
		{
			GccUtils.SetStoreSessionClientIPEndpointsFromHttpRequest(session, MethodWideStoreSessionCache.GetCurrentHttpRequest(), serverToServer);
		}

		private void SetClientIPEndpoint(StoreSession session)
		{
			GccUtils.SetStoreSessionClientIPEndpointsFromHttpRequest(session, MethodWideStoreSessionCache.GetCurrentHttpRequest());
		}

		private static HttpRequest GetCurrentHttpRequest()
		{
			CallContext callContext = CallContext.Current;
			if (callContext != null && !callContext.IsDetachedFromRequest)
			{
				return callContext.HttpContext.Request;
			}
			return null;
		}

		private readonly bool isForExternalUser;

		private Dictionary<string, SessionAndAuthZ> cache = new Dictionary<string, SessionAndAuthZ>(StringComparer.OrdinalIgnoreCase);

		private AppWideStoreSessionCache backingCache;

		private CallContext callContext;

		private bool returnStoreSessionsToCache = true;

		private PublicFolderSession firstPublicFolderSession;
	}
}
