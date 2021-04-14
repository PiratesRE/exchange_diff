using System;
using System.Text;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class NotLdapFilter : CompositionLdapFilter
	{
		public NotLdapFilter(LdapFilter subfFilter)
		{
			base.Add(subfFilter);
		}

		public NotLdapFilter()
		{
		}

		public override bool Evaluate(object[] marshalledAttributes)
		{
			bool result = false;
			int index = 0;
			if (base.SubFilters.Count != 0)
			{
				result = !base.SubFilters[index].Evaluate(marshalledAttributes);
			}
			return result;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(!");
			stringBuilder.Append(base.ToString());
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}
	}
}
