using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AddressEntryId
	{
		internal AddressEntryId(byte[] entryId)
		{
			this.entryId = entryId;
		}

		public static AddressEntryId Parse(Reader reader, Encoding string8Encoding, uint sizeEntry)
		{
			return new AddressEntryId(reader.ReadBytes(sizeEntry));
		}

		public virtual void Serialize(Writer writer)
		{
			writer.WriteBytes(this.entryId);
		}

		public virtual void SetUnicode()
		{
		}

		public virtual void SetString8(Encoding string8Encoding)
		{
		}

		internal static byte[] ToBytes(BufferWriter.SerializeDelegate serialize)
		{
			return BufferWriter.Serialize(serialize);
		}

		private readonly byte[] entryId;
	}
}
