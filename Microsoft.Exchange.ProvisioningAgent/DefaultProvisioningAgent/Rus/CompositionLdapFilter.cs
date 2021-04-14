using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal abstract class CompositionLdapFilter : LdapFilter
	{
		public void Add(LdapFilter subFilter)
		{
			this.subFilters.Add(subFilter);
		}

		public void AddRange(IEnumerable<LdapFilter> filters)
		{
			this.subFilters.AddRange(filters);
		}

		public ReadOnlyCollection<LdapFilter> SubFilters
		{
			get
			{
				return this.subFilters.AsReadOnly();
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (LdapFilter ldapFilter in this.SubFilters)
			{
				stringBuilder.Append(ldapFilter.ToString());
			}
			return stringBuilder.ToString();
		}

		private List<LdapFilter> subFilters = new List<LdapFilter>();
	}
}
