using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class RopWriteStreamExtended : InputRop
	{
		internal override RopId RopId
		{
			get
			{
				return RopId.WriteStreamExtended;
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
			return new RopWriteStreamExtended();
		}

		internal void SetInput(byte logonIndex, byte handleTableIndex, ArraySegment<byte>[] dataChunks)
		{
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
			this.result = WriteStreamExtendedResult.Parse(reader);
		}

		protected override IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer)
		{
			return RopWriteStreamExtended.resultFactory;
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
			RopWriteStreamExtended ropWriteStreamExtended = rop as RopWriteStreamExtended;
			if (ropWriteStreamExtended == null)
			{
				throw new BufferParseException("Chained Rop not same type");
			}
			if (ropWriteStreamExtended.LogonIndex != base.LogonIndex)
			{
				throw new BufferParseException("Chained Rop not same logon index");
			}
			if (ropWriteStreamExtended.HandleTableIndex != base.HandleTableIndex)
			{
				throw new BufferParseException("Chained Rop not same SOHT index");
			}
			foreach (ArraySegment<byte> item in ropWriteStreamExtended.DataChunks)
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
					RopWriteStreamExtended rop = new RopWriteStreamExtended();
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
			if ((long)outputBuffer.Count < RopWriteStreamExtended.resultFactory.SuccessfulResultMinimalSize)
			{
				throw new BufferTooSmallException();
			}
			this.result = ropHandler.WriteStreamExtended(serverObject, this.dataChunks.ToArray(), RopWriteStreamExtended.resultFactory);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			foreach (ArraySegment<byte> arraySegment in this.dataChunks)
			{
				stringBuilder.Append(" Size=").Append(arraySegment.Count);
			}
		}

		private const RopId RopType = RopId.WriteStreamExtended;

		private static WriteStreamExtendedResultFactory resultFactory = new WriteStreamExtendedResultFactory();

		private List<ArraySegment<byte>> dataChunks;
	}
}
