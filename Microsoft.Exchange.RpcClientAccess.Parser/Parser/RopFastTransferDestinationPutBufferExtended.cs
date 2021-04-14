using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopFastTransferDestinationPutBufferExtended : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.FastTransferDestinationPutBufferExtended;
			}
		}

		internal List<ArraySegment<byte>> DataChunks
		{
			get
			{
				return this.dataChunks;
			}
		}

		internal static Rop CreateRop()
		{
			return new RopFastTransferDestinationPutBufferExtended();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ArraySegment<byte>[] dataChunks)
		{
			Util.ThrowOnNullArgument(dataChunks, "dataChunks");
			base.SetCommonInput(logonIndex, handleTableIndex);
			this.dataChunks = new List<ArraySegment<byte>>(dataChunks);
		}

		protected override void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			base.InternalSerializeInput(writer, string8Encoding);
			writer.WriteSizedBytesSegment(this.dataChunks[0], FieldLength.WordSize);
		}

		protected override void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
			base.InternalParseOutput(reader, string8Encoding);
			this.result = new FastTransferDestinationPutBufferExtendedResult(reader);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopFastTransferDestinationPutBufferExtended.resultFactory;
		}

		protected override void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			base.InternalParseInput(reader, serverObjectHandleTable);
			this.dataChunks = new List<ArraySegment<byte>>(1);
			this.dataChunks.Add(reader.ReadSizeAndByteArraySegment());
		}

		protected override void InternalSerializeOutput(Writer writer)
		{
			base.InternalSerializeOutput(writer);
			this.result.Serialize(writer);
		}

		internal override void MergeChainedData(Rop rop)
		{
			RopFastTransferDestinationPutBufferExtended ropFastTransferDestinationPutBufferExtended = rop as RopFastTransferDestinationPutBufferExtended;
			if (ropFastTransferDestinationPutBufferExtended == null)
			{
				throw new BufferParseException("Chained Rop not same type");
			}
			if (ropFastTransferDestinationPutBufferExtended.LogonIndex != base.LogonIndex)
			{
				throw new BufferParseException("Chained Rop not same logon index");
			}
			if (ropFastTransferDestinationPutBufferExtended.HandleTableIndex != base.HandleTableIndex)
			{
				throw new BufferParseException("Chained Rop not same SOHT index");
			}
			foreach (ArraySegment<byte> item in ropFastTransferDestinationPutBufferExtended.DataChunks)
			{
				this.dataChunks.Add(item);
			}
		}

		internal override IEnumerator<Rop> SplitChainedData()
		{
			if (this.dataChunks.Count > 1)
			{
				for (int i = 1; i < this.dataChunks.Count; i++)
				{
					RopFastTransferDestinationPutBuffer rop = new RopFastTransferDestinationPutBuffer();
					rop.SetInput(base.LogonIndex, base.HandleTableIndex, new ArraySegment<byte>[]
					{
						this.dataChunks[i]
					});
					yield return rop;
				}
			}
			yield break;
		}

		protected override void InternalExecute(IServerObject serverObject, IRopHandler ropHandler, ArraySegment<byte> outputBuffer)
		{
			this.result = ropHandler.FastTransferDestinationPutBufferExtended(serverObject, this.dataChunks.ToArray(), RopFastTransferDestinationPutBufferExtended.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			foreach (ArraySegment<byte> arraySegment in this.dataChunks)
			{
				stringBuilder.Append(" data=[");
				Util.AppendToString<byte>(stringBuilder, arraySegment);
				stringBuilder.Append("]");
			}
		}

		private const RopId RopType = RopId.FastTransferDestinationPutBufferExtended;

		private static FastTransferDestinationPutBufferExtendedResultFactory resultFactory = new FastTransferDestinationPutBufferExtendedResultFactory();

		private List<ArraySegment<byte>> dataChunks;
	}
}
