using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	public class OfflineAddressBookCacheEntry
	{
		private OfflineAddressBookCacheEntry(OfflineAddressBook oab)
		{
			this.hasValue = (oab != null);
			if (this.hasValue)
			{
				this.webDistributionEnabled = oab.WebDistributionEnabled;
				this.globalWebDistributionEnabled = oab.GlobalWebDistributionEnabled;
				this.id = oab.Id;
				this.distinguishedName = oab.Id.DistinguishedName;
				this.exchangeObjectId = oab.ExchangeObjectId;
				this.exchangeVersion = oab.ExchangeVersion;
				this.isDefault = oab.IsDefault;
				if (this.isDefault)
				{
					this.configurationUnitId = oab.ConfigurationUnit;
				}
			}
			this.createdTime = DateTime.UtcNow;
		}

		[CLSCompliant(false)]
		public static OfflineAddressBookCacheEntry Create(OfflineAddressBook oab)
		{
			return new OfflineAddressBookCacheEntry(oab);
		}

		public bool HasValue
		{
			get
			{
				return this.hasValue;
			}
		}

		public bool WebDistributionEnabled
		{
			get
			{
				return this.webDistributionEnabled;
			}
		}

		public bool GlobalWebDistributionEnabled
		{
			get
			{
				return this.globalWebDistributionEnabled;
			}
		}

		[CLSCompliant(false)]
		public ADObjectId Id
		{
			get
			{
				return this.id;
			}
		}

		[CLSCompliant(false)]
		public ADObjectId ConfigurationUnitId
		{
			get
			{
				return this.configurationUnitId;
			}
		}

		public string DistinguishedName
		{
			get
			{
				return this.distinguishedName;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.exchangeObjectId;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this.isDefault;
			}
		}

		[CLSCompliant(false)]
		public ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				return this.exchangeVersion;
			}
		}

		public TimeSpan ElapsedTimeSinceCreated
		{
			get
			{
				return DateTime.UtcNow - this.createdTime;
			}
		}

		public void NullConfigurationUnit()
		{
			this.configurationUnitId = null;
		}

		private readonly bool hasValue;

		private readonly bool webDistributionEnabled;

		private readonly bool globalWebDistributionEnabled;

		private readonly ADObjectId id;

		private readonly string distinguishedName;

		private readonly Guid exchangeObjectId;

		private readonly bool isDefault;

		private readonly ExchangeObjectVersion exchangeVersion;

		private readonly DateTime createdTime;

		private ADObjectId configurationUnitId;
	}
}
