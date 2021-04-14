using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class OrLdapFilter : CompositionLdapFilter
	{
		public OrLdapFilter(IEnumerable<LdapFilter> subfFilters)
		{
			base.AddRange(subfFilters);
		}

		public OrLdapFilter()
		{
		}

		public override bool Evaluate(object[] marshalledAttributest)
		{
			bool flag = false;
			int num = 0;
			while (!flag && num < base.SubFilters.Count)
			{
				flag = (flag || base.SubFilters[num].Evaluate(marshalledAttributest));
				num++;
			}
			return flag;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(|");
			stringBuilder.Append(base.ToString());
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}
	}
}
