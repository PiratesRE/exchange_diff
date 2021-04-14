using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class EmailAddressItem
	{
		public EmailAddressItem(ProxyAddressBase address)
		{
			this.address = address;
		}

		[DataMember]
		public string Prefix
		{
			get
			{
				return this.address.PrefixString;
			}
			set
			{
			}
		}

		[DataMember]
		public string AddressString
		{
			get
			{
				return this.address.ValueString;
			}
			set
			{
			}
		}

		[DataMember]
		public string Identity
		{
			get
			{
				return string.Format("{0}:{1}", this.address.PrefixString, this.address.ValueString);
			}
			set
			{
			}
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			EmailAddressItem emailAddressItem = obj as EmailAddressItem;
			return emailAddressItem != null && this.Identity.Equals(emailAddressItem.Identity);
		}

		public override int GetHashCode()
		{
			return this.Identity.GetHashCode();
		}

		private ProxyAddressBase address;
	}
}
