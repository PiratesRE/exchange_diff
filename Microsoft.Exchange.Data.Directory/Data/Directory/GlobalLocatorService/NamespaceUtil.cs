using System;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal static class NamespaceUtil
	{
		internal static string NamespaceWildcard(Namespace ns)
		{
			return NamespaceUtil.NamespaceToString(ns) + ".*";
		}

		internal static string NamespaceToString(Namespace ns)
		{
			switch (ns)
			{
			case Namespace.Invalid:
				throw new ArgumentException("Namespace is Invalid", "ns");
			case Namespace.TestOnly:
				return "ATT";
			case Namespace.Exo:
				return GlsProperty.ExoPrefix;
			case Namespace.Ffo:
				return GlsProperty.FfoPrefix;
			}
			throw new ArgumentException("unknown Namespace value", "ns");
		}
	}
}
