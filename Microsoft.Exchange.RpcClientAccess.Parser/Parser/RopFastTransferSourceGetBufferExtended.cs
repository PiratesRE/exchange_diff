using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFastTransferSourceGetBufferExtended : RopFastTransferSourceGetBufferBase
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FastTransferSourceGetBufferExtended;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFastTransferSourceGetBufferExtended();
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			base.InternalParseOutput(reader, new RopFastTransferSourceGetBufferBase.ParseOutputDelegate(FastTransferSourceGetBufferExtendedResult.Parse), string8Encoding);
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			int requestedByteCount = (int)(base.ReadMaximum ? this.maximumBufferSize.Value : this.bufferSize);
			ushort num = Rop.ComputeRemainingBufferSize(requestedByteCount, 13, outputBuffer.Count, base.ReadMaximum);
			ArraySegment<byte> outputBuffer2 = outputBuffer.SubSegment(FastTransferSourceGetBufferExtendedResult.FullHeaderSize, (int)num);
			FastTransferSourceGetBufferExtendedResultFactory resultFactory = new FastTransferSourceGetBufferExtendedResultFactory(outputBuffer2);
			this.result = ropHandler.FastTransferSourceGetBufferExtended(serverObject, num, resultFactory);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return FastTransferSourceGetBufferExtendedResultFactory.Empty;
		}

		private const RopId RopType = RopId.FastTransferSourceGetBufferExtended;
	}
}
