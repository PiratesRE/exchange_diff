using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class ExtendedBufferHeader
	{
		public ExtendedBufferHeader(ExtendedBufferFlag flags, ushort payloadSize, ushort uncompressedSize)
		{
			this.flags = flags;
			this.payloadSize = payloadSize;
			this.uncompressedSize = uncompressedSize;
		}

		internal ExtendedBufferHeader(Reader reader)
		{
			if (reader.ReadUInt16() != 0)
			{
				throw new BufferParseException("Extended buffer header version not correct");
			}
			this.flags = (ExtendedBufferFlag)reader.ReadUInt16();
			this.payloadSize = reader.ReadUInt16();
			this.uncompressedSize = reader.ReadUInt16();
		}

		internal ExtendedBufferFlag Flags
		{
			get
			{
				return this.flags;
			}
		}

		internal ushort PayloadSize
		{
			get
			{
				return this.payloadSize;
			}
		}

		internal ushort UncompressedSize
		{
			get
			{
				return this.uncompressedSize;
			}
		}

		internal bool IsCompressed
		{
			get
			{
				return (ushort)(this.flags & ExtendedBufferFlag.Compressed) != 0;
			}
		}

		internal bool IsObfuscated
		{
			get
			{
				return (ushort)(this.flags & ExtendedBufferFlag.Obfuscated) != 0;
			}
		}

		internal bool IsLast
		{
			get
			{
				return (ushort)(this.flags & ExtendedBufferFlag.Last) != 0;
			}
			set
			{
				if (value)
				{
					this.flags |= ExtendedBufferFlag.Last;
					return;
				}
				this.flags &= ~ExtendedBufferFlag.Last;
			}
		}

		internal void Serialize(Writer writer)
		{
			writer.WriteUInt16(0);
			writer.WriteUInt16((ushort)this.flags);
			writer.WriteUInt16(this.payloadSize);
			writer.WriteUInt16(this.uncompressedSize);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		internal void AppendToString(StringBuilder stringBuilder)
		{
			stringBuilder.Append("[ExtendedBufferHeader ");
			stringBuilder.Append(" Flags=[").Append(this.flags).Append("]");
			stringBuilder.Append(" PayloadSize=[").Append(this.payloadSize).Append("]");
			stringBuilder.Append(" UncompressedSize=[").Append(this.uncompressedSize).Append("]");
			stringBuilder.Append("]");
		}

		public const int Size = 8;

		private readonly ushort payloadSize;

		private readonly ushort uncompressedSize;

		private ExtendedBufferFlag flags;
	}
}
