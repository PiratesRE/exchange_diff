using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlType(TypeName = "AddressListSequence")]
	public class AddressListSequence : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "AddressListId")]
		public string AddressListId
		{
			get
			{
				return this.addressListId;
			}
			set
			{
				this.addressListId = value;
			}
		}

		[XmlAttribute(AttributeName = "Sequence")]
		public uint Sequence
		{
			get
			{
				return this.sequence;
			}
			set
			{
				this.sequence = value;
			}
		}

		public override int GetHashCode()
		{
			return (string.IsNullOrEmpty(this.addressListId) ? 0 : this.addressListId.GetHashCode()) ^ (int)this.sequence;
		}

		private string addressListId;

		private uint sequence;
	}
}
