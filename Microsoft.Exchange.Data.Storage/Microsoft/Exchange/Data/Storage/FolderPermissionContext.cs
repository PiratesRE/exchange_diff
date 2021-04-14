using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class FolderPermissionContext : IDisposeTrackable, IDisposable
	{
		private FolderPermissionContext(MailboxSession mailboxSession, SharingContext sharingContext)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			Util.ThrowOnNullArgument(sharingContext, "sharingContext");
			Util.ThrowOnNullArgument(sharingContext.FolderId, "sharingContext.FolderId");
			if (FolderPermissionContext.activeContexts.ContainsKey(sharingContext))
			{
				throw new InvalidOperationException("FolderPermissionContext has been created for this SharingContext.");
			}
			this.disposeTracker = this.GetDisposeTracker();
			this.mailboxSession = mailboxSession;
			this.sharingContext = sharingContext;
			this.folderId = sharingContext.FolderId;
			this.permissionsToAddOrChange = new Dictionary<PermissionSecurityPrincipal, MemberRights>();
			this.permissionsToRemove = new HashSet<PermissionSecurityPrincipal>();
			this.refCount = 0;
			FolderPermissionContext.activeContexts.Add(sharingContext, this);
		}

		internal static FolderPermissionContext GetCurrent(MailboxSession mailboxSession, SharingContext sharingContext)
		{
			FolderPermissionContext folderPermissionContext;
			if (!FolderPermissionContext.activeContexts.ContainsKey(sharingContext))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingContext>(0L, "{0}: Create new FolderPermissionContext for SharingContext {1}.", mailboxSession.MailboxOwner, sharingContext);
				folderPermissionContext = new FolderPermissionContext(mailboxSession, sharingContext);
			}
			else
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingContext>(0L, "{0}: Get existing FolderPermissionContext for SharingContext {1}.", mailboxSession.MailboxOwner, sharingContext);
				folderPermissionContext = FolderPermissionContext.activeContexts[sharingContext];
			}
			folderPermissionContext.refCount++;
			folderPermissionContext.enabled = true;
			return folderPermissionContext;
		}

		internal void Disable()
		{
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingContext>((long)this.GetHashCode(), "{0}: Disable the FolderPermissionContext for SharingContext {1}.", this.mailboxSession.MailboxOwner, this.sharingContext);
			this.enabled = false;
		}

		internal void AddOrChangePermission(PermissionSecurityPrincipal principal, PermissionLevel permissionLevel, FreeBusyAccess? freeBusyAccess)
		{
			this.CheckDisposed();
			Util.ThrowOnNullArgument(principal, "principal");
			Permission permission;
			if (freeBusyAccess == null)
			{
				permission = new Permission(principal, MemberRights.None);
			}
			else
			{
				permission = new CalendarFolderPermission(principal, MemberRights.None)
				{
					FreeBusyAccess = freeBusyAccess.Value
				};
			}
			permission.PermissionLevel = permissionLevel;
			if (freeBusyAccess != null)
			{
				permission.MemberRights |= MemberRights.Visible;
			}
			if (this.permissionsToRemove.Remove(principal))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, PermissionSecurityPrincipal>((long)this.GetHashCode(), "{0}: Removed {1} from permissionsToRemove list.", this.mailboxSession.MailboxOwner, principal);
			}
			if (!this.permissionsToAddOrChange.ContainsKey(principal))
			{
				this.permissionsToAddOrChange.Add(principal, permission.MemberRights);
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, PermissionSecurityPrincipal, MemberRights>((long)this.GetHashCode(), "{0}: Added {1} to permissionsToAddOrChange list: MemberRights = {2}.", this.mailboxSession.MailboxOwner, principal, permission.MemberRights);
				return;
			}
			this.permissionsToAddOrChange[principal] = permission.MemberRights;
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, PermissionSecurityPrincipal, MemberRights>((long)this.GetHashCode(), "{0}: Changed {1} in permissionsToAddOrChange list: MemberRights = {2}.", this.mailboxSession.MailboxOwner, principal, permission.MemberRights);
		}

		internal void RemovePermission(PermissionSecurityPrincipal principal)
		{
			this.CheckDisposed();
			Util.ThrowOnNullArgument(principal, "principal");
			if (this.permissionsToAddOrChange.Remove(principal))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, PermissionSecurityPrincipal>((long)this.GetHashCode(), "{0}: Removed {1} from permissionsToAddOrChange list.", this.mailboxSession.MailboxOwner, principal);
			}
			if (this.permissionsToRemove.Add(principal))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, PermissionSecurityPrincipal>((long)this.GetHashCode(), "{0}: Added {1} to permissionsToRemove list.", this.mailboxSession.MailboxOwner, principal);
			}
		}

		private void ApplyBatchPermissions()
		{
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: Entering FolderPermissionContext::ApplyBatchPermissions()", this.mailboxSession.MailboxOwner);
			if (this.permissionsToAddOrChange.Count == 0 && this.permissionsToRemove.Count == 0)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: Exiting FolderPermissionContext::ApplyBatchPermissions() due to no changes requested", this.mailboxSession.MailboxOwner);
				return;
			}
			using (Folder folder = Folder.Bind(this.mailboxSession, this.folderId))
			{
				for (int i = 2; i >= 0; i--)
				{
					PermissionSet permissionSet = folder.GetPermissionSet();
					foreach (KeyValuePair<PermissionSecurityPrincipal, MemberRights> keyValuePair in this.permissionsToAddOrChange)
					{
						PermissionSecurityPrincipal key = keyValuePair.Key;
						MemberRights value = keyValuePair.Value;
						Permission permission = permissionSet.GetEntry(key);
						if (permission == null)
						{
							ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, PermissionSecurityPrincipal, MemberRights>((long)this.GetHashCode(), "{0}: {1} is NOT found in permission table. Adding a new entry: MemberRights = {2}.", this.mailboxSession.MailboxOwner, key, value);
							permission = permissionSet.AddEntry(key, value);
						}
						else
						{
							ExTraceGlobals.SharingTracer.TraceDebug((long)this.GetHashCode(), "{0}: {1} is found in permission table: MemberRights = {2}. Modifying the entry: MemberRights = {3}.", new object[]
							{
								this.mailboxSession.MailboxOwner,
								key,
								permission.MemberRights,
								value
							});
							permission.MemberRights = value;
						}
					}
					foreach (PermissionSecurityPrincipal permissionSecurityPrincipal in this.permissionsToRemove)
					{
						if (permissionSet.GetEntry(permissionSecurityPrincipal) != null)
						{
							ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, PermissionSecurityPrincipal>((long)this.GetHashCode(), "{0}: {1} is found in permission table, Removing the entry.", this.mailboxSession.MailboxOwner, permissionSecurityPrincipal);
							permissionSet.RemoveEntry(permissionSecurityPrincipal);
						}
						else
						{
							ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, PermissionSecurityPrincipal>((long)this.GetHashCode(), "{0}: {1} is NOT found in permission table, ignored.", this.mailboxSession.MailboxOwner, permissionSecurityPrincipal);
						}
					}
					ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: Applying permission changes on the folder.", this.mailboxSession.MailboxOwner);
					try
					{
						folder.Save();
						break;
					}
					catch (RightsNotAllowedByPolicyException ex)
					{
						string[] invalidRecipients = Array.ConvertAll<RightsNotAllowedRecipient, string>(ex.RightsNotAllowedRecipients, delegate(RightsNotAllowedRecipient recipient)
						{
							ExTraceGlobals.StorageTracer.TraceError<IExchangePrincipal, PermissionSecurityPrincipal, MemberRights>((long)this.GetHashCode(), "{0}: Policy does not allow granting permission {2} to user {1}.", this.mailboxSession.MailboxOwner, recipient.Principal, recipient.NotAllowedRights);
							return recipient.Principal.ToString();
						});
						throw new InvalidSharingRecipientsException(invalidRecipients, ex);
					}
					catch (StoragePermanentException)
					{
						if (i == 0)
						{
							throw;
						}
						folder.Load();
					}
				}
			}
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: Exiting FolderPermissionContext::ApplyBatchPermissions()", this.mailboxSession.MailboxOwner);
		}

		private void CheckDisposed()
		{
			if (this.refCount < 1)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (this.refCount > 0 && --this.refCount == 0)
			{
				this.InternalDispose(disposing);
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				FolderPermissionContext.activeContexts.Remove(this.sharingContext);
				if (this.enabled)
				{
					this.ApplyBatchPermissions();
					this.sharingContext.CountOfApplied++;
				}
				else
				{
					ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: Skipped FolderPermissionContext::ApplyBatchPermissions() since this context is disabled.", this.mailboxSession.MailboxOwner);
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FolderPermissionContext>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private const int MaxRetryTime = 3;

		private readonly DisposeTracker disposeTracker;

		private readonly MailboxSession mailboxSession;

		private readonly SharingContext sharingContext;

		private readonly StoreObjectId folderId;

		private static Dictionary<SharingContext, FolderPermissionContext> activeContexts = new Dictionary<SharingContext, FolderPermissionContext>();

		private Dictionary<PermissionSecurityPrincipal, MemberRights> permissionsToAddOrChange;

		private HashSet<PermissionSecurityPrincipal> permissionsToRemove;

		private int refCount;

		private bool enabled;
	}
}
