using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class CaseInsensitiveStringComparer : IStringComparer
	{
		public static CaseInsensitiveStringComparer Instance
		{
			get
			{
				return CaseInsensitiveStringComparer.instance;
			}
		}

		public bool Equals(string x, string y)
		{
			return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
		}

		private static readonly CaseInsensitiveStringComparer instance = new CaseInsensitiveStringComparer();
	}
}
