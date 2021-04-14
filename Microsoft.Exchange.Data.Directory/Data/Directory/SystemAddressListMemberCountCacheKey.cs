using System;

namespace Microsoft.Exchange.Data.Directory
{
	public class SystemAddressListMemberCountCacheKey : IEquatable<SystemAddressListMemberCountCacheKey>
	{
		public SystemAddressListMemberCountCacheKey(OrganizationId orgId, string systemAddressListName)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			if (string.IsNullOrEmpty(systemAddressListName))
			{
				throw new ArgumentNullException("systemAddressListName");
			}
			if (orgId.ConfigurationUnit == null || string.IsNullOrEmpty(orgId.ConfigurationUnit.DistinguishedName))
			{
				this.configUnitDN = Guid.Empty.ToString();
			}
			else
			{
				this.configUnitDN = orgId.ConfigurationUnit.DistinguishedName.ToLower();
			}
			this.systemAddressListName = systemAddressListName.ToLower();
		}

		public static bool Equals(SystemAddressListMemberCountCacheKey key1, SystemAddressListMemberCountCacheKey key2)
		{
			return string.Equals(key1.configUnitDN, key2.configUnitDN) && string.Equals(key1.systemAddressListName, key2.systemAddressListName);
		}

		public bool Equals(SystemAddressListMemberCountCacheKey other)
		{
			return string.Equals(this.configUnitDN, other.configUnitDN) && string.Equals(this.systemAddressListName, other.systemAddressListName);
		}

		public override bool Equals(object o)
		{
			SystemAddressListMemberCountCacheKey systemAddressListMemberCountCacheKey = o as SystemAddressListMemberCountCacheKey;
			return systemAddressListMemberCountCacheKey != null && string.Equals(this.configUnitDN, systemAddressListMemberCountCacheKey.configUnitDN) && string.Equals(this.systemAddressListName, systemAddressListMemberCountCacheKey.systemAddressListName);
		}

		public override int GetHashCode()
		{
			return this.configUnitDN.GetHashCode() ^ this.systemAddressListName.GetHashCode();
		}

		private string configUnitDN;

		private string systemAddressListName;
	}
}
