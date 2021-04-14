using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class IPAllowListProvider : IPListProvider
	{
		internal override ADObjectId ParentPath
		{
			get
			{
				return IPAllowListProvider.parentPath;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return IPAllowListProvider.mostDerivedClass;
			}
		}

		private static string mostDerivedClass = "msExchMessageHygieneIPAllowListProvider";

		private static ADObjectId parentPath = new ADObjectId("CN=IPAllowListProviderConfig,CN=Message Hygiene,CN=Transport Settings");
	}
}
