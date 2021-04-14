using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class X400Domain
	{
		internal IList<string> Components
		{
			get
			{
				return this.components;
			}
		}

		private X400Domain(IList<string> components)
		{
			this.components = components;
		}

		public static X400Domain Parse(string s)
		{
			X400Domain result;
			if (X400Domain.TryParse(s, out result))
			{
				return result;
			}
			throw new FormatException(DataStrings.InvalidX400Domain(s));
		}

		public static bool TryParse(string s, out X400Domain result)
		{
			IList<string> list;
			if (X400AddressParser.TryParse(s, out list))
			{
				int i = list.Count - 1;
				while (i >= 0 && string.IsNullOrEmpty(list[i]))
				{
					list.RemoveAt(i--);
				}
				for (i = 0; i < list.Count; i++)
				{
					if (list[i] == string.Empty)
					{
						list[i] = null;
					}
					else if (!X400Domain.IsValidComponent(i, list[i]))
					{
						result = null;
						return false;
					}
				}
				if (list.Count > 0 && list.Count < 8)
				{
					result = new X400Domain(list);
					return true;
				}
			}
			result = null;
			return false;
		}

		public override string ToString()
		{
			return X400AddressParser.ToCanonicalString(this.components);
		}

		public bool Match(RoutingX400Address address)
		{
			if (address == null || address.ComponentsCount < this.components.Count)
			{
				return false;
			}
			for (int i = 0; i < this.components.Count; i++)
			{
				if (!X400Domain.MatchOneComponent(this.components[i], address[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static bool MatchOneComponent(string domainComponent, string addressComponent)
		{
			if (domainComponent == null)
			{
				return string.IsNullOrEmpty(addressComponent);
			}
			return string.Equals(domainComponent, addressComponent, StringComparison.OrdinalIgnoreCase);
		}

		private static bool IsValidComponent(int type, string value)
		{
			return value == null || (!value.Contains("*") && value.IndexOf('%') == -1 && ((type == 1 && string.Equals(value, " ", StringComparison.Ordinal)) || value.Trim().Length > 0));
		}

		private IList<string> components;
	}
}
