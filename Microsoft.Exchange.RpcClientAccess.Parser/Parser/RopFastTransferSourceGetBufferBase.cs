using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopFastTransferSourceGetBufferBase : InputRop
	{
		protected bool ReadMaximum
		{
			get
			{
				return this.bufferSize == 47806;
			}
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ushort bufferSize, ushort? maximumBufferSize)
		{
			if (bufferSize != 47806 && maximumBufferSize != null)
			{
				throw new ArgumentException("maximumBufferSize must not have a value when bufferSize is not 0xBABE");
			}
			if (bufferSize == 47806 && maximumBufferSize == null)
			{
				throw new ArgumentException("maximumBufferSize must have a value when bufferSize is 0xBABE");
			}
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.bufferSize = bufferSize;
			this.maximumBufferSize = maximumBufferSize;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16(this.bufferSize);
			if (this.ReadMaximum)
			{
				if (this.maximumBufferSize == null)
				{
					throw new InvalidOperationException("BufferSize is set to 0xBABE, but there was no maximumBufferSize");
				}
				writer.WriteUInt16(this.maximumBufferSize.Value);
			}
		}

		protected void InternalParseOutput(Reader reader, RopFastTransferSourceGetBufferBase.ParseOutputDelegate parseOutput, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			RopResult ropResult = parseOutput(reader);
			if (this.result == null)
			{
				this.result = ropResult;
			}
			if (base.ChainedResults == null)
			{
				base.ChainedResults = new List<RopResult>(10);
			}
			base.ChainedResults.Add(ropResult);
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.bufferSize = reader.ReadUInt16();
			if (this.bufferSize == 47806)
			{
				this.maximumBufferSize = new ushort?(reader.ReadUInt16());
				return;
			}
			this.maximumBufferSize = null;
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override byte[] CreateFakeRopRequest(RopResult result, ServerObjectHandleTable serverObjectHandleTable)
		{
			FastTransferSourceGetBufferResultBase fastTransferSourceGetBufferResultBase = result as FastTransferSourceGetBufferResultBase;
			if ((fastTransferSourceGetBufferResultBase.State == FastTransferState.Partial || fastTransferSourceGetBufferResultBase.State == FastTransferState.NoRoom) && this.ReadMaximum)
			{
				return RopDriver.CreateFakeRopRequest(this, serverObjectHandleTable);
			}
			return null;
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" BufferSize=").Append(this.bufferSize);
			if (this.maximumBufferSize != null)
			{
				stringBuilder.Append(" MaximumBufferSize=").Append(this.maximumBufferSize.Value);
			}
		}

		public const ushort MaximumSizeRequestedValue = 47806;

		protected ushort bufferSize;

		protected ushort? maximumBufferSize;

		protected delegate RopResult ParseOutputDelegate(Reader reader);
	}
}
