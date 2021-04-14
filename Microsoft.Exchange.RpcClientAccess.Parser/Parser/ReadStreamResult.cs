using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ReadStreamResult : RopResult
	{
		internal ReadStreamResult(ErrorCode errorCode, ArraySegment<byte> dataSegment) : base(RopId.ReadStream, errorCode, null)
		{
			if (dataSegment.Count > 65535)
			{
				throw new ArgumentException("Buffers larger than ushort.MaxValue are not supported", "dataSegment");
			}
			if (errorCode != ErrorCode.None && dataSegment.Count != 0)
			{
				throw new ArgumentException("Cannot use a non-empty buffer with an error", "dataSegment");
			}
			this.dataSegment = dataSegment;
		}

		internal ReadStreamResult(Reader reader) : base(reader)
		{
			ushort count = reader.ReadUInt16();
			this.dataSegment = reader.ReadArraySegment((uint)count);
		}

		public ushort ByteCount
		{
			get
			{
				return (ushort)this.dataSegment.Count;
			}
		}

		public byte[] GetData()
		{
			byte[] array = new byte[this.dataSegment.Count];
			Array.Copy(this.dataSegment.Array, this.dataSegment.Offset, array, 0, this.dataSegment.Count);
			return array;
		}

		public ushort GetData(byte[] dest, uint offset)
		{
			Buffer.BlockCopy(this.dataSegment.Array, this.dataSegment.Offset, dest, (int)offset, this.dataSegment.Count);
			return (ushort)this.dataSegment.Count;
		}

		internal static RopResult Parse(Reader reader)
		{
			return new ReadStreamResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16((ushort)this.dataSegment.Count);
			writer.WriteBytesSegment(this.dataSegment);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" BytesRead=0x").Append(this.dataSegment.Count.ToString("X"));
		}

		internal const int SpecificRopHeaderSize = 2;

		private readonly ArraySegment<byte> dataSegment;
	}
}
