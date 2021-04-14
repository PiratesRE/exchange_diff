using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopReadStream : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.ReadStream;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopReadStream();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ushort byteCountInput, uint? maximumByteCountInput)
		{
			if (byteCountInput != 47806 && maximumByteCountInput != null)
			{
				throw new ArgumentException("maximumByteCount must not have a value when byteCount is not 0xBABE");
			}
			if (byteCountInput == 47806 && maximumByteCountInput == null)
			{
				throw new ArgumentException("maximumByteCount must have a value when byteCount is 0xBABE");
			}
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.byteCount = byteCountInput;
			this.maximumByteCount = maximumByteCountInput;
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteUInt16(this.byteCount);
			if (this.byteCount == 47806)
			{
				if (this.maximumByteCount == null)
				{
					throw new InvalidOperationException("ByteCount is set to 0xBABE, but there was no maximumByteCount");
				}
				writer.WriteUInt32(this.maximumByteCount.Value);
			}
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			RopResult ropResult = RopResult.Parse(reader, new RopResult.ResultParserDelegate(ReadStreamResult.Parse), new RopResult.ResultParserDelegate(ReadStreamResult.Parse));
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

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopReadStream.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.byteCount = reader.ReadUInt16();
			if (this.byteCount == 47806)
			{
				this.maximumByteCount = new uint?(reader.ReadUInt32());
				if (this.maximumByteCount > 2147483647U)
				{
					throw new BufferParseException("Invalid maximum number of bytes to read from stream.");
				}
			}
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override byte[] CreateFakeRopRequest(RopResult result, ServerObjectHandleTable serverObjectHandleTable)
		{
			ReadStreamResult readStreamResult = result as ReadStreamResult;
			if (this.maximumByteCount != null && readStreamResult.ByteCount > 0 && this.maximumByteCount.Value > (uint)readStreamResult.ByteCount)
			{
				RopReadStream ropReadStream = (RopReadStream)RopReadStream.CreateRop();
				ropReadStream.SetInput(base.LogonIndex, base.HandleTableIndex, 47806, new uint?(this.maximumByteCount.Value - (uint)readStreamResult.ByteCount));
				return RopDriver.CreateFakeRopRequest(ropReadStream, serverObjectHandleTable);
			}
			return null;
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			bool flag = this.byteCount == 47806;
			int requestedByteCount = (int)(flag ? this.maximumByteCount.Value : ((uint)this.byteCount));
			ushort num = Rop.ComputeRemainingBufferSize(requestedByteCount, 2, outputBuffer.Count, flag);
			this.result = ropHandler.ReadStream(serverObject, num, RopReadStream.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Bytes=").Append(this.byteCount);
			if (this.maximumByteCount != null)
			{
				stringBuilder.Append(" Maximum Bytes=").Append(this.maximumByteCount.Value);
			}
		}

		private const RopId RopType = RopId.ReadStream;

		public const int MaximumSizeRequestedValue = 47806;

		private static ReadStreamResultFactory resultFactory = new ReadStreamResultFactory();

		private ushort byteCount;

		private uint? maximumByteCount;
	}
}
