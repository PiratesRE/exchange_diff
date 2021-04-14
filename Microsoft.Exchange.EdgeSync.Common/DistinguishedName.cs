using System;
using System.Text;

namespace Microsoft.Exchange.EdgeSync
{
	internal static class DistinguishedName
	{
		public static bool IsEmpty(string name)
		{
			return string.IsNullOrEmpty(name);
		}

		public static string Parent(string name)
		{
			if (DistinguishedName.IsEmpty(name))
			{
				throw new InvalidOperationException("cannot get parent of empty");
			}
			int num = name.IndexOf(',');
			if (num < 0)
			{
				return DistinguishedName.Empty;
			}
			return name.Substring(num + 1);
		}

		public static bool IsChildOf(string child, string parent)
		{
			return child.EndsWith(parent);
		}

		public static string Concatinate(params string[] components)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (string text in components)
			{
				if (!DistinguishedName.IsEmpty(text))
				{
					if (!flag)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(text);
					flag = false;
				}
			}
			return stringBuilder.ToString();
		}

		public static string MakeRelativePath(string child, string parent)
		{
			if (!DistinguishedName.IsChildOf(child, parent))
			{
				throw new InvalidOperationException("invalid suffix");
			}
			if (DistinguishedName.Equals(child, parent))
			{
				return DistinguishedName.Empty;
			}
			return child.Substring(0, child.Length - parent.Length - 1);
		}

		public static bool Equals(string s1, string s2)
		{
			return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
		}

		public static string RemoveLeafRelativeDistinguishedNames(string name, int n)
		{
			int num = 1;
			while (n > 0)
			{
				num = name.IndexOf(',', num);
				if (num < 0)
				{
					throw new InvalidOperationException("no RDN to remove");
				}
				num++;
				n--;
			}
			return name.Substring(num);
		}

		public static string ExtractRDN(string name)
		{
			return name.Split(new char[]
			{
				','
			}, 2)[0];
		}

		public const string EdgeOrganizationRelativePath = "CN=First Organization,CN=Microsoft Exchange,CN=Services";

		public const string EdgeServersRelativePath = "CN=Servers,CN=Exchange Administrative Group (FYDIBOHF23SPDLT),CN=Administrative Groups,CN=First Organization,CN=Microsoft Exchange,CN=Services";

		public static readonly string Empty = string.Empty;
	}
}
