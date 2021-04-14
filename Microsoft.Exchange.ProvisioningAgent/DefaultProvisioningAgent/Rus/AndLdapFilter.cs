using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class AndLdapFilter : CompositionLdapFilter
	{
		public AndLdapFilter(IEnumerable<LdapFilter> subfFilters)
		{
			base.AddRange(subfFilters);
		}

		public AndLdapFilter()
		{
		}

		public override bool Evaluate(object[] marshalledAttributes)
		{
			bool flag = true;
			int num = 0;
			while (flag && num < base.SubFilters.Count)
			{
				flag = (flag && base.SubFilters[num].Evaluate(marshalledAttributes));
				num++;
			}
			return flag;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(&");
			stringBuilder.Append(base.ToString());
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}
	}
}
