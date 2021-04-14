using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal class RopExecutionHistory
	{
		public RopExecutionHistory(uint historyLength)
		{
			if (historyLength > 0U)
			{
				this.ropExecutionHistory = new RopExecutionHistory.RopHistoryEntry[historyLength];
			}
			else
			{
				this.ropExecutionHistory = null;
			}
			this.currentRopExecutionHistorySlot = 0U;
		}

		public void OnBeforeRopExecution(Rop rop, ServerObjectHandleTable serverObjectHandleTable)
		{
			if (this.ropExecutionHistory == null)
			{
				return;
			}
			this.currentRopExecutionHistorySlot += 1U;
			if ((ulong)this.currentRopExecutionHistorySlot == (ulong)((long)this.ropExecutionHistory.Length))
			{
				this.currentRopExecutionHistorySlot = 0U;
			}
			InputRop inputRop = rop as InputRop;
			byte key;
			if (inputRop != null)
			{
				key = inputRop.InputHandleTableIndex;
			}
			else
			{
				key = rop.HandleTableIndex;
			}
			ServerObjectHandle inputObjectHandle = serverObjectHandleTable[(int)key];
			this.ropExecutionHistory[(int)((UIntPtr)this.currentRopExecutionHistorySlot)].RopType = rop.RopId;
			this.ropExecutionHistory[(int)((UIntPtr)this.currentRopExecutionHistorySlot)].InputObjectHandle = inputObjectHandle;
			this.ropExecutionHistory[(int)((UIntPtr)this.currentRopExecutionHistorySlot)].ErrorCode = (ErrorCode)4294967295U;
			this.ropExecutionHistory[(int)((UIntPtr)this.currentRopExecutionHistorySlot)].OutputObjectHandle = ServerObjectHandle.None;
		}

		public void OnAfterRopExecution(Rop rop, IRopDriver ropDriver, ServerObjectHandleTable serverObjectHandleTable)
		{
			if (this.ropExecutionHistory == null)
			{
				return;
			}
			if (rop.Result != null)
			{
				this.ropExecutionHistory[(int)((UIntPtr)this.currentRopExecutionHistorySlot)].ErrorCode = rop.Result.ErrorCode;
				ErrorCode errorCode = ErrorCode.None;
				ServerObjectMap map;
				if (ropDriver.TryGetServerObjectMap(rop.LogonIndex, out map, out errorCode))
				{
					this.ropExecutionHistory[(int)((UIntPtr)this.currentRopExecutionHistorySlot)].OutputObjectHandle = rop.Result.GetServerObjectHandle(map);
				}
			}
		}

		private readonly RopExecutionHistory.RopHistoryEntry[] ropExecutionHistory;

		private uint currentRopExecutionHistorySlot;

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct RopHistoryEntry
		{
			public RopId RopType;

			public ServerObjectHandle InputObjectHandle;

			public ServerObjectHandle OutputObjectHandle;

			public ErrorCode ErrorCode;
		}
	}
}
