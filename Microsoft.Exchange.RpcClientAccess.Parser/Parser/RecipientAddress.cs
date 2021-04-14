using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RecipientAddress
	{
		protected RecipientAddress(RecipientAddressType recipientAddressType)
		{
			this.recipientAddressType = recipientAddressType;
		}

		internal RecipientAddressType RecipientAddressType
		{
			get
			{
				return this.recipientAddressType;
			}
		}

		internal static RecipientAddress Parse(Reader reader, RecipientAddressType recipientAddressType)
		{
			if (recipientAddressType == RecipientAddressType.Exchange)
			{
				return new RecipientAddress.ExchangeRecipientAddress(reader, recipientAddressType);
			}
			switch (recipientAddressType)
			{
			case RecipientAddressType.MapiPrivateDistributionList:
			case RecipientAddressType.DosPrivateDistributionList:
				return new RecipientAddress.DistributionListRecipientAddress(reader, recipientAddressType);
			default:
				if (recipientAddressType == RecipientAddressType.Other)
				{
					return new RecipientAddress.OtherRecipientAddress(reader, recipientAddressType);
				}
				return new RecipientAddress.EmptyRecipientAddress(reader, recipientAddressType);
			}
		}

		internal abstract void Serialize(Writer writer);

		private readonly RecipientAddressType recipientAddressType;

		internal class DistributionListRecipientAddress : RecipientAddress
		{
			public DistributionListRecipientAddress(RecipientAddressType recipientAddressType, byte[] entryId, byte[] searchKey) : base(recipientAddressType)
			{
				if (recipientAddressType != RecipientAddressType.DosPrivateDistributionList && recipientAddressType != RecipientAddressType.MapiPrivateDistributionList)
				{
					throw new ArgumentException(string.Format("Invalid address type: {0}", recipientAddressType), "recipientAddressType");
				}
				Util.ThrowOnNullArgument(entryId, "entryId");
				Util.ThrowOnNullArgument(searchKey, "searchKey");
				this.entryId = entryId;
				this.searchKey = searchKey;
			}

			internal DistributionListRecipientAddress(Reader reader, RecipientAddressType recipientAddressType) : base(recipientAddressType)
			{
				this.entryId = reader.ReadSizeAndByteArray();
				this.searchKey = reader.ReadSizeAndByteArray();
			}

			internal byte[] EntryId
			{
				get
				{
					return this.entryId;
				}
			}

			internal byte[] SearchKey
			{
				get
				{
					return this.searchKey;
				}
			}

			internal override void Serialize(Writer writer)
			{
				writer.WriteSizedBytes(this.entryId);
				writer.WriteSizedBytes(this.searchKey);
			}

			private readonly byte[] entryId;

			private readonly byte[] searchKey;
		}

		internal class ExchangeRecipientAddress : RecipientAddress
		{
			public ExchangeRecipientAddress(RecipientAddressType recipientAddressType, RecipientDisplayType recipientDisplayType, string addressPrefix, string address) : base(recipientAddressType)
			{
				if (recipientAddressType != RecipientAddressType.Exchange)
				{
					throw new ArgumentException(string.Format("Invalid address type: {0}", recipientAddressType), "recipientAddressType");
				}
				Util.ThrowOnNullArgument(addressPrefix, "addressPrefix");
				Util.ThrowOnNullArgument(address, "address");
				this.recipientDisplayType = recipientDisplayType;
				this.addressPrefixLengthUsed = 0;
				while ((int)this.addressPrefixLengthUsed < addressPrefix.Length && (int)this.addressPrefixLengthUsed < address.Length && addressPrefix[(int)this.addressPrefixLengthUsed] == address[(int)this.addressPrefixLengthUsed] && this.addressPrefixLengthUsed < 255)
				{
					this.addressPrefixLengthUsed += 1;
				}
				this.address = address.Substring((int)this.addressPrefixLengthUsed);
			}

			internal ExchangeRecipientAddress(Reader reader, RecipientAddressType recipientAddressType) : base(recipientAddressType)
			{
				this.addressPrefixLengthUsed = reader.ReadByte();
				this.recipientDisplayType = (RecipientDisplayType)reader.ReadByte();
				this.address = reader.ReadAsciiString(StringFlags.IncludeNull);
			}

			internal byte AddressPrefixLengthUsed
			{
				get
				{
					return this.addressPrefixLengthUsed;
				}
			}

			internal string Address
			{
				get
				{
					return this.address;
				}
			}

			internal bool TryGetFullAddress(string addressPrefix, out string fullAddress)
			{
				Util.ThrowOnNullArgument(addressPrefix, "addressPrefix");
				if (this.AddressPrefixLengthUsed == 0)
				{
					fullAddress = this.Address;
					return true;
				}
				if (addressPrefix.Length == (int)this.AddressPrefixLengthUsed)
				{
					fullAddress = addressPrefix + this.Address;
					return true;
				}
				if (addressPrefix.Length > (int)this.AddressPrefixLengthUsed)
				{
					fullAddress = addressPrefix.Substring((int)this.AddressPrefixLengthUsed) + this.Address;
					return true;
				}
				fullAddress = null;
				return false;
			}

			internal RecipientDisplayType RecipientDisplayType
			{
				get
				{
					return this.recipientDisplayType;
				}
			}

			internal override void Serialize(Writer writer)
			{
				writer.WriteByte(this.addressPrefixLengthUsed);
				writer.WriteByte((byte)this.recipientDisplayType);
				writer.WriteAsciiString(this.address, StringFlags.IncludeNull);
			}

			private readonly RecipientDisplayType recipientDisplayType;

			private readonly byte addressPrefixLengthUsed;

			private readonly string address;
		}

		internal class OtherRecipientAddress : RecipientAddress
		{
			public OtherRecipientAddress(RecipientAddressType recipientAddressType, string addressType) : base(recipientAddressType)
			{
				if (recipientAddressType != RecipientAddressType.Other)
				{
					throw new ArgumentException(string.Format("Invalid address type: {0}", recipientAddressType), "recipientAddressType");
				}
				Util.ThrowOnNullArgument(addressType, "addressType");
				this.addressType = addressType;
			}

			internal OtherRecipientAddress(Reader reader, RecipientAddressType recipientAddressType) : base(recipientAddressType)
			{
				this.addressType = reader.ReadAsciiString(StringFlags.IncludeNull);
			}

			internal string AddressType
			{
				get
				{
					return this.addressType;
				}
			}

			internal override void Serialize(Writer writer)
			{
				writer.WriteAsciiString(this.addressType, StringFlags.IncludeNull);
			}

			private readonly string addressType;
		}

		internal class EmptyRecipientAddress : RecipientAddress
		{
			public EmptyRecipientAddress(RecipientAddressType recipientAddressType) : base(recipientAddressType)
			{
				if (recipientAddressType == RecipientAddressType.DosPrivateDistributionList || recipientAddressType == RecipientAddressType.MapiPrivateDistributionList || recipientAddressType == RecipientAddressType.Exchange || recipientAddressType == RecipientAddressType.Other)
				{
					throw new ArgumentException(string.Format("Invalid address type: {0}", recipientAddressType), "recipientAddressType");
				}
			}

			internal EmptyRecipientAddress(Reader reader, RecipientAddressType recipientAddressType) : base(recipientAddressType)
			{
			}

			internal override void Serialize(Writer writer)
			{
			}
		}
	}
}
