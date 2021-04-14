using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Search
{
	internal class SearchRpcClient : RpcClientBase
	{
		public SearchRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		protected void RecordDocumentProcessing(Guid mdbGuid, Guid flowInstance, Guid correlationId, long docId)
		{
			base.ResetRetryCounter();
			int num = 0;
			try
			{
				do
				{
					try
					{
						_GUID guid = <Module>.ToGUID(ref correlationId);
						_GUID guid2 = <Module>.ToGUID(ref flowInstance);
						_GUID guid3 = <Module>.ToGUID(ref mdbGuid);
						<Module>.cli_RecordDocumentProcessing(base.BindingHandle, guid3, guid2, guid, docId);
					}
					catch when (endfilter(true))
					{
						num = Marshal.GetExceptionCode();
					}
					if (num >= 0)
					{
						goto IL_5B;
					}
				}
				while (base.RetryRpcCall(num, RpcRetryType.ServerBusy) != 0);
				<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "cli_RecordDocumentProcessing");
				IL_5B:;
			}
			finally
			{
			}
		}

		[HandleProcessCorruptedStateExceptions]
		protected unsafe void RecordDocumentFailure(Guid mdbGuid, Guid correlationId, long docId, string errorMessage)
		{
			base.ResetRetryCounter();
			int num = 0;
			using (SafeMarshalHGlobalHandle safeMarshalHGlobalHandle = new SafeMarshalHGlobalHandle(Marshal.StringToHGlobalUni(errorMessage)))
			{
				do
				{
					try
					{
						IntPtr intPtr = safeMarshalHGlobalHandle.DangerousGetHandle();
						_GUID guid = <Module>.ToGUID(ref correlationId);
						_GUID guid2 = <Module>.ToGUID(ref mdbGuid);
						<Module>.cli_RecordDocumentFailure(base.BindingHandle, guid2, guid, docId, (ushort*)intPtr.ToPointer());
					}
					catch when (endfilter(true))
					{
						num = Marshal.GetExceptionCode();
					}
					if (num >= 0)
					{
						goto IL_6D;
					}
				}
				while (base.RetryRpcCall(num, RpcRetryType.ServerBusy) != 0);
				<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "cli_RecordDocumentFailure");
				IL_6D:;
			}
		}

		[HandleProcessCorruptedStateExceptions]
		protected void UpdateIndexSystems()
		{
			base.ResetRetryCounter();
			int num = 0;
			try
			{
				do
				{
					try
					{
						<Module>.cli_UpdateIndexSystems(base.BindingHandle);
					}
					catch when (endfilter(true))
					{
						num = Marshal.GetExceptionCode();
					}
					if (num >= 0)
					{
						goto IL_3E;
					}
				}
				while (base.RetryRpcCall(num, RpcRetryType.ServerBusy) != 0);
				<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "cli_SuspendIndexing");
				IL_3E:;
			}
			finally
			{
			}
		}

		[HandleProcessCorruptedStateExceptions]
		protected void ResumeIndexing(Guid databaseGuid)
		{
			base.ResetRetryCounter();
			int num = 0;
			try
			{
				do
				{
					try
					{
						_GUID guid = <Module>.ToGUID(ref databaseGuid);
						<Module>.cli_ResumeIndexing(base.BindingHandle, guid);
					}
					catch when (endfilter(true))
					{
						num = Marshal.GetExceptionCode();
					}
					if (num >= 0)
					{
						goto IL_47;
					}
				}
				while (base.RetryRpcCall(num, RpcRetryType.ServerBusy) != 0);
				<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "cli_ResumeIndexing");
				IL_47:;
			}
			finally
			{
			}
		}

		[HandleProcessCorruptedStateExceptions]
		protected void RebuildIndexSystem(Guid databaseGuid)
		{
			base.ResetRetryCounter();
			int num = 0;
			try
			{
				do
				{
					try
					{
						_GUID guid = <Module>.ToGUID(ref databaseGuid);
						<Module>.cli_RebuildIndexSystem(base.BindingHandle, guid);
					}
					catch when (endfilter(true))
					{
						num = Marshal.GetExceptionCode();
					}
					if (num >= 0)
					{
						goto IL_47;
					}
				}
				while (base.RetryRpcCall(num, RpcRetryType.ServerBusy) != 0);
				<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(num, "cli_RebuildIndexSystem");
				IL_47:;
			}
			finally
			{
			}
		}
	}
}
