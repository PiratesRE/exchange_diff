using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class RoutingX400Address
	{
		private RoutingX400Address(IList<string> components)
		{
			this.components = components;
		}

		public int ComponentsCount
		{
			get
			{
				return this.components.Count;
			}
		}

		public string this[int componentIndex]
		{
			get
			{
				return this.components[componentIndex];
			}
		}

		public override string ToString()
		{
			return X400AddressParser.ToCanonicalString(this.components);
		}

		public static bool TryParse(string s, out RoutingX400Address address)
		{
			return RoutingX400Address.TryParse(s, false, false, out address);
		}

		public static bool TryParseAddressSpace(string s, bool locallyScoped, out RoutingX400Address address)
		{
			return RoutingX400Address.TryParse(s, true, locallyScoped, out address);
		}

		private static bool TryParse(string s, bool addressSpace, bool locallyScoped, out RoutingX400Address address)
		{
			address = null;
			IList<string> list = null;
			if (!X400AddressParser.TryParse(s, 8, addressSpace, locallyScoped, out list))
			{
				return false;
			}
			int i = 8;
			if (addressSpace)
			{
				while (i > 0)
				{
					if (list[i - 1] != null)
					{
						break;
					}
					list.RemoveAt(--i);
				}
				while (i > 0)
				{
					string text = list[i - 1];
					if (text == null || !text.Equals("*", StringComparison.OrdinalIgnoreCase))
					{
						break;
					}
					list.RemoveAt(--i);
				}
			}
			while (i > 0)
			{
				if (list[i - 1] == null)
				{
					list[i - 1] = string.Empty;
				}
				i--;
			}
			address = new RoutingX400Address(list);
			return true;
		}

		public const char SingleCharWildcard = '%';

		public const string AdmdWildcard = " ";

		public const string Wildcard = "*";

		internal const int RoutingComponentsCount = 8;

		private IList<string> components;
	}
}
