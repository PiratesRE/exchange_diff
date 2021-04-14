using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DiagnosticContextResult : RopResult
	{
		internal DiagnosticContextResult(uint threadId, uint requestId, DiagnosticContextFlags flags, byte[] data) : base(RopId.DiagnosticContext, ErrorCode.None, null)
		{
			this.threadId = threadId;
			this.requestId = requestId;
			this.flags = flags;
			this.data = data;
		}

		internal DiagnosticContextResult(Reader reader) : base(reader)
		{
			byte b = reader.ReadByte();
			if (b != 255)
			{
				throw new BufferParseException("Did not recognize DiagnosticContext format: " + b);
			}
			this.threadId = reader.ReadUInt32();
			this.requestId = reader.ReadUInt32();
			this.flags = (DiagnosticContextFlags)reader.ReadByte();
			uint count = reader.ReadUInt32();
			this.data = reader.ReadBytes(count);
		}

		public uint ThreadId
		{
			get
			{
				return this.threadId;
			}
		}

		public uint RequestId
		{
			get
			{
				return this.requestId;
			}
		}

		public DiagnosticContextFlags Flags
		{
			get
			{
				return this.flags;
			}
		}

		public byte[] Data
		{
			get
			{
				return this.data;
			}
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte(byte.MaxValue);
			writer.WriteUInt32(this.threadId);
			writer.WriteUInt32(this.requestId);
			writer.WriteByte((byte)this.flags);
			writer.WriteUInt32((uint)this.data.Length);
			writer.WriteBytes(this.data);
		}

		internal const int DiagnosticHeaderSize = 20;

		private readonly uint threadId;

		private readonly uint requestId;

		private readonly DiagnosticContextFlags flags;

		private readonly byte[] data;
	}
}
