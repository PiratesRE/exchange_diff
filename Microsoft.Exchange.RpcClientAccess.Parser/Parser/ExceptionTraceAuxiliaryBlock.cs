using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ExceptionTraceAuxiliaryBlock : AuxiliaryBlock
	{
		public ExceptionTraceAuxiliaryBlock(uint ropIndex, string exceptionTrace) : base(1, AuxiliaryBlockTypes.ExceptionTrace)
		{
			this.ropIndex = ropIndex;
			this.exceptionTrace = exceptionTrace;
		}

		internal ExceptionTraceAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.ropIndex = reader.ReadUInt32();
			this.exceptionTrace = reader.ReadString8(this.traceEncoding, StringFlags.IncludeNull);
		}

		public uint RopIndex
		{
			get
			{
				return this.ropIndex;
			}
		}

		public string ExceptionTrace
		{
			get
			{
				return this.exceptionTrace;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.ropIndex);
			writer.WriteString8(this.exceptionTrace, this.traceEncoding, StringFlags.IncludeNull);
		}

		protected override int Truncate(int maxSerializedSize, int currentSize)
		{
			byte[] bytes = this.traceEncoding.GetBytes(this.ExceptionTrace);
			if (currentSize > maxSerializedSize && currentSize - maxSerializedSize < bytes.Length)
			{
				this.exceptionTrace = this.traceEncoding.GetString(bytes, 0, maxSerializedSize - (currentSize - bytes.Length));
				return maxSerializedSize;
			}
			return currentSize;
		}

		private readonly Encoding traceEncoding = Encoding.UTF8;

		private readonly uint ropIndex;

		private string exceptionTrace;
	}
}
