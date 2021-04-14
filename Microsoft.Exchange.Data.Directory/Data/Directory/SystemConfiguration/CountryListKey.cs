using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class CountryListKey
	{
		public CountryListKey(string countryListName)
		{
			if (string.IsNullOrEmpty(countryListName))
			{
				throw new ArgumentNullException("countryListName");
			}
			ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			this.Key = rootOrgContainerIdForLocalForest.GetDescendantId(CountryList.RdnContainer.GetChildId(countryListName.ToLower()));
			this.cachedHashCode = this.Key.DistinguishedName.ToLower().GetHashCode();
		}

		internal ADObjectId Key { get; private set; }

		public static bool operator ==(CountryListKey key1, CountryListKey key2)
		{
			return object.Equals(key1, key2);
		}

		public static bool operator !=(CountryListKey key1, CountryListKey key2)
		{
			return !object.Equals(key1, key2);
		}

		public override int GetHashCode()
		{
			return this.cachedHashCode;
		}

		public override bool Equals(object obj)
		{
			CountryListKey countryListKey = obj as CountryListKey;
			return !(null == countryListKey) && ADObjectId.Equals(this.Key, countryListKey.Key);
		}

		public override string ToString()
		{
			return this.Key.DistinguishedName.ToLower();
		}

		private readonly int cachedHashCode;
	}
}
