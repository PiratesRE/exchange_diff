using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	public struct OfflineAddressBookCacheKey : IComparable, IComparable<OfflineAddressBookCacheKey>, IEquatable<OfflineAddressBookCacheKey>
	{
		[CLSCompliant(false)]
		public OfflineAddressBookCacheKey(ADObjectId key, FilterType type)
		{
			this = new OfflineAddressBookCacheKey(null, key, type);
		}

		[CLSCompliant(false)]
		public OfflineAddressBookCacheKey(OrganizationId orgId, ADObjectId key, FilterType type)
		{
			this.organizationId = orgId;
			this.key = key;
			this.type = type;
		}

		[CLSCompliant(false)]
		public OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		[CLSCompliant(false)]
		public ADObjectId Key
		{
			get
			{
				return this.key;
			}
		}

		public FilterType FilterType
		{
			get
			{
				return this.type;
			}
		}

		public int CompareTo(object other)
		{
			if (other is OfflineAddressBookCacheKey)
			{
				return this.CompareTo((OfflineAddressBookCacheKey)other);
			}
			throw new InvalidOperationException("type mismatch");
		}

		public int CompareTo(OfflineAddressBookCacheKey other)
		{
			string text = (this.key == null) ? string.Empty : this.key.ToString();
			string strB = (other.key == null) ? string.Empty : other.key.ToString();
			int num = text.CompareTo(strB);
			if (num == 0)
			{
				return this.type.CompareTo(other.type);
			}
			return num;
		}

		public bool Equals(OfflineAddressBookCacheKey other)
		{
			return this.CompareTo(other) == 0;
		}

		public override bool Equals(object other)
		{
			if (other is OfflineAddressBookCacheKey)
			{
				return this.Equals((OfflineAddressBookCacheKey)other);
			}
			throw new InvalidOperationException("type mismatch");
		}

		public override int GetHashCode()
		{
			return ((this.key == null) ? 0 : this.key.GetHashCode()) ^ this.type.GetHashCode();
		}

		private readonly OrganizationId organizationId;

		private readonly ADObjectId key;

		private readonly FilterType type;
	}
}
