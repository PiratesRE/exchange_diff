using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Exchange.EdgeSync.Datacenter;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfWellKnownGroup
	{
		public EhfWellKnownGroup(string groupName)
		{
			this.wellknownGroupName = groupName;
		}

		public EhfWellKnownGroup(string groupName, Guid externalDirectoryObjectId) : this(groupName)
		{
			this.externalDirectoryObjectId = externalDirectoryObjectId;
		}

		public Dictionary<Guid, MailboxAdminSyncUser> GroupMembers
		{
			get
			{
				return this.groupMembers;
			}
		}

		public Dictionary<Guid, AdminSyncUser> SubGroups
		{
			get
			{
				return this.subGroups;
			}
		}

		public Dictionary<Guid, PartnerGroupAdminSyncUser> LinkedRoleGroups
		{
			get
			{
				return this.linkedRoleGroups;
			}
		}

		public Guid ExternalDirectoryObjectId
		{
			get
			{
				return this.externalDirectoryObjectId;
			}
		}

		public string WellKnownGroupName
		{
			get
			{
				return this.wellknownGroupName;
			}
		}

		public static bool IsViewOnlyOrganizationManagementGroup(ExSearchResultEntry change)
		{
			return change.DistinguishedName.StartsWith(EhfWellKnownGroup.VomRoleGroupPrefix, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsOrganizationManagementGroup(ExSearchResultEntry change)
		{
			return change.DistinguishedName.StartsWith(EhfWellKnownGroup.OMRoleGroupPrefix, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsWellKnownPartnerGroupDN(string dn)
		{
			return EhfWellKnownGroup.IsAdminAgentGroup(dn) || EhfWellKnownGroup.IsHelpdeskAgentGroup(dn);
		}

		public static bool IsAdminAgentGroup(string dn)
		{
			return dn.StartsWith(EhfWellKnownGroup.AdminAgentGroupDnPrefix, StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsHelpdeskAgentGroup(string dn)
		{
			return dn.StartsWith(EhfWellKnownGroup.HelpdeskAgentGroupDnPrefix, StringComparison.OrdinalIgnoreCase);
		}

		public string[] GetWlidsOfGroupMembers(int maxAdmins, EdgeSyncDiag diagSession)
		{
			List<string> list = new List<string>(this.groupMembers.Count);
			foreach (MailboxAdminSyncUser mailboxAdminSyncUser in this.groupMembers.Values)
			{
				if (!string.IsNullOrEmpty(mailboxAdminSyncUser.WindowsLiveId))
				{
					if (!EhfWellKnownGroup.ValidateWindowsLiveId(mailboxAdminSyncUser))
					{
						diagSession.LogAndTraceError("WLID <{0}> for user <{1}>:<{2}> is not valid. The admin will be ignored from group <{3}>.", new object[]
						{
							mailboxAdminSyncUser.WindowsLiveId,
							mailboxAdminSyncUser.DistinguishedName,
							mailboxAdminSyncUser.UserGuid,
							this.wellknownGroupName
						});
					}
					else
					{
						list.Add(mailboxAdminSyncUser.WindowsLiveId);
					}
					if (list.Count == maxAdmins)
					{
						diagSession.LogAndTraceError("The group <{0}> has more than the maximum allowed number ({1}) of members. Only the first <{1}> will be taken.", new object[]
						{
							this.wellknownGroupName,
							maxAdmins
						});
						break;
					}
				}
			}
			return list.ToArray();
		}

		public Guid[] GetLinkedPartnerGroupGuidsOfLinkedRoleGroups(int maxMembers, EdgeSyncDiag diagSession)
		{
			if (this.linkedRoleGroups == null)
			{
				return null;
			}
			Guid[] array = new Guid[Math.Min(this.linkedRoleGroups.Count, maxMembers)];
			int num = 0;
			foreach (PartnerGroupAdminSyncUser partnerGroupAdminSyncUser in this.linkedRoleGroups.Values)
			{
				array[num++] = partnerGroupAdminSyncUser.PartnerGroupGuid;
				if (num == maxMembers)
				{
					diagSession.LogAndTraceError("The group <{0}> has more than the maximum allowed number ({1}) of partner linked role groups. Only the first <{1}> will be taken.", new object[]
					{
						this.wellknownGroupName,
						maxMembers
					});
					break;
				}
			}
			return array;
		}

		public void ToString(StringBuilder sb)
		{
			EhfWellKnownGroup.AddAdminToList<MailboxAdminSyncUser>(this.groupMembers, sb, "Local Admins: ");
			EhfWellKnownGroup.AddAdminToList<PartnerGroupAdminSyncUser>(this.linkedRoleGroups, sb, "Partner Admins: ");
		}

		private static bool ValidateWindowsLiveId(MailboxAdminSyncUser user)
		{
			return user.WindowsLiveId.Trim().Length >= 6 && user.WindowsLiveId.Trim().Length <= 320 && EhfWellKnownGroup.EmailAddressRegex.Match(user.WindowsLiveId).Success;
		}

		private static void AddAdminToList<T>(Dictionary<Guid, T> admins, StringBuilder stringBuilder, string adminName) where T : AdminSyncUser
		{
			stringBuilder.Append("; ");
			stringBuilder.Append(adminName);
			if (admins != null)
			{
				bool flag = false;
				using (Dictionary<Guid, T>.Enumerator enumerator = admins.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<Guid, T> keyValuePair = enumerator.Current;
						if (flag)
						{
							stringBuilder.Append(",");
						}
						else
						{
							flag = true;
						}
						stringBuilder.Append(keyValuePair.Value);
					}
					return;
				}
			}
			stringBuilder.Append("<NoChange> ");
		}

		public const int MaxAdminsPerRole = 1000;

		public const int MaxGroupUsers = 2000;

		public const int MinEmailLength = 6;

		public const int MaxEmailLength = 320;

		public static readonly string AdminAgentGroupDnPrefix = string.Format("CN={0}", EhfCompanyAdmins.AdminAgentGroupNamePrefix);

		public static readonly string HelpdeskAgentGroupDnPrefix = string.Format("CN={0}", EhfCompanyAdmins.HelpdeskAgentGroupNamePrefix);

		private static readonly string OMRoleGroupPrefix = string.Format("CN={0},", "Organization Management");

		private static readonly string VomRoleGroupPrefix = string.Format("CN={0},", "View-Only Organization Management");

		private static readonly Regex EmailAddressRegex = new Regex("^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@([a-z0-9]([a-z0-9\\-]{0,61}[a-z0-9])?\\.)+[a-z]{2,8}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

		private string wellknownGroupName;

		private Dictionary<Guid, MailboxAdminSyncUser> groupMembers = new Dictionary<Guid, MailboxAdminSyncUser>();

		private Dictionary<Guid, AdminSyncUser> subGroups = new Dictionary<Guid, AdminSyncUser>();

		private Dictionary<Guid, PartnerGroupAdminSyncUser> linkedRoleGroups = new Dictionary<Guid, PartnerGroupAdminSyncUser>();

		private Guid externalDirectoryObjectId;
	}
}
