using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.GroupMailbox;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class GroupMailboxPermissionHandler
	{
		public static bool IsFolderToBeIgnored(DefaultFolderType defaultFolderType)
		{
			return defaultFolderType == DefaultFolderType.None || defaultFolderType == DefaultFolderType.LegacySpoolerQueue || defaultFolderType == DefaultFolderType.Audits || defaultFolderType == DefaultFolderType.System || defaultFolderType == DefaultFolderType.AdminAuditLogs || defaultFolderType == DefaultFolderType.RssSubscription || defaultFolderType == DefaultFolderType.Conflicts || defaultFolderType == DefaultFolderType.SyncIssues || defaultFolderType == DefaultFolderType.LocalFailures || defaultFolderType == DefaultFolderType.ServerFailures;
		}

		public static bool ModifyPermission(PermissionSet permissionSet, PermissionSecurityPrincipal permissionSecurityPrincipal, MemberRights memberRights)
		{
			bool result = false;
			if (permissionSecurityPrincipal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.SpecialPrincipal && permissionSecurityPrincipal.SpecialType == PermissionSecurityPrincipal.SpecialPrincipalType.Default)
			{
				GroupMailboxPermissionHandler.Tracer.TraceDebug<PermissionSecurityPrincipal.SpecialPrincipalType, MemberRights>(0L, "Modifying permissions for permissionSecurityPrincipal {0} with rights {1}", PermissionSecurityPrincipal.SpecialPrincipalType.Default, memberRights);
				Permission permission = permissionSet.DefaultPermission;
				if (permission != null && permission.MemberRights != memberRights)
				{
					permissionSet.SetDefaultPermission(memberRights);
					result = true;
				}
			}
			else
			{
				GroupMailboxPermissionHandler.Tracer.TraceDebug<string, MemberRights>(0L, "Modifying permissions for permissionSecurityPrincipal {0} with rights {1}", (permissionSecurityPrincipal.Type == PermissionSecurityPrincipal.SecurityPrincipalType.ExternalUserPrincipal) ? permissionSecurityPrincipal.ExternalUser.ExternalId : string.Empty, memberRights);
				Permission permission = permissionSet.GetEntry(permissionSecurityPrincipal);
				if (permission == null && memberRights != MemberRights.None)
				{
					permission = permissionSet.AddEntry(permissionSecurityPrincipal, PermissionLevel.None);
					permission.MemberRights = memberRights;
					result = true;
				}
				else if (permission != null && memberRights == MemberRights.None)
				{
					permissionSet.RemoveEntry(permissionSecurityPrincipal);
					result = true;
				}
				else if (permission != null && permission.MemberRights != memberRights)
				{
					permission.MemberRights = memberRights;
					result = true;
				}
			}
			return result;
		}

		public static bool AssignMemberRight(MailboxSession mailboxSession, List<PermissionEntry> permissionEntries, DefaultFolderType defaultFolderType, out int foldersModified)
		{
			foldersModified = 0;
			try
			{
				using (Folder folder = Folder.Bind(mailboxSession, defaultFolderType))
				{
					bool flag = false;
					PermissionSet permissionSet = folder.GetPermissionSet();
					foreach (PermissionEntry permissionEntry in permissionEntries)
					{
						if (permissionEntry.UserSecurityPrincipal != null && GroupMailboxPermissionHandler.ModifyPermission(permissionSet, permissionEntry.UserSecurityPrincipal, permissionEntry.UserRights))
						{
							flag = true;
							foldersModified++;
						}
					}
					if (flag)
					{
						folder.Save();
					}
					return true;
				}
			}
			catch (CorruptDataException arg)
			{
				GroupMailboxPermissionHandler.Tracer.TraceDebug<DefaultFolderType, CorruptDataException>(1L, "Member rights already configured for folder {0}. Exception: {1}", defaultFolderType, arg);
			}
			catch (ObjectNotFoundException ex)
			{
				GroupMailboxPermissionHandler.Tracer.TraceError(1L, (ex.InnerException != null) ? ex.InnerException.ToString() : ex.ToString());
			}
			return false;
		}

		public static readonly MemberRights DefaultFolderPermission = Permission.GetMemberRights(PermissionLevel.Author);

		public static readonly MemberRights OwnerSpecificPermission = MemberRights.DeleteAny;

		public static readonly MemberRights CalendarFolderPermissionForDefaultUsers = Permission.GetMemberRights(PermissionLevel.Reviewer);

		public static readonly MemberRights CalendarFolderPermission = Permission.GetMemberRights(PermissionLevel.Editor) | MemberRights.FreeBusySimple | MemberRights.FreeBusyDetailed;

		public static readonly MemberRights SearchFolderPermission = Permission.GetMemberRights(PermissionLevel.PublishingAuthor);

		public static readonly MemberRights MailboxAssociationPermission = MemberRights.None;

		public static readonly MemberRights ConfigurationFolderPermission = GroupMailboxPermissionHandler.DefaultFolderPermission | GroupMailboxPermissionHandler.OwnerSpecificPermission | MemberRights.EditAny;

		public static readonly ActiveDirectoryRights MailboxRightsFullAccess = ActiveDirectoryRights.CreateChild;

		internal static readonly int GroupMailboxPermissionVersion = 2;

		internal static readonly ActiveDirectoryAccessRule MailboxRightsFullAccessRule = new ActiveDirectoryAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), GroupMailboxPermissionHandler.MailboxRightsFullAccess, AccessControlType.Allow, Guid.Empty, ActiveDirectorySecurityInheritance.All, Guid.Empty);

		private static readonly Trace Tracer = ExTraceGlobals.GroupMailboxAccessLayerTracer;
	}
}
