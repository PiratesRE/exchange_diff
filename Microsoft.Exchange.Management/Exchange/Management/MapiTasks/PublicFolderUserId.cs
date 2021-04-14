using System;
using System.ComponentModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MapiTasks
{
	[ImmutableObject(true)]
	[Serializable]
	public sealed class PublicFolderUserId : ObjectId, IEquatable<PublicFolderUserId>
	{
		private PublicFolderUserId(bool isDefault, bool isAnonymous)
		{
			this.isDefault = isDefault;
			this.isAnonymous = isAnonymous;
		}

		public PublicFolderUserId(ADObjectId activeDirectoryId, string exchangeLegacyDN, MapiEntryId exchangeAddressBookEntryId, string exchangeAddressBookDisplayName)
		{
			if (null == exchangeAddressBookEntryId)
			{
				throw new ArgumentNullException("exchangeAddressBookEntryId");
			}
			if (exchangeAddressBookDisplayName == null)
			{
				throw new ArgumentNullException("exchangeAddressBookDisplayName");
			}
			if (activeDirectoryId != null && string.IsNullOrEmpty(activeDirectoryId.DistinguishedName) && Guid.Empty == activeDirectoryId.ObjectGuid)
			{
				throw new ArgumentException("activeDirectoryId is Invalid", "activeDirectoryId");
			}
			this.activeDirectoryId = activeDirectoryId;
			this.exchangeLegacyDN = exchangeLegacyDN;
			this.exchangeAddressBookEntryId = exchangeAddressBookEntryId;
			this.exchangeAddressBookDisplayName = exchangeAddressBookDisplayName;
		}

		public override byte[] GetBytes()
		{
			throw new NotImplementedException();
		}

		public ADObjectId ActiveDirectoryIdentity
		{
			get
			{
				return this.activeDirectoryId;
			}
			internal set
			{
				this.activeDirectoryId = value;
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return this.exchangeLegacyDN;
			}
			internal set
			{
				this.exchangeLegacyDN = value;
			}
		}

		public MapiEntryId ExchangeAddressBookEntryId
		{
			get
			{
				return this.exchangeAddressBookEntryId;
			}
		}

		public string ExchangeAddressBookDisplayName
		{
			get
			{
				return this.exchangeAddressBookDisplayName;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this.isDefault;
			}
		}

		public bool IsAnonymous
		{
			get
			{
				return this.isAnonymous;
			}
		}

		public override string ToString()
		{
			if (this.isAnonymous)
			{
				return Strings.AnonymousUser;
			}
			if (this.isDefault)
			{
				return Strings.DefaultUser;
			}
			if (this.ActiveDirectoryIdentity != null)
			{
				return this.ActiveDirectoryIdentity.ToString();
			}
			if (!string.IsNullOrEmpty(this.ExchangeLegacyDN))
			{
				return this.ExchangeLegacyDN;
			}
			if (string.IsNullOrEmpty(this.ExchangeAddressBookDisplayName))
			{
				return this.ExchangeAddressBookEntryId.ToString();
			}
			return this.ExchangeAddressBookDisplayName;
		}

		public override int GetHashCode()
		{
			if (!(null == this.ExchangeAddressBookEntryId))
			{
				return this.ExchangeAddressBookEntryId.GetHashCode();
			}
			if (!this.IsDefault)
			{
				return 32767;
			}
			return 2147418112;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as PublicFolderUserId);
		}

		public bool Equals(PublicFolderUserId other)
		{
			if (null == other)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(this.ExchangeLegacyDN) && !string.IsNullOrEmpty(other.ExchangeLegacyDN))
			{
				return string.Equals(this.ExchangeLegacyDN, other.ExchangeLegacyDN, StringComparison.InvariantCultureIgnoreCase);
			}
			if (null == this.ExchangeAddressBookEntryId)
			{
				return this.IsAnonymous == other.IsAnonymous && this.IsDefault == other.IsDefault;
			}
			return this.ExchangeAddressBookEntryId == other.ExchangeAddressBookEntryId;
		}

		public static bool operator ==(PublicFolderUserId operand1, PublicFolderUserId operand2)
		{
			return object.Equals(operand1, operand2);
		}

		public static bool operator !=(PublicFolderUserId operand1, PublicFolderUserId operand2)
		{
			return !object.Equals(operand1, operand2);
		}

		private string exchangeLegacyDN;

		private ADObjectId activeDirectoryId;

		private MapiEntryId exchangeAddressBookEntryId;

		private readonly string exchangeAddressBookDisplayName;

		private readonly bool isDefault;

		private readonly bool isAnonymous;

		public static PublicFolderUserId DefaultUserId = new PublicFolderUserId(true, false);

		public static PublicFolderUserId AnonymousUserId = new PublicFolderUserId(false, true);
	}
}
