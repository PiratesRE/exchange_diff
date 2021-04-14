using System;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Exchange.Data
{
	internal class DelegatedPrincipal : GenericPrincipal
	{
		internal DelegatedPrincipal(IIdentity identity, string[] roles) : base(identity, roles)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (string.IsNullOrEmpty(identity.Name))
			{
				throw new ArgumentNullException("identity.Name");
			}
			if (!(identity is GenericIdentity) || !DelegatedPrincipal.DelegatedAuthenticationType.Equals(identity.AuthenticationType, StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("identity");
			}
			if (!DelegatedPrincipal.TryParseDelegatedString(identity.Name, out this.userId, out this.userOrgId, out this.delegatedOrg, out this.displayName, out this.groups))
			{
				throw new ArgumentException("identity.Name");
			}
		}

		private DelegatedPrincipal(string delegatedIdentity, string userId, string userOrgId, string delegatedOrg, string displayName, string[] groups) : base(new GenericIdentity(delegatedIdentity, DelegatedPrincipal.DelegatedAuthenticationType), null)
		{
			if (string.IsNullOrEmpty(delegatedIdentity))
			{
				throw new ArgumentNullException("delegatedIdentity");
			}
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("userId");
			}
			if (string.IsNullOrEmpty(userOrgId))
			{
				throw new ArgumentNullException("userOrgId");
			}
			if (string.IsNullOrEmpty(delegatedOrg))
			{
				throw new ArgumentNullException("delegatedOrg");
			}
			if (string.IsNullOrEmpty(displayName))
			{
				throw new ArgumentNullException("displayName");
			}
			this.userId = userId;
			this.userOrgId = userOrgId;
			this.delegatedOrg = delegatedOrg;
			this.displayName = displayName;
			this.groups = groups;
		}

		internal string[] Roles
		{
			get
			{
				return this.groups;
			}
		}

		internal string UserId
		{
			get
			{
				return this.userId;
			}
		}

		internal string UserOrganizationId
		{
			get
			{
				return this.userOrgId;
			}
		}

		internal string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		internal string DelegatedOrganization
		{
			get
			{
				return this.delegatedOrg;
			}
		}

		internal static IIdentity GetDelegatedIdentity(string userId, string userOrgId, string targetOrg, string displayName, string[] groups)
		{
			return new GenericIdentity(DelegatedPrincipal.ToString(userId, userOrgId, targetOrg, displayName, groups), DelegatedPrincipal.DelegatedAuthenticationType);
		}

		internal static IIdentity GetDelegatedIdentity(string delegatedStr)
		{
			string text;
			string text2;
			string text3;
			string text4;
			string[] array;
			if (!DelegatedPrincipal.TryParseDelegatedString(delegatedStr, out text, out text2, out text3, out text4, out array))
			{
				throw new ArgumentException("delegatedStr");
			}
			return new GenericIdentity(DelegatedPrincipal.ToString(text, text2, text3, text4, array), DelegatedPrincipal.DelegatedAuthenticationType);
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Format("{0}\\{1}", this.delegatedOrg, this.userId);
			}
			return this.toString;
		}

		public string GetUserName()
		{
			if (this.ToString().Length <= DelegatedPrincipal.MaxNameLength)
			{
				return this.ToString();
			}
			if (this.userId.Length <= DelegatedPrincipal.MaxNameLength)
			{
				return this.userId;
			}
			return this.userId.Substring(0, DelegatedPrincipal.MaxNameLength);
		}

		internal static bool TryParseDelegatedString(string delegatedStr, out DelegatedPrincipal principal)
		{
			principal = null;
			string text;
			string text2;
			string text3;
			string text4;
			string[] array;
			if (!DelegatedPrincipal.TryParseDelegatedString(delegatedStr, out text, out text2, out text3, out text4, out array))
			{
				return false;
			}
			principal = new DelegatedPrincipal(delegatedStr, text, text2, text3, text4, array);
			return true;
		}

		private static bool TryParseDelegatedString(string delegatedStr, out string userId, out string userOrgId, out string delegatedOrg, out string displayName, out string[] groups)
		{
			userId = null;
			userOrgId = null;
			delegatedOrg = null;
			displayName = null;
			groups = null;
			if (string.IsNullOrEmpty(delegatedStr))
			{
				return false;
			}
			string[] array = delegatedStr.Split(new char[]
			{
				DelegatedPrincipal.Separator
			});
			if (array.Length < DelegatedPrincipal.MinNumberOfComponents)
			{
				return false;
			}
			if (array.Length > DelegatedPrincipal.MinNumberOfComponents)
			{
				groups = new string[array.Length - DelegatedPrincipal.MinNumberOfComponents];
				try
				{
					Array.ConstrainedCopy(array, DelegatedPrincipal.MinNumberOfComponents, groups, 0, array.Length - DelegatedPrincipal.MinNumberOfComponents);
					goto IL_81;
				}
				catch (Exception)
				{
					return false;
				}
			}
			groups = new string[0];
			IL_81:
			userId = Uri.UnescapeDataString(array[0]);
			userOrgId = array[1];
			delegatedOrg = Uri.UnescapeDataString(array[2]);
			displayName = Uri.UnescapeDataString(array[3]);
			return true;
		}

		private static string ToString(string userId, string userOrgId, string delegatedOrg, string displayName, string[] groups)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("userId");
			}
			if (string.IsNullOrEmpty(userOrgId))
			{
				throw new ArgumentNullException("userOrgId");
			}
			if (string.IsNullOrEmpty(delegatedOrg))
			{
				throw new ArgumentNullException("delegatedOrg");
			}
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			if (groups == null)
			{
				throw new ArgumentNullException("groups");
			}
			string text = Uri.EscapeDataString(userId);
			string text2 = Uri.EscapeDataString(delegatedOrg);
			string text3 = Uri.EscapeDataString(displayName);
			StringBuilder stringBuilder = new StringBuilder(text.Length + userOrgId.Length + text2.Length + text3.Length + groups.Length * 32 + groups.Length + 4);
			stringBuilder.Append(text);
			stringBuilder.Append(DelegatedPrincipal.Separator);
			stringBuilder.Append(userOrgId);
			stringBuilder.Append(DelegatedPrincipal.Separator);
			stringBuilder.Append(text2);
			stringBuilder.Append(DelegatedPrincipal.Separator);
			stringBuilder.Append(text3);
			stringBuilder.Append(DelegatedPrincipal.Separator);
			for (int i = 0; i < groups.Length; i++)
			{
				stringBuilder.Append(groups[i]);
				if (i + 1 < groups.Length)
				{
					stringBuilder.Append(DelegatedPrincipal.Separator);
				}
			}
			return stringBuilder.ToString();
		}

		internal static readonly string DelegatedAuthenticationType = "DelegatedPartnerTenant";

		internal static readonly char Separator = ',';

		internal static readonly char ExpirationSeparator = '&';

		internal static readonly int MaxNameLength = 64;

		private static int MinNumberOfComponents = 4;

		private string[] groups;

		private string userId;

		private string userOrgId;

		private string delegatedOrg;

		private string displayName;

		private string toString;
	}
}
