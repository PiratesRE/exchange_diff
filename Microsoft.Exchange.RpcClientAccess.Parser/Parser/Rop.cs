using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class Rop
	{
		internal byte LogonIndex
		{
			get
			{
				return this.logonIndex;
			}
		}

		internal byte HandleTableIndex
		{
			get
			{
				return this.handleTableIndex;
			}
		}

		internal List<RopResult> ChainedResults
		{
			get
			{
				return this.chainedResults;
			}
			set
			{
				this.chainedResults = value;
			}
		}

		internal int InputBufferSize
		{
			get
			{
				return this.inputBufferSize;
			}
			set
			{
				this.inputBufferSize = value;
			}
		}

		public virtual void ParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
		{
			int num = (int)reader.Position;
			this.InternalParseInput(reader, serverObjectHandleTable, logonTracker);
			this.inputBufferSize = (int)reader.Position - num;
		}

		public void SerializeInput(Writer writer, Encoding string8Encoding)
		{
			this.InternalSerializeInput(writer, string8Encoding);
		}

		public void ParseOutput(Reader reader, Encoding string8Encoding)
		{
			this.InternalParseOutput(reader, string8Encoding);
		}

		public void SerializeOutput(Writer writer)
		{
			this.InternalSerializeOutput(writer);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append(this.RopId).Append(" [");
			this.AppendToString(stringBuilder);
			stringBuilder.Append("]");
			if (this.Result != null)
			{
				stringBuilder.Append(" Result [");
				this.Result.AppendToString(stringBuilder);
				stringBuilder.Append("]");
			}
			return stringBuilder.ToString();
		}

		internal static int ComputeResultHeaderSize(int specificRopHeaderSize)
		{
			return 6 + specificRopHeaderSize;
		}

		protected static byte ReadHandleTableIndex(Reader reader, ServerObjectHandleTable serverObjectHandleTable)
		{
			byte index = reader.ReadByte();
			serverObjectHandleTable.AccessIndex((int)index);
			return index;
		}

		protected static ushort ComputeRemainingBufferSize(int requestedByteCount, ushort specificRopHeaderSize, int availableOutputBufferSize, bool readMaximum)
		{
			int num = requestedByteCount;
			int num2 = availableOutputBufferSize - Rop.ComputeResultHeaderSize((int)specificRopHeaderSize);
			if (num2 < 0)
			{
				throw new BufferTooSmallException();
			}
			if (readMaximum)
			{
				if (num > num2)
				{
					num = num2;
				}
			}
			else if (num > num2)
			{
				throw new BufferTooSmallException();
			}
			if (num > 65535 || num < 0)
			{
				string message = string.Format("Invalid byte count (must fit in the range of a ushort): {0}", num);
				throw new InvalidOperationException(message);
			}
			return (ushort)num;
		}

		protected void SetCommonInput(byte logonIndex, byte handleTableIndex)
		{
			this.logonIndex = logonIndex;
			this.handleTableIndex = handleTableIndex;
		}

		internal abstract RopId RopId { get; }

		internal abstract void Execute(IConnectionInformation connection, IRopDriver ropDriver, ServerObjectHandleTable handleTable, ArraySegment<byte> outputBuffer);

		internal RopResult Result
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		protected abstract IResultFactory GetDefaultResultFactory(IConnectionInformation connection, ArraySegment<byte> outputBuffer);

		protected virtual void InternalParseInput(Reader reader, ServerObjectHandleTable serverObjectHandleTable, IParseLogonTracker logonTracker)
		{
			RopId ropId = (RopId)reader.ReadByte();
			if (ropId != this.RopId)
			{
				throw new BufferParseException(string.Format("RopId does not match.  Found {0} but expecting {1}", ropId, this.RopId));
			}
			this.logonIndex = reader.ReadByte();
			this.handleTableIndex = Rop.ReadHandleTableIndex(reader, serverObjectHandleTable);
		}

		protected virtual void InternalSerializeInput(Writer writer, Encoding string8Encoding)
		{
			writer.WriteByte((byte)this.RopId);
			writer.WriteByte(this.logonIndex);
			writer.WriteByte(this.handleTableIndex);
		}

		protected virtual void InternalParseOutput(Reader reader, Encoding string8Encoding)
		{
		}

		protected virtual void InternalSerializeOutput(Writer writer)
		{
		}

		internal virtual void AppendToString(StringBuilder stringBuilder)
		{
		}

		internal virtual byte[] CreateFakeRopRequest(RopResult result, ServerObjectHandleTable serverObjectHandleTable)
		{
			return null;
		}

		internal virtual void ResolveString8Values(Encoding string8Encoding)
		{
		}

		internal virtual void MergeChainedData(Rop rop)
		{
			throw new BufferParseException(string.Format("Not a chainable input rop: {0}", this.RopId));
		}

		internal virtual IEnumerator<Rop> SplitChainedData()
		{
			yield break;
		}

		internal virtual int? MinimumResponseSizeRequired
		{
			get
			{
				return null;
			}
		}

		private int inputBufferSize;

		private byte logonIndex;

		private byte handleTableIndex;

		private List<RopResult> chainedResults;

		protected RopResult result;

		internal delegate Rop CreateRopDelegate();
	}
}
