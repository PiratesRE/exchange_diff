using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class RopResult : Result
	{
		protected RopResult(RopId ropId, ErrorCode errorCode, IServerObject returnObject) : base(ropId)
		{
			this.errorCode = errorCode;
			this.returnObject = returnObject;
			this.returnObjectHandle = ServerObjectHandle.None;
			this.serverObjectHandleTableIndex = 0;
		}

		protected RopResult(Reader reader) : base(reader)
		{
			this.serverObjectHandleTableIndex = reader.ReadByte();
			this.errorCode = (ErrorCode)reader.ReadUInt32();
			this.returnObjectHandle = ServerObjectHandle.None;
		}

		public ErrorCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;
			}
			set
			{
				this.exception = value;
			}
		}

		internal IServerObject ReturnObject
		{
			get
			{
				return this.returnObject;
			}
			set
			{
				this.returnObject = value;
			}
		}

		internal byte HandleTableIndex
		{
			get
			{
				return this.serverObjectHandleTableIndex;
			}
		}

		internal static RopResult Parse(Reader reader, RopResult.ResultParserDelegate parseSuccessfulResult, RopResult.ResultParserDelegate parseFailedResult)
		{
			ErrorCode errorCode = (ErrorCode)reader.PeekUInt32(2L);
			ErrorCode errorCode2 = errorCode;
			if (errorCode2 == ErrorCode.None)
			{
				return parseSuccessfulResult(reader);
			}
			return parseFailedResult(reader);
		}

		internal static long CalculateResultSize(RopResult result)
		{
			long position;
			using (CountWriter countWriter = new CountWriter())
			{
				result.Serialize(countWriter);
				position = countWriter.Position;
			}
			return position;
		}

		internal DiagnosticContextResult GetDiagnosticContextResult(long maxSize)
		{
			if (this.diagnosticInfoProvider == null)
			{
				return null;
			}
			if (maxSize <= 20L)
			{
				return null;
			}
			uint threadId;
			uint requestId;
			DiagnosticContextFlags flags;
			byte[] data;
			this.diagnosticInfoProvider.GetDiagnosticData(maxSize - 20L, out threadId, out requestId, out flags, out data);
			return new DiagnosticContextResult(threadId, requestId, flags, data);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteByte(this.serverObjectHandleTableIndex);
			writer.WriteUInt32((uint)this.errorCode);
		}

		internal void SetServerObjectHandleIndex(byte tableIndex)
		{
			this.serverObjectHandleTableIndex = tableIndex;
		}

		internal ServerObjectHandle GetServerObjectHandle(ServerObjectMap map)
		{
			if (this.returnObject != null)
			{
				this.returnObjectHandle = map.Add(this.returnObject);
				this.returnObject = null;
			}
			return this.returnObjectHandle;
		}

		internal void SynchronizeForTest(RopResult otherResult)
		{
			this.returnObject = otherResult.returnObject;
			this.returnObjectHandle = otherResult.returnObjectHandle;
			base.String8Encoding = otherResult.String8Encoding;
		}

		public void SetDiagnosticInfoProvider(IDiagnosticInfoProvider diagnosticInfoProvider)
		{
			this.diagnosticInfoProvider = diagnosticInfoProvider;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.AppendToString(stringBuilder);
			return stringBuilder.ToString();
		}

		internal virtual void AppendToString(StringBuilder stringBuilder)
		{
			stringBuilder.Append(base.GetType()).Append("(").Append(base.RopId).Append(").");
			stringBuilder.Append(" Error code=").Append(this.ErrorCode);
			stringBuilder.Append("(0x").Append(((uint)this.ErrorCode).ToString("X8")).Append(")");
		}

		internal const int HeaderSize = 6;

		private readonly ErrorCode errorCode;

		private Exception exception;

		private IServerObject returnObject;

		private ServerObjectHandle returnObjectHandle;

		private byte serverObjectHandleTableIndex;

		private IDiagnosticInfoProvider diagnosticInfoProvider;

		internal delegate RopResult ResultParserDelegate(Reader reader);
	}
}
