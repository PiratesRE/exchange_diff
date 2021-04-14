using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class FastTransferSourceGetBufferResultBase : RopResult
	{
		internal FastTransferState State
		{
			get
			{
				return this.ResultData.State;
			}
		}

		internal bool IsMoveUser
		{
			get
			{
				return this.ResultData.IsMoveUser;
			}
		}

		internal ArraySegment<byte> Data
		{
			get
			{
				return this.ResultData.Data;
			}
		}

		internal uint BackOffTime
		{
			get
			{
				return this.ResultData.BackOffTime;
			}
		}

		internal FastTransferSourceGetBufferResultBase(RopId ropId, ErrorCode errorCode, FastTransferSourceGetBufferData resultData) : base(ropId, errorCode, null)
		{
			this.ResultData = resultData;
		}

		internal FastTransferSourceGetBufferResultBase(Reader reader, bool isServerBusy, bool isExtendedRop) : base(reader)
		{
			this.ResultData = new FastTransferSourceGetBufferData(reader, isExtendedRop, isServerBusy);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			this.ResultData.Serialize(writer);
		}

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			this.ResultData.AppendToString(stringBuilder);
		}

		protected readonly FastTransferSourceGetBufferData ResultData;
	}
}
