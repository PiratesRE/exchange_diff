using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class CustomLdapFilter : QueryFilter
	{
		public CustomLdapFilter(string ldapFilter)
		{
			if (string.IsNullOrEmpty(ldapFilter))
			{
				throw new ArgumentNullException("ldapFilter");
			}
			this.ldapFilter = ldapFilter;
		}

		public string LdapFilter
		{
			get
			{
				return this.ldapFilter;
			}
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("([LDAP](");
			sb.Append(this.ldapFilter);
			sb.Append("))");
		}

		public override bool Equals(object obj)
		{
			CustomLdapFilter customLdapFilter = obj as CustomLdapFilter;
			return customLdapFilter != null && this.LdapFilter.Equals(customLdapFilter.LdapFilter, StringComparison.OrdinalIgnoreCase);
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode() ^ this.LdapFilter.GetHashCode();
		}

		private readonly string ldapFilter;
	}
}
