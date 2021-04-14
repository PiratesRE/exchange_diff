using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFastTransferSourceGetBuffer : RopFastTransferSourceGetBufferBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FastTransferSourceGetBuffer;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFastTransferSourceGetBuffer();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			base.InternalParseOutput(reader, new RopFastTransferSourceGetBufferBase.ParseOutputDelegate(FastTransferSourceGetBufferResult.Parse), string8Encoding);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			int requestedByteCount = (int)(base.ReadMaximum ? this.maximumBufferSize.Value : this.bufferSize);
			ushort num = Rop.ComputeRemainingBufferSize(requestedByteCount, 9, outputBuffer.Count, base.ReadMaximum);
			ArraySegment<byte> outputBuffer2 = outputBuffer.SubSegment(FastTransferSourceGetBufferResult.FullHeaderSize, (int)num);
			FastTransferSourceGetBufferResultFactory resultFactory = new FastTransferSourceGetBufferResultFactory(outputBuffer2);
			this.result = ropHandler.FastTransferSourceGetBuffer(serverObject, num, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return FastTransferSourceGetBufferResultFactory.Empty;
		}

		private const RopId RopType = RopId.FastTransferSourceGetBuffer;
	}
}
