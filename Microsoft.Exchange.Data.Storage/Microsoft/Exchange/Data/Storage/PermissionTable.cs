using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PermissionTable : IEnumerable<Permission>, IEnumerable
	{
		internal static PermissionTable Load(Func<PermissionTable, PermissionSet> permissionSetFactory, CoreFolder coreFolder)
		{
			PermissionTable permissionTable = new PermissionTable(permissionSetFactory);
			permissionTable.LoadFrom(coreFolder);
			object obj = coreFolder.Session.Mailbox.TryGetProperty(MailboxSchema.MailboxType);
			if (obj is int && StoreSession.IsPublicFolderMailbox((int)obj))
			{
				permissionTable.isPublicFolder = true;
			}
			PermissionTable.isCrossPremiseDelegateAllowedForMailboxOwner = (coreFolder.Session.MailboxOwner != null && DelegateUserCollection.IsCrossPremiseDelegateEnabled(coreFolder.Session.MailboxOwner));
			return permissionTable;
		}

		internal static PermissionTable Create(Func<PermissionTable, PermissionSet> permissionSetFactory)
		{
			PermissionTable permissionTable = new PermissionTable(permissionSetFactory);
			permissionTable.defaultMemberPermission = permissionTable.PermissionSet.CreatePermission(new PermissionSecurityPrincipal(PermissionSecurityPrincipal.SpecialPrincipalType.Default), MemberRights.None, 0L);
			permissionTable.anonymousMemberPermission = permissionTable.PermissionSet.CreatePermission(new PermissionSecurityPrincipal(PermissionSecurityPrincipal.SpecialPrincipalType.Anonymous), MemberRights.None, -1L);
			return permissionTable;
		}

		private PermissionTable(Func<PermissionTable, PermissionSet> permissionSetFactory)
		{
			Util.ThrowOnNullArgument(permissionSetFactory, "permissionSetFactory");
			this.permissionSet = permissionSetFactory(this);
		}

		internal Permission AddEntry(PermissionSecurityPrincipal securityPrincipal, MemberRights memberRights)
		{
			this.CheckValid();
			EnumValidator.ThrowIfInvalid<MemberRights>(memberRights, "memberRights");
			if (securityPrincipal == null)
			{
				throw new ArgumentNullException("securityPrincipal");
			}
			if (string.IsNullOrEmpty(securityPrincipal.IndexString))
			{
				throw new ArgumentException("SecurityPrincipal must at least have a valid index string.", "securityPrincipal");
			}
			if (securityPrincipal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal && !PermissionTable.isCrossPremiseDelegateAllowedForMailboxOwner)
			{
				ADRecipient adrecipient = securityPrincipal.ADRecipient;
				if (!ADRecipient.IsValidRecipient(adrecipient, !this.isPublicFolder))
				{
					throw new ArgumentException("Cannot use " + adrecipient.DisplayName + " as security principal", "securityPrincipal");
				}
			}
			Permission permission = null;
			if (!this.permissions.ContainsKey(securityPrincipal.IndexString))
			{
				if (this.removedPermissions.TryGetValue(securityPrincipal.IndexString, out permission))
				{
					this.removedPermissions.Remove(securityPrincipal.IndexString);
					permission.MemberRights = memberRights;
				}
				else
				{
					permission = this.PermissionSet.CreatePermission(securityPrincipal, memberRights);
				}
			}
			this.AddPermissionEntry(securityPrincipal, permission);
			return permission;
		}

		internal void RemoveEntry(PermissionSecurityPrincipal securityPrincipal)
		{
			this.CheckValid();
			Util.ThrowOnNullArgument(securityPrincipal, "securityPrincipal");
			Permission permission;
			if (this.permissions.TryGetValue(securityPrincipal.IndexString, out permission))
			{
				if (permission.Origin == PermissionOrigin.Read)
				{
					this.removedPermissions.Add(securityPrincipal.IndexString, permission);
				}
				this.permissions.Remove(securityPrincipal.IndexString);
			}
		}

		internal void Clear()
		{
			this.CheckValid();
			if (this.DefaultPermission != null)
			{
				this.DefaultPermission.MemberRights = MemberRights.None;
			}
			if (this.AnonymousPermission != null)
			{
				this.AnonymousPermission.MemberRights = MemberRights.None;
			}
			List<PermissionSecurityPrincipal> list = new List<PermissionSecurityPrincipal>();
			foreach (Permission permission in this)
			{
				list.Add(permission.Principal);
			}
			foreach (PermissionSecurityPrincipal securityPrincipal in list)
			{
				this.RemoveEntry(securityPrincipal);
			}
		}

		internal Permission GetEntry(PermissionSecurityPrincipal securityPrincipal)
		{
			this.CheckValid();
			Util.ThrowOnNullArgument(securityPrincipal, "securityPrincipal");
			Permission result = null;
			if (this.permissions.TryGetValue(securityPrincipal.IndexString, out result))
			{
				return result;
			}
			return null;
		}

		internal Permission DefaultPermission
		{
			get
			{
				this.CheckValid();
				return this.defaultMemberPermission;
			}
			set
			{
				this.CheckValid();
				this.defaultMemberPermission = value;
			}
		}

		internal Permission AnonymousPermission
		{
			get
			{
				this.CheckValid();
				return this.anonymousMemberPermission;
			}
			set
			{
				this.CheckValid();
				this.anonymousMemberPermission = value;
			}
		}

		internal void Save(CoreFolder coreFolder)
		{
			this.CheckValid();
			Util.ThrowOnNullArgument(coreFolder, "coreFolder");
			if (this.IsDirty)
			{
				this.EnforceRestriction(coreFolder);
				this.SaveSharingPartnership(coreFolder.Session as MailboxSession);
				using (IModifyTable permissionTable = coreFolder.GetPermissionTable(this.PermissionSet.ModifyTableOptions))
				{
					MapiAclTableAdapter mapiAclTableAdapter = new MapiAclTableAdapter(permissionTable);
					this.AddPermissionEntriesForRemove(mapiAclTableAdapter);
					this.AddPermissionEntriesForAddOrModify(mapiAclTableAdapter);
					mapiAclTableAdapter.ApplyPendingChanges(true);
				}
			}
			this.isInvalid = true;
		}

		internal bool IsDirty
		{
			get
			{
				this.CheckValid();
				if (this.removedPermissions.Count > 0)
				{
					return true;
				}
				if (this.defaultMemberPermission != null && this.defaultMemberPermission.IsDirty)
				{
					return true;
				}
				if (this.anonymousMemberPermission != null && this.anonymousMemberPermission.IsDirty)
				{
					return true;
				}
				foreach (Permission permission in this.permissions.Values)
				{
					if (permission.IsDirty)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal PermissionSet PermissionSet
		{
			get
			{
				this.CheckValid();
				return this.permissionSet;
			}
		}

		public IEnumerator<Permission> GetEnumerator()
		{
			this.CheckValid();
			return this.permissions.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			this.CheckValid();
			return this.GetEnumerator();
		}

		private void CheckValid()
		{
			if (this.isInvalid)
			{
				throw new InvalidOperationException("PermissionTable should not be used after a call to Save()");
			}
		}

		private void LoadFrom(CoreFolder coreFolder)
		{
			using (IModifyTable permissionTable = coreFolder.GetPermissionTable(this.PermissionSet.ModifyTableOptions))
			{
				try
				{
					this.LoadFrom(new MapiAclTableAdapter(permissionTable));
				}
				catch (DataSourceOperationException ex)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "PermissionsTable.LoadFrom. Failed due to directory exception {0}.", new object[]
					{
						ex
					});
				}
				catch (DataSourceTransientException ex2)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "PermissionsTable.LoadFrom. Failed due to directory exception {0}.", new object[]
					{
						ex2
					});
				}
			}
		}

		private void LoadFrom(MapiAclTableAdapter mapiAclTableAdapter)
		{
			IRecipientSession recipientSession = null;
			ExternalUserCollection disposable = null;
			AclTableEntry[] all = mapiAclTableAdapter.GetAll();
			try
			{
				foreach (AclTableEntry aclTableEntry in all)
				{
					long memberId = aclTableEntry.MemberId;
					byte[] memberEntryId = aclTableEntry.MemberEntryId;
					string memberName = aclTableEntry.MemberName;
					MemberRights memberRights = aclTableEntry.MemberRights;
					if (memberId == 0L)
					{
						this.defaultMemberPermission = this.permissionSet.CreatePermission(new PermissionSecurityPrincipal(PermissionSecurityPrincipal.SpecialPrincipalType.Default), memberRights, memberId);
					}
					else if (memberId == -1L)
					{
						this.anonymousMemberPermission = this.permissionSet.CreatePermission(new PermissionSecurityPrincipal(PermissionSecurityPrincipal.SpecialPrincipalType.Anonymous), memberRights, memberId);
					}
					else if (memberEntryId != null)
					{
						ADParticipantEntryId adparticipantEntryId = mapiAclTableAdapter.TryGetParticipantEntryId(memberEntryId);
						if (adparticipantEntryId != null)
						{
							if (recipientSession == null)
							{
								recipientSession = mapiAclTableAdapter.Session.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid);
							}
							ADRecipient adrecipient = null;
							try
							{
								adrecipient = recipientSession.FindByLegacyExchangeDN(adparticipantEntryId.LegacyDN);
							}
							catch (DataValidationException)
							{
								ExTraceGlobals.StorageTracer.TraceDebug<string, string>(0L, "PermissionTable::PermissionTable. Caught exception from ADSesssion.FindByLegacyExchangeDN when trying to find a recipient from the ACL Table. Recipient name = {0}, LegDN = {1}.", memberName, adparticipantEntryId.LegacyDN);
								this.AddUnknownEntry(memberName, memberId, memberEntryId, memberRights);
								goto IL_17E;
							}
							if (adrecipient != null)
							{
								Permission permission = this.permissionSet.CreatePermission(new PermissionSecurityPrincipal(adrecipient), memberRights, memberId);
								this.AddPermissionEntry(permission.Principal, permission);
							}
							else
							{
								ExTraceGlobals.StorageTracer.TraceDebug<string, string>(0L, "PermissionTable::PermissionTable. Did not find the recipient from the ACL table in the AD. Recipient name = {0}, LegDN = {1}.", memberName, adparticipantEntryId.LegacyDN);
								this.AddUnknownEntry(memberName, memberId, memberEntryId, memberRights);
							}
						}
						else
						{
							this.AddNonADEntry(mapiAclTableAdapter, ref disposable, memberName, memberId, memberEntryId, memberRights);
						}
					}
					else
					{
						ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PermissionTable::PermissionTable. Found a member in the ACL table (other than anonymous and default) without a member entry id. Recipient Name = {0}.", memberName);
						this.AddUnknownEntry(memberName, memberId, memberEntryId, memberRights);
					}
					IL_17E:;
				}
			}
			finally
			{
				Util.DisposeIfPresent(disposable);
			}
		}

		private void AddNonADEntry(MapiAclTableAdapter mapiAclTableAdapter, ref ExternalUserCollection externalUsers, string memberName, long memberId, byte[] memberEntryId, MemberRights rights)
		{
			ExternalUser externalUser = mapiAclTableAdapter.TryGetExternalUser(memberEntryId, ref externalUsers);
			if (externalUser != null)
			{
				PermissionSecurityPrincipal securityPrincipal = new PermissionSecurityPrincipal(externalUser);
				Permission permission = this.permissionSet.CreatePermission(securityPrincipal, rights, memberId);
				this.AddPermissionEntry(securityPrincipal, permission);
				return;
			}
			ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "PermissionTable::PermissionTable. Member has invalid entry id, member name = {0}.", memberName);
			this.AddUnknownEntry(memberName, memberId, memberEntryId, rights);
		}

		private void AddUnknownEntry(string memberName, long memberId, byte[] memberEntryId, MemberRights rights)
		{
			PermissionSecurityPrincipal securityPrincipal = new PermissionSecurityPrincipal(memberName, memberEntryId, memberId);
			Permission permission = this.permissionSet.CreatePermission(securityPrincipal, rights, memberId);
			this.AddPermissionEntry(securityPrincipal, permission);
		}

		private void AddPermissionEntriesForRemove(MapiAclTableAdapter mapiAclTableAdapter)
		{
			foreach (Permission permission in this.removedPermissions.Values)
			{
				mapiAclTableAdapter.RemovePermissionEntry(permission.MemberId);
			}
		}

		private void AddPermissionEntriesForAddOrModify(MapiAclTableAdapter mapiAclTableAdapter)
		{
			foreach (Permission permission in this.permissions.Values)
			{
				byte[] array = null;
				if (permission.Origin == PermissionOrigin.New)
				{
					if (permission.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ADRecipientPrincipal)
					{
						ParticipantEntryId participantEntryId = ParticipantEntryId.FromParticipant(new Participant(permission.Principal.ADRecipient), ParticipantEntryIdConsumer.SupportsADParticipantEntryId);
						array = participantEntryId.ToByteArray();
					}
					else if (permission.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal)
					{
						ExternalUser externalUser = permission.Principal.ExternalUser;
						byte[] array2 = new byte[externalUser.Sid.BinaryLength];
						externalUser.Sid.GetBinaryForm(array2, 0);
						array = MapiStore.GetAddressBookEntryIdFromLocalDirectorySID(array2);
					}
					if (array != null)
					{
						mapiAclTableAdapter.AddPermissionEntry(array, permission.MemberRights);
					}
				}
				else if (permission.IsDirty)
				{
					mapiAclTableAdapter.ModifyPermissionEntry(permission.MemberId, permission.MemberRights);
				}
			}
			if (this.anonymousMemberPermission != null && this.anonymousMemberPermission.IsDirty)
			{
				mapiAclTableAdapter.ModifyPermissionEntry(this.anonymousMemberPermission.MemberId, this.anonymousMemberPermission.MemberRights);
			}
			if (this.defaultMemberPermission != null && this.defaultMemberPermission.IsDirty)
			{
				mapiAclTableAdapter.ModifyPermissionEntry(this.defaultMemberPermission.MemberId, this.defaultMemberPermission.MemberRights);
			}
		}

		private void EnforceRestriction(CoreFolder coreFolder)
		{
			new MapiAclTableRestriction(coreFolder).Enforce(new Func<ICollection<MapiAclTableRestriction.ExternalUserPermission>>(this.GetExternalUserPermissions));
		}

		private ICollection<MapiAclTableRestriction.ExternalUserPermission> GetExternalUserPermissions()
		{
			List<MapiAclTableRestriction.ExternalUserPermission> list = null;
			foreach (Permission permission in this.permissions.Values)
			{
				if (permission.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal)
				{
					if (permission.Origin == PermissionOrigin.New || permission.IsDirty)
					{
						if (list == null)
						{
							list = new List<MapiAclTableRestriction.ExternalUserPermission>(this.permissions.Count);
						}
						list.Add(new MapiAclTableRestriction.ExternalUserPermission(permission.Principal, permission.MemberRights));
					}
					else
					{
						ExTraceGlobals.StorageTracer.TraceDebug<PermissionSecurityPrincipal, MemberRights>((long)this.GetHashCode(), "PermissionTable::GetExternalUserPermissions. Skipped policy checking on unchanged external permission: Principal={0}, MemberRights={1}.", permission.Principal, permission.MemberRights);
					}
				}
			}
			return list;
		}

		private void SaveSharingPartnership(MailboxSession mailboxSession)
		{
			if (mailboxSession == null)
			{
				return;
			}
			IRecipientSession recipientSession = null;
			ADUser aduser = null;
			foreach (Permission permission in this.permissions.Values)
			{
				if (permission.Principal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal && !permission.Principal.ExternalUser.IsReachUser)
				{
					if (aduser == null)
					{
						recipientSession = mailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent);
						ADRecipient adrecipient = recipientSession.Read(mailboxSession.MailboxOwner.ObjectId);
						if (adrecipient == null)
						{
							throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
						}
						aduser = (adrecipient as ADUser);
						if (aduser == null)
						{
							ExTraceGlobals.StorageTracer.TraceDebug<ADRecipient>((long)this.GetHashCode(), "PermissionTable::SaveSharingParterner. This is not an ADUser so SharingPartnerIdentities doesn't apply. Recipient = {0}.", adrecipient);
							return;
						}
					}
					string externalId = permission.Principal.ExternalUser.ExternalId;
					if (!aduser.SharingPartnerIdentities.Contains(externalId))
					{
						try
						{
							aduser.SharingPartnerIdentities.Add(externalId);
						}
						catch (InvalidOperationException ex)
						{
							ExTraceGlobals.StorageTracer.TraceError<ADUser, InvalidOperationException>((long)this.GetHashCode(), "PermissionTable::SaveSharingParterner. Failed to add SharingPartnerIdentities on user {0} due to exception {1}.", aduser, ex);
							throw new InvalidObjectOperationException(new LocalizedString(ex.Message), ex);
						}
					}
				}
			}
			if (aduser != null && aduser.SharingPartnerIdentities.Changed)
			{
				try
				{
					recipientSession.Save(aduser);
				}
				catch (DataValidationException innerException)
				{
					throw new CorruptDataException(ServerStrings.ExCannotSaveInvalidObject(aduser), innerException);
				}
				catch (DataSourceOperationException ex2)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "PermissionTable::SaveSharingParterner(): Failed due to directory exception {0}.", new object[]
					{
						ex2
					});
				}
				catch (DataSourceTransientException ex3)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex3, null, "PermissionTable::SaveSharingParterner(): Failed due to directory exception {0}.", new object[]
					{
						ex3
					});
				}
			}
		}

		private void AddPermissionEntry(PermissionSecurityPrincipal securityPrincipal, Permission permission)
		{
			if (this.permissions.ContainsKey(securityPrincipal.IndexString))
			{
				throw new CorruptDataException(ServerStrings.SecurityPrincipalAlreadyDefined);
			}
			this.permissions.Add(securityPrincipal.IndexString, permission);
		}

		internal const long AnonymousMemberId = -1L;

		internal const long DefaultMemberId = 0L;

		private bool isPublicFolder;

		private static bool isCrossPremiseDelegateAllowedForMailboxOwner;

		private readonly Dictionary<string, Permission> permissions = new Dictionary<string, Permission>();

		private readonly Dictionary<string, Permission> removedPermissions = new Dictionary<string, Permission>();

		private readonly PermissionSet permissionSet;

		private Permission defaultMemberPermission;

		private Permission anonymousMemberPermission;

		private bool isInvalid;

		private static readonly PropertyDefinition[] PropertiesToRead = new PropertyDefinition[]
		{
			InternalSchema.MemberId,
			InternalSchema.MemberEntryId,
			InternalSchema.MemberName,
			InternalSchema.MemberRights
		};
	}
}
