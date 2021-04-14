using System;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "OfflineAddressBookManifestVersion")]
	public class OfflineAddressBookManifestVersion : XMLSerializableBase, IEquatable<OfflineAddressBookManifestVersion>
	{
		[XmlElement(ElementName = "AddressLists")]
		public AddressListSequence[] AddressLists
		{
			get
			{
				return this.addressLists;
			}
			set
			{
				this.addressLists = value;
			}
		}

		public bool HasValue
		{
			get
			{
				return this.addressLists != null && this.addressLists.Length > 0;
			}
		}

		public override int GetHashCode()
		{
			int num = 0;
			foreach (AddressListSequence addressListSequence in this.addressLists)
			{
				num ^= ((addressListSequence == null) ? 0 : addressListSequence.GetHashCode());
			}
			return num;
		}

		public override bool Equals(object other)
		{
			return other != null && other is OfflineAddressBookManifestVersion && this.Equals((OfflineAddressBookManifestVersion)other);
		}

		public bool Equals(OfflineAddressBookManifestVersion other)
		{
			if (other == null)
			{
				return false;
			}
			if (!this.HasValue && !other.HasValue)
			{
				return true;
			}
			if (!this.HasValue || !other.HasValue)
			{
				return false;
			}
			if (this.addressLists.Length != other.addressLists.Length)
			{
				return false;
			}
			foreach (AddressListSequence addressListSequence in this.addressLists)
			{
				bool flag = false;
				foreach (AddressListSequence addressListSequence2 in other.addressLists)
				{
					if (string.Compare(addressListSequence.AddressListId, addressListSequence2.AddressListId, StringComparison.OrdinalIgnoreCase) == 0 && addressListSequence.Sequence == addressListSequence2.Sequence)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			foreach (AddressListSequence addressListSequence in this.addressLists)
			{
				stringBuilder.Append(string.Format("({0},{1})", addressListSequence.AddressListId, addressListSequence.Sequence));
			}
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private AddressListSequence[] addressLists;
	}
}
