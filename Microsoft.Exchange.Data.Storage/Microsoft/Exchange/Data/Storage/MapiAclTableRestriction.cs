using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MapiAclTableRestriction : IModifyTableRestriction
	{
		internal MapiAclTableRestriction(CoreFolder coreFolder)
		{
			Util.ThrowOnNullArgument(coreFolder, "coreFolder");
			this.coreFolder = coreFolder;
			this.session = coreFolder.Session;
		}

		void IModifyTableRestriction.Enforce(IModifyTable modifyTable, IEnumerable<ModifyTableOperation> changingEntries)
		{
			if (changingEntries == null)
			{
				return;
			}
			this.Enforce(() => this.GetExternalUserPermissions(new MapiAclTableAdapter(modifyTable), from entry in changingEntries
			select AclTableEntry.ModifyOperation.FromModifyTableOperation(entry)));
		}

		internal void Enforce(Func<ICollection<MapiAclTableRestriction.ExternalUserPermission>> getExternalUserPermissions)
		{
			if ((this.session == null || !this.session.IsMoveUser) && this.coreFolder.IsPermissionChangeBlocked())
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: Cannot change permissions on permission-change-blocked folder.", this.session.DisplayAddress);
				throw new CannotChangePermissionsOnFolderException();
			}
			MailboxSession mailboxSession = this.session as MailboxSession;
			if (mailboxSession == null || mailboxSession.IsGroupMailbox())
			{
				return;
			}
			this.EnforceSharingPolicy(mailboxSession, getExternalUserPermissions());
		}

		private void EnforceSharingPolicy(MailboxSession mailboxSession, ICollection<MapiAclTableRestriction.ExternalUserPermission> externalUserPermissions)
		{
			Util.ThrowOnNullArgument(mailboxSession, "mailboxSession");
			if (externalUserPermissions == null || externalUserPermissions.Count == 0)
			{
				return;
			}
			List<RightsNotAllowedRecipient> list = new List<RightsNotAllowedRecipient>(externalUserPermissions.Count);
			SharingPolicy sharingPolicy = null;
			foreach (MapiAclTableRestriction.ExternalUserPermission externalUserPermission in externalUserPermissions)
			{
				if (sharingPolicy == null)
				{
					IMailboxInfo mailboxInfo = mailboxSession.MailboxOwner.MailboxInfo;
					sharingPolicy = DirectoryHelper.ReadSharingPolicy(mailboxInfo.MailboxGuid, mailboxInfo.IsArchive, mailboxSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid));
					if (sharingPolicy == null)
					{
						ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: No policy assigned means no external sharing is allowed for this user.", mailboxSession.MailboxOwner);
						throw new NotAllowedExternalSharingByPolicyException();
					}
				}
				if (!sharingPolicy.Enabled)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: A disabled policy means no external sharing is allowed for this user.", mailboxSession.MailboxOwner);
					throw new NotAllowedExternalSharingByPolicyException();
				}
				SharingPolicyAction sharingPolicyAction = externalUserPermission.Principal.ExternalUser.IsReachUser ? sharingPolicy.GetAllowedForAnonymousCalendarSharing() : sharingPolicy.GetAllowed(externalUserPermission.Principal.ExternalUser.SmtpAddress.Domain);
				if (sharingPolicyAction == (SharingPolicyAction)0)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal, PermissionSecurityPrincipal>((long)this.GetHashCode(), "{0}: Policy does not allow granting permissions to {1}.", mailboxSession.MailboxOwner, externalUserPermission.Principal);
					throw new PrincipalNotAllowedByPolicyException(externalUserPermission.Principal);
				}
				MemberRights allowed = PolicyAllowedMemberRights.GetAllowed(sharingPolicyAction, this.FolderInfo.StoreObjectType);
				MemberRights memberRights = ~allowed & externalUserPermission.MemberRights;
				if (memberRights != MemberRights.None)
				{
					ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "{0}: Policy does not allow granting permission {1} to {2} on {3} folder '{4}'.", new object[]
					{
						mailboxSession.MailboxOwner,
						memberRights,
						externalUserPermission.Principal,
						this.FolderInfo.StoreObjectType,
						this.FolderInfo.DisplayName
					});
					list.Add(new RightsNotAllowedRecipient(externalUserPermission.Principal, memberRights));
				}
			}
			if (list.Count > 0)
			{
				throw new RightsNotAllowedByPolicyException(list.ToArray(), this.FolderInfo.StoreObjectType, this.FolderInfo.DisplayName);
			}
		}

		private ICollection<MapiAclTableRestriction.ExternalUserPermission> GetExternalUserPermissions(MapiAclTableAdapter mapiAclTableAdapter, IEnumerable<AclTableEntry.ModifyOperation> changingEntries)
		{
			List<MapiAclTableRestriction.ExternalUserPermission> list = null;
			ExternalUserCollection disposable = null;
			try
			{
				foreach (AclTableEntry.ModifyOperation modifyOperation in changingEntries)
				{
					if (modifyOperation.Operation == ModifyTableOperationType.Add || modifyOperation.Operation == ModifyTableOperationType.Modify)
					{
						MapiAclTableRestriction.ExternalUserPermission externalUserPermission = this.TryGetExternalUserPermission(modifyOperation.Entry, mapiAclTableAdapter, ref disposable);
						if (externalUserPermission != null)
						{
							if (list == null)
							{
								list = new List<MapiAclTableRestriction.ExternalUserPermission>();
							}
							list.Add(externalUserPermission);
						}
					}
				}
			}
			finally
			{
				Util.DisposeIfPresent(disposable);
			}
			return list;
		}

		private MapiAclTableRestriction.ExternalUserPermission TryGetExternalUserPermission(AclTableEntry aclTableEntry, MapiAclTableAdapter mapiAclTableAdapter, ref ExternalUserCollection externalUsers)
		{
			Util.ThrowOnNullArgument(aclTableEntry, "aclTableEntry");
			Util.ThrowOnNullArgument(mapiAclTableAdapter, "mapiAclTableAdapter");
			MailboxSession mailboxSession = this.session as MailboxSession;
			if (mailboxSession == null)
			{
				return null;
			}
			byte[] memberEntryId = aclTableEntry.MemberEntryId;
			MemberRights memberRights = aclTableEntry.MemberRights;
			long memberId = aclTableEntry.MemberId;
			if (memberEntryId == null || memberEntryId.Length == 0)
			{
				if (memberId <= 0L)
				{
					return null;
				}
				ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal, long>((long)this.GetHashCode(), "{0}: Getting memberEntryId from current ACL table for MemberId {1}.", mailboxSession.MailboxOwner, memberId);
				AclTableEntry byMemberId = mapiAclTableAdapter.GetByMemberId(memberId);
				if (byMemberId == null || byMemberId.MemberEntryId == null)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal, long>((long)this.GetHashCode(), "{0}: Not found memberEntryId from current ACL table for MemberId {1}. Skipped.", mailboxSession.MailboxOwner, memberId);
					return null;
				}
				memberEntryId = byMemberId.MemberEntryId;
			}
			if (mapiAclTableAdapter.TryGetParticipantEntryId(memberEntryId) != null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: MemberEntryId indicates internal user. Skipped.", mailboxSession.MailboxOwner);
				return null;
			}
			ExternalUser externalUser = mapiAclTableAdapter.TryGetExternalUser(memberEntryId, ref externalUsers);
			if (externalUser == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<IExchangePrincipal>((long)this.GetHashCode(), "{0}: MemberEntryId is not external user. Skipped.", mailboxSession.MailboxOwner);
				return null;
			}
			return new MapiAclTableRestriction.ExternalUserPermission(externalUser, memberRights);
		}

		private MapiAclTableRestriction.FolderInformation FolderInfo
		{
			get
			{
				if (this.folderInfo == null)
				{
					this.folderInfo = new MapiAclTableRestriction.FolderInformation(this.coreFolder);
				}
				return this.folderInfo;
			}
		}

		private readonly CoreFolder coreFolder;

		private readonly StoreSession session;

		private MapiAclTableRestriction.FolderInformation folderInfo;

		internal class ExternalUserPermission
		{
			internal ExternalUserPermission(ExternalUser externalUser, MemberRights memberRights)
			{
				this.Principal = new PermissionSecurityPrincipal(externalUser);
				this.MemberRights = memberRights;
			}

			internal ExternalUserPermission(PermissionSecurityPrincipal principal, MemberRights memberRights)
			{
				this.Principal = principal;
				this.MemberRights = memberRights;
			}

			internal readonly MemberRights MemberRights;

			internal readonly PermissionSecurityPrincipal Principal;
		}

		private class FolderInformation
		{
			internal FolderInformation(CoreFolder coreFolder)
			{
				this.propertyBag = CoreObject.GetPersistablePropertyBag(coreFolder);
				this.propertyBag.Load(MapiAclTableRestriction.FolderInformation.PropertyDefinitions);
				string valueOrDefault = this.propertyBag.GetValueOrDefault<string>(StoreObjectSchema.ContainerClass);
				this.storeObjectType = ObjectClass.GetObjectType(valueOrDefault);
			}

			internal StoreObjectType StoreObjectType
			{
				get
				{
					return this.storeObjectType;
				}
			}

			internal string DisplayName
			{
				get
				{
					if (string.IsNullOrEmpty(this.displayName))
					{
						this.displayName = this.propertyBag.GetValueOrDefault<string>(FolderSchema.DisplayName);
					}
					return this.displayName ?? string.Empty;
				}
			}

			private static readonly PropertyDefinition[] PropertyDefinitions = new PropertyDefinition[]
			{
				StoreObjectSchema.ContainerClass,
				FolderSchema.DisplayName
			};

			private readonly PersistablePropertyBag propertyBag;

			private StoreObjectType storeObjectType;

			private string displayName;
		}

		private enum RowFlags
		{
			Add = 1,
			Modify
		}
	}
}
