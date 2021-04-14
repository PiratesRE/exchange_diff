using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderSessionCache : DisposeTrackableBase
	{
		public PublicFolderSessionCache(OrganizationId organizationId, ExchangePrincipal connectAsPrincipal, ClientSecurityContext clientSecurityContext, CultureInfo cultureInfo, string clientInfoString, IBudget budget, ExTimeZone timeZone, bool openSessionAsAdmin)
		{
			Util.ThrowOnNullArgument(organizationId, "organizationId");
			if (!openSessionAsAdmin && connectAsPrincipal == null)
			{
				throw new ArgumentException("connectAsPrincipal must be set if openSessionAsAdmin is false");
			}
			this.organizationId = organizationId;
			this.connectAsPrincipal = connectAsPrincipal;
			this.clientSecurityContext = clientSecurityContext;
			this.cultureInfo = cultureInfo;
			this.clientInfoString = clientInfoString;
			this.budget = budget;
			this.timeZone = timeZone;
			this.openSessionAsAdmin = openSessionAsAdmin;
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				lock (this.lockObject)
				{
					foreach (PublicFolderSession publicFolderSession in this.openedSessions.Values)
					{
						publicFolderSession.Dispose();
					}
					this.openedSessions.Clear();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PublicFolderSessionCache>(this);
		}

		public void DisconnectAllSessions()
		{
			using (this.CheckDisposed("DisconnectAllSessions"))
			{
				lock (this.lockObject)
				{
					foreach (PublicFolderSession publicFolderSession in this.openedSessions.Values)
					{
						if (publicFolderSession.IsConnected)
						{
							publicFolderSession.Disconnect();
						}
					}
				}
			}
		}

		public void UpdateTimeZoneOnAllSessions(ExTimeZone timeZone)
		{
			using (this.CheckDisposed("UpdateTimeZoneOnAllSessions"))
			{
				lock (this.lockObject)
				{
					foreach (PublicFolderSession publicFolderSession in this.openedSessions.Values)
					{
						publicFolderSession.ExTimeZone = timeZone;
					}
				}
			}
		}

		public PublicFolderSession GetPublicFolderHierarchySession()
		{
			PublicFolderSession publicFolderSession;
			using (this.CheckDisposed("GetPublicFolderHierarchySession"))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)this.GetHashCode(), "PublicFolderSessionCache.GetPublicFolderHierarchySession called");
				publicFolderSession = this.GetPublicFolderSession(PublicFolderSession.HierarchyMailboxGuidAlias);
			}
			return publicFolderSession;
		}

		public PublicFolderSession GetPublicFolderSession(Guid publicFolderMailboxGuid)
		{
			PublicFolderSession result;
			using (this.CheckDisposed("GetPublicFolderSession"))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)this.GetHashCode(), "PublicFolderSessionCache.GetPublicFolderContentSession called");
				if (publicFolderMailboxGuid == PublicFolderSession.HierarchyMailboxGuidAlias)
				{
					publicFolderMailboxGuid = this.GetHierarchyMailboxGuidForUser();
				}
				PublicFolderSession publicFolderSession;
				if (this.openedSessions.TryGetValue(publicFolderMailboxGuid, out publicFolderSession))
				{
					result = publicFolderSession;
				}
				else
				{
					lock (this.lockObject)
					{
						if (this.openedSessions.TryGetValue(publicFolderMailboxGuid, out publicFolderSession))
						{
							return publicFolderSession;
						}
						if (this.openSessionAsAdmin)
						{
							publicFolderSession = PublicFolderSession.OpenAsAdmin(this.organizationId, this.connectAsPrincipal, publicFolderMailboxGuid, (this.clientSecurityContext == null) ? null : new WindowsPrincipal(this.clientSecurityContext.Identity), this.cultureInfo, this.clientInfoString, this.budget);
						}
						else
						{
							publicFolderSession = PublicFolderSession.Open(this.connectAsPrincipal, publicFolderMailboxGuid, this.clientSecurityContext, this.cultureInfo, this.clientInfoString);
							publicFolderSession.AccountingObject = this.budget;
						}
						if (this.timeZone != null)
						{
							publicFolderSession.ExTimeZone = this.timeZone;
						}
						if (this.firstPublicFolderSession == null)
						{
							this.firstPublicFolderSession = publicFolderSession;
						}
						this.openedSessions.Add(publicFolderSession.MailboxGuid, publicFolderSession);
					}
					result = publicFolderSession;
				}
			}
			return result;
		}

		public PublicFolderSession GetPublicFolderSession(StoreId folderId)
		{
			PublicFolderSession result;
			using (this.CheckDisposed("GetPublicFolderSession"))
			{
				ExTraceGlobals.SessionTracer.TraceDebug<StoreId>((long)this.GetHashCode(), "PublicFolderSessionCache.GetPublicFolderSession called for folder id: {0}", folderId);
				bool flag = false;
				Guid hierarchyMailboxGuidForUser = this.GetHierarchyMailboxGuidForUser();
				if (this.firstPublicFolderSession != null && this.firstPublicFolderSession.MailboxGuid != hierarchyMailboxGuidForUser)
				{
					try
					{
						using (Folder folder = Folder.Bind(this.firstPublicFolderSession, folderId))
						{
							flag = true;
							if (folder.IsContentAvailable())
							{
								return this.firstPublicFolderSession;
							}
							PublicFolderContentMailboxInfo contentMailboxInfo = folder.GetContentMailboxInfo();
							if (!contentMailboxInfo.IsValid)
							{
								throw new InvalidOperationException(string.Format("IsContentAvailable() should have returned true if content mailbox property (value={0}) was not parse-able as a guid", contentMailboxInfo));
							}
							return this.GetPublicFolderSession(contentMailboxInfo.MailboxGuid);
						}
					}
					catch (ObjectNotFoundException)
					{
					}
				}
				PublicFolderSession publicFolderSession = null;
				if (!flag)
				{
					publicFolderSession = this.GetPublicFolderHierarchySession();
					using (Folder folder2 = Folder.Bind(publicFolderSession, folderId))
					{
						if (!folder2.IsContentAvailable())
						{
							PublicFolderContentMailboxInfo contentMailboxInfo2 = folder2.GetContentMailboxInfo();
							if (!contentMailboxInfo2.IsValid)
							{
								throw new InvalidOperationException(string.Format("IsContentAvailable() should have returned true if content mailbox property (value={0}) was not parse-able as a guid", contentMailboxInfo2));
							}
							return this.GetPublicFolderSession(contentMailboxInfo2.MailboxGuid);
						}
					}
				}
				result = publicFolderSession;
			}
			return result;
		}

		private ObjectAccessGuard CheckDisposed(string methodName)
		{
			if (base.IsDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(this.ToString());
			}
			return ObjectAccessGuard.Create(this.threadTracker, methodName);
		}

		private Guid GetHierarchyMailboxGuidForUser()
		{
			Guid result;
			using (this.CheckDisposed("GetHierarchyMailboxGuidForUser"))
			{
				ExTraceGlobals.SessionTracer.TraceDebug((long)this.GetHashCode(), "PublicFolderSessionCache.GetHierarchyMailboxGuidForUser called");
				Guid empty = Guid.Empty;
				bool flag;
				if (this.openSessionAsAdmin)
				{
					flag = PublicFolderSession.TryGetPrimaryHierarchyMailboxGuid(this.organizationId, out empty);
				}
				else
				{
					flag = PublicFolderSession.TryGetHierarchyMailboxGuidForUser(this.organizationId, this.connectAsPrincipal.MailboxInfo.MailboxGuid, this.connectAsPrincipal.DefaultPublicFolderMailbox, out empty);
				}
				if (!flag)
				{
					throw new ObjectNotFoundException(PublicFolderSession.GetNoPublicFoldersProvisionedError(this.organizationId));
				}
				result = empty;
			}
			return result;
		}

		private readonly ObjectThreadTracker threadTracker = new ObjectThreadTracker();

		private readonly Dictionary<Guid, PublicFolderSession> openedSessions = new Dictionary<Guid, PublicFolderSession>(2);

		private readonly OrganizationId organizationId;

		private readonly ExchangePrincipal connectAsPrincipal;

		private readonly ClientSecurityContext clientSecurityContext;

		private readonly CultureInfo cultureInfo;

		private readonly string clientInfoString;

		private readonly IBudget budget;

		private readonly ExTimeZone timeZone;

		private readonly bool openSessionAsAdmin;

		private readonly object lockObject = new object();

		private PublicFolderSession firstPublicFolderSession;
	}
}
