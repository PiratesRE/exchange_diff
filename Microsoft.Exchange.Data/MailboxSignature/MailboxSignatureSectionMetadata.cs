using System;
using Microsoft.Exchange.Data.Serialization;

namespace Microsoft.Exchange.Data.MailboxSignature
{
	internal class MailboxSignatureSectionMetadata
	{
		internal MailboxSignatureSectionMetadata(MailboxSignatureSectionType type, short version, int elementsNumber, int length)
		{
			this.type = type;
			this.version = version;
			this.elementsNumber = elementsNumber;
			this.length = length;
			this.Validate();
		}

		internal MailboxSignatureSectionType Type
		{
			get
			{
				return this.type;
			}
		}

		internal short Version
		{
			get
			{
				return this.version;
			}
		}

		internal int Length
		{
			get
			{
				return this.length;
			}
		}

		internal int ElementsNumber
		{
			get
			{
				return this.elementsNumber;
			}
		}

		internal static MailboxSignatureSectionMetadata Parse(byte[] buffer, ref int offset)
		{
			if (offset > buffer.Length || buffer.Length - offset < 12)
			{
				throw new ArgumentException("Parse: insufficient buffer.");
			}
			return new MailboxSignatureSectionMetadata((MailboxSignatureSectionType)Serialization.DeserializeUInt16(buffer, ref offset), (short)Serialization.DeserializeUInt16(buffer, ref offset), (int)Serialization.DeserializeUInt32(buffer, ref offset), (int)Serialization.DeserializeUInt32(buffer, ref offset));
		}

		internal int Serialize(byte[] buffer, int offset)
		{
			if (buffer == null)
			{
				return 12;
			}
			if (offset > buffer.Length || buffer.Length - offset < 12)
			{
				throw new ArgumentException("Serialize: insufficient buffer.");
			}
			Serialization.SerializeUInt16(buffer, ref offset, (ushort)this.type);
			Serialization.SerializeUInt16(buffer, ref offset, (ushort)this.version);
			Serialization.SerializeUInt32(buffer, ref offset, (uint)this.elementsNumber);
			Serialization.SerializeUInt32(buffer, ref offset, (uint)this.length);
			return 12;
		}

		private void Validate()
		{
			if (this.elementsNumber < 0)
			{
				throw new ArgumentException("Invalid number of section elements.");
			}
			if (this.length < 0)
			{
				throw new ArgumentException("Invalid section length.");
			}
			if (this.length != 0 && this.elementsNumber == 0)
			{
				throw new ArgumentException("Zero elements serialized to non-zero length.");
			}
		}

		public const int SerializedSize = 12;

		private readonly MailboxSignatureSectionType type;

		private readonly short version;

		private readonly int elementsNumber;

		private readonly int length;
	}
}
