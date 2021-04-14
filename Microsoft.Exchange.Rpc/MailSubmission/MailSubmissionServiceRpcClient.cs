using System;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics.Components.StoreDriver;

namespace Microsoft.Exchange.Rpc.MailSubmission
{
	internal class MailSubmissionServiceRpcClient : RpcClientBase
	{
		public MailSubmissionServiceRpcClient(string machineName, NetworkCredential nc) : base(machineName, nc)
		{
		}

		public MailSubmissionServiceRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] SubmitMessage(byte[] inBytes)
		{
			byte[] result = null;
			IntPtr hglobal = IntPtr.Zero;
			byte* ptr = null;
			int cBytes = 0;
			try
			{
				int num = 0;
				hglobal = <Module>.MToUBytes(inBytes, &num);
				bool flag = true;
				RpcRetryState rpcRetryState = 0;
				*(ref rpcRetryState + 4) = 0;
				do
				{
					try
					{
						<Module>.cli_SubmitMessage3(base.BindingHandle, num, (byte*)hglobal.ToPointer(), &cBytes, &ptr);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "SubmitMessage");
						}
					}
				}
				while (flag);
				result = <Module>.UToMBytes(cBytes, ptr);
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe SubmissionStatus SubmitMessage(string messageClass, string serverDN, string serverSADN, byte[] mailboxGuid, byte[] entryId, byte[] parentEntryId, byte[] mdbGuid, int eventCounter, byte[] ipAddress)
		{
			IntPtr hglobal = IntPtr.Zero;
			IntPtr hglobal2 = IntPtr.Zero;
			IntPtr hglobal3 = IntPtr.Zero;
			IntPtr hglobal4 = IntPtr.Zero;
			IntPtr hglobal5 = IntPtr.Zero;
			IntPtr hglobal6 = IntPtr.Zero;
			IntPtr hglobal7 = IntPtr.Zero;
			IntPtr hglobal8 = IntPtr.Zero;
			tagSubmissionStatus result;
			try
			{
				hglobal = Marshal.StringToHGlobalUni(messageClass);
				ushort* ptr = (ushort*)hglobal.ToPointer();
				hglobal2 = Marshal.StringToHGlobalUni(serverDN);
				ushort* ptr2 = (ushort*)hglobal2.ToPointer();
				hglobal3 = Marshal.StringToHGlobalUni(serverSADN);
				ushort* ptr3 = (ushort*)hglobal3.ToPointer();
				int num = 0;
				hglobal4 = <Module>.MToUBytes(mailboxGuid, &num);
				int num2 = 0;
				hglobal5 = <Module>.MToUBytes(entryId, &num2);
				int num3 = 0;
				hglobal6 = <Module>.MToUBytes(parentEntryId, &num3);
				int num4 = 0;
				hglobal7 = <Module>.MToUBytes(mdbGuid, &num4);
				int num5 = 0;
				hglobal8 = <Module>.MToUBytes(ipAddress, &num5);
				bool flag = true;
				RpcRetryState rpcRetryState = 0;
				*(ref rpcRetryState + 4) = 0;
				do
				{
					try
					{
						result = (tagSubmissionStatus)<Module>.cli_SubmitMessage(base.BindingHandle, ptr, ptr2, ptr3, num, (byte*)hglobal4.ToPointer(), num2, (byte*)hglobal5.ToPointer(), num3, (byte*)hglobal6.ToPointer(), num4, (byte*)hglobal7.ToPointer(), eventCounter, num5, (byte*)hglobal8.ToPointer());
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "SubmitMessage");
						}
					}
				}
				while (flag);
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
				Marshal.FreeHGlobal(hglobal2);
				Marshal.FreeHGlobal(hglobal3);
				Marshal.FreeHGlobal(hglobal4);
				Marshal.FreeHGlobal(hglobal5);
				Marshal.FreeHGlobal(hglobal6);
				Marshal.FreeHGlobal(hglobal7);
				Marshal.FreeHGlobal(hglobal8);
			}
			return (SubmissionStatus)result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe SubmissionStatus SubmitDumpsterMessages(string storageGroupDN, long startTime, long endTime)
		{
			IntPtr hglobal = IntPtr.Zero;
			tagSubmissionStatus result;
			try
			{
				hglobal = Marshal.StringToHGlobalUni(storageGroupDN);
				ushort* ptr = (ushort*)hglobal.ToPointer();
				bool flag = true;
				RpcRetryState rpcRetryState = 0;
				*(ref rpcRetryState + 4) = 0;
				do
				{
					try
					{
						result = (tagSubmissionStatus)<Module>.cli_SubmitDumpsterMessages(base.BindingHandle, ptr, startTime, endTime);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "SubmitDumpsterMessages");
						}
					}
				}
				while (flag);
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
			}
			return (SubmissionStatus)result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe AddResubmitRequestStatus AddMdbResubmitRequest(Guid requestGuid, Guid MdbGuid, long startTime, long endTime, string[] unresponsivePrimaryServers, byte[] reservedBytes)
		{
			ushort** ptr = null;
			int num = 0;
			IntPtr hglobal = IntPtr.Zero;
			bool flag = true;
			RpcRetryState rpcRetryState = 0;
			*(ref rpcRetryState + 4) = 0;
			tagAddResubmitRequestStatus result;
			try
			{
				if (unresponsivePrimaryServers != null)
				{
					num = unresponsivePrimaryServers.Length;
					ptr = (ushort**)Marshal.AllocHGlobal((int)((long)num * 8L)).ToPointer();
					for (int i = 0; i < num; i++)
					{
						IntPtr intPtr = Marshal.StringToHGlobalUni(unresponsivePrimaryServers[i]);
						*(long*)((long)i * 8L + ptr / 8) = intPtr.ToPointer();
					}
				}
				else
				{
					ptr = (ushort**)Marshal.AllocHGlobal(0).ToPointer();
				}
				int num2 = 0;
				hglobal = <Module>.MToUBytes(reservedBytes, &num2);
				do
				{
					try
					{
						_GUID guid = <Module>.ToGUID(ref MdbGuid);
						_GUID guid2 = <Module>.ToGUID(ref requestGuid);
						result = (tagAddResubmitRequestStatus)<Module>.cli_AddMdbResubmitRequest(base.BindingHandle, guid2, guid, startTime, endTime, num, ptr, num2, (byte*)hglobal.ToPointer());
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "AddMdbResubmitRequest");
						}
					}
				}
				while (flag);
			}
			finally
			{
				if (ptr != null)
				{
					for (int j = 0; j < num; j++)
					{
						IntPtr hglobal2 = new IntPtr(*(long*)((long)j * 8L + ptr / 8));
						Marshal.FreeHGlobal(hglobal2);
					}
					IntPtr hglobal3 = new IntPtr((void*)ptr);
					Marshal.FreeHGlobal(hglobal3);
				}
				Marshal.FreeHGlobal(hglobal);
			}
			return (AddResubmitRequestStatus)result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe MailSubmissionResult SubmitMessage2(string serverDN, Guid mailboxGuid, Guid mdbGuid, int eventCounter, byte[] entryId, byte[] parentEntryId, string serverFQDN, byte[] ipAddress)
		{
			IntPtr hglobal = IntPtr.Zero;
			IntPtr hglobal2 = IntPtr.Zero;
			IntPtr hglobal3 = IntPtr.Zero;
			IntPtr hglobal4 = IntPtr.Zero;
			IntPtr hglobal5 = IntPtr.Zero;
			_Mail_Info pStr = 0L;
			*(ref pStr + 24) = 0L;
			*(ref pStr + 8) = 0L;
			*(ref pStr + 16) = 0L;
			MailSubmissionResult mailSubmissionResult = new MailSubmissionResult();
			MailSubmissionResult result;
			try
			{
				hglobal = Marshal.StringToHGlobalUni(serverDN);
				ushort* ptr = (ushort*)hglobal.ToPointer();
				hglobal4 = Marshal.StringToHGlobalUni(serverFQDN);
				ushort* ptr2 = (ushort*)hglobal4.ToPointer();
				int num = 0;
				hglobal2 = <Module>.MToUBytes(entryId, &num);
				int num2 = 0;
				hglobal3 = <Module>.MToUBytes(parentEntryId, &num2);
				int num3 = 0;
				hglobal5 = <Module>.MToUBytes(ipAddress, &num3);
				int errorCode = 0;
				bool flag = true;
				RpcRetryState rpcRetryState = 0;
				*(ref rpcRetryState + 4) = 0;
				do
				{
					try
					{
						_GUID guid = <Module>.ToGUID(ref mdbGuid);
						_GUID guid2 = <Module>.ToGUID(ref mailboxGuid);
						errorCode = <Module>.cli_SubmitMessage2(base.BindingHandle, ptr, guid2, guid, eventCounter, num, (byte*)hglobal2.ToPointer(), num2, (byte*)hglobal3.ToPointer(), ptr2, num3, (byte*)hglobal5.ToPointer(), &pStr);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "SubmitMessage");
						}
					}
				}
				while (flag);
				mailSubmissionResult.ErrorCode = errorCode;
				mailSubmissionResult.Sender = <Module>.UToMString(pStr);
				mailSubmissionResult.MessageId = <Module>.UToMString(*(ref pStr + 8));
				mailSubmissionResult.Subject = <Module>.UToMString(*(ref pStr + 16));
				mailSubmissionResult.DiagnosticInfo = <Module>.UToMString(*(ref pStr + 24));
				result = mailSubmissionResult;
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
				Marshal.FreeHGlobal(hglobal2);
				Marshal.FreeHGlobal(hglobal3);
				Marshal.FreeHGlobal(hglobal5);
				Marshal.FreeHGlobal(hglobal4);
				<Module>.FreeString(*(ref pStr + 24));
				<Module>.FreeString(pStr);
				<Module>.FreeString(*(ref pStr + 16));
				<Module>.FreeString(*(ref pStr + 8));
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe int QueryDumpsterStats(string storageGroupDn, ref long ticks, ref long queueSize, ref int numberOfItem)
		{
			IntPtr intPtr = IntPtr.Zero;
			long num = 0L;
			long num2 = 0L;
			int num3 = 0;
			int num4 = 0;
			try
			{
				intPtr = Marshal.StringToHGlobalUni(storageGroupDn);
				ushort* ptr = (ushort*)intPtr.ToPointer();
				bool flag = false;
				RpcRetryState rpcRetryState = 0;
				*(ref rpcRetryState + 4) = 0;
				do
				{
					try
					{
						num4 = <Module>.cli_QueryDumpsterStats(base.BindingHandle, ptr, &num, &num2, &num3);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "QueryDumpsterStats");
						}
					}
				}
				while (flag);
				if (num4 < 0)
				{
					ExTraceGlobals.MailSubmissionServiceTracer.TraceError<IntPtr, int>(0L, "QueryDumpsterStats failed for storage group DN {0} with error code {1}.", intPtr, num4);
				}
				else
				{
					ticks = num;
					queueSize = num2;
					numberOfItem = num3;
				}
			}
			finally
			{
				Marshal.FreeHGlobal(intPtr);
			}
			return num4;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] ShadowHeartBeat(byte[] inBytes)
		{
			byte[] result = null;
			IntPtr hglobal = IntPtr.Zero;
			byte* ptr = null;
			int cBytes = 0;
			try
			{
				int num = 0;
				hglobal = <Module>.MToUBytes(inBytes, &num);
				bool flag = true;
				RpcRetryState rpcRetryState = 0;
				*(ref rpcRetryState + 4) = 0;
				do
				{
					try
					{
						<Module>.cli_ShadowHeartBeat(base.BindingHandle, num, (byte*)hglobal.ToPointer(), &cBytes, &ptr);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "ShadowHeartBeat");
						}
					}
				}
				while (flag);
				result = <Module>.UToMBytes(cBytes, ptr);
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] GetResubmitRequest(byte[] inBytes)
		{
			byte[] result = null;
			IntPtr hglobal = IntPtr.Zero;
			byte* ptr = null;
			int cBytes = 0;
			try
			{
				bool flag = false;
				int num = 0;
				hglobal = <Module>.MToUBytes(inBytes, &num);
				RpcRetryState rpcRetryState = 0;
				*(ref rpcRetryState + 4) = 0;
				do
				{
					try
					{
						<Module>.cli_GetResubmitRequest(base.BindingHandle, num, (byte*)hglobal.ToPointer(), &cBytes, &ptr);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "GetResubmitRequest");
						}
					}
				}
				while (flag);
				result = <Module>.UToMBytes(cBytes, ptr);
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] SetResubmitRequest(byte[] inBytes)
		{
			byte[] result = null;
			IntPtr hglobal = IntPtr.Zero;
			byte* ptr = null;
			int cBytes = 0;
			try
			{
				bool flag = false;
				int num = 0;
				hglobal = <Module>.MToUBytes(inBytes, &num);
				RpcRetryState rpcRetryState = 0;
				*(ref rpcRetryState + 4) = 0;
				do
				{
					try
					{
						<Module>.cli_SetResubmitRequest(base.BindingHandle, num, (byte*)hglobal.ToPointer(), &cBytes, &ptr);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(Marshal.GetExceptionCode(), "SetResubmitRequest");
						}
					}
				}
				while (flag);
				result = <Module>.UToMBytes(cBytes, ptr);
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] RemoveResubmitRequest(byte[] inBytes)
		{
			byte[] result = null;
			IntPtr hglobal = IntPtr.Zero;
			byte* ptr = null;
			int cBytes = 0;
			try
			{
				bool flag = false;
				int num = 0;
				hglobal = <Module>.MToUBytes(inBytes, &num);
				RpcRetryState rpcRetryState = 0;
				*(ref rpcRetryState + 4) = 0;
				do
				{
					try
					{
						<Module>.cli_RemoveResubmitRequest(base.BindingHandle, num, (byte*)hglobal.ToPointer(), &cBytes, &ptr);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(Marshal.GetExceptionCode(), "RemoveResubmitRequest");
						}
					}
				}
				while (flag);
				result = <Module>.UToMBytes(cBytes, ptr);
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe AddResubmitRequestStatus AddConditionalResubmitRequest(Guid requestGuid, long startTime, long endTime, string conditions, string[] unresponsivePrimaryServers, byte[] reservedBytes)
		{
			ushort** ptr = null;
			int num = 0;
			IntPtr hglobal = IntPtr.Zero;
			IntPtr hglobal2 = IntPtr.Zero;
			bool flag = true;
			RpcRetryState rpcRetryState = 0;
			*(ref rpcRetryState + 4) = 0;
			tagAddResubmitRequestStatus result;
			try
			{
				if (unresponsivePrimaryServers != null)
				{
					num = unresponsivePrimaryServers.Length;
					ptr = (ushort**)Marshal.AllocHGlobal((int)((long)num * 8L)).ToPointer();
					for (int i = 0; i < num; i++)
					{
						IntPtr intPtr = Marshal.StringToHGlobalUni(unresponsivePrimaryServers[i]);
						*(long*)((long)i * 8L + ptr / 8) = intPtr.ToPointer();
					}
				}
				else
				{
					ptr = (ushort**)Marshal.AllocHGlobal(0).ToPointer();
				}
				int num2 = 0;
				hglobal = <Module>.MToUBytes(reservedBytes, &num2);
				hglobal2 = Marshal.StringToHGlobalUni(conditions);
				ushort* ptr2 = (ushort*)hglobal2.ToPointer();
				do
				{
					try
					{
						_GUID guid = <Module>.ToGUID(ref requestGuid);
						result = (tagAddResubmitRequestStatus)<Module>.cli_AddConditionalResubmitRequest(base.BindingHandle, guid, startTime, endTime, ptr2, num, ptr, num2, (byte*)hglobal.ToPointer());
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (<Module>.RpcRetryState.Retry(ref rpcRetryState, exceptionCode) == null)
						{
							<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "AddConditionalResubmitRequest");
						}
					}
				}
				while (flag);
			}
			finally
			{
				if (ptr != null)
				{
					for (int j = 0; j < num; j++)
					{
						IntPtr hglobal3 = new IntPtr(*(long*)((long)j * 8L + ptr / 8));
						Marshal.FreeHGlobal(hglobal3);
					}
					IntPtr hglobal4 = new IntPtr((void*)ptr);
					Marshal.FreeHGlobal(hglobal4);
				}
				Marshal.FreeHGlobal(hglobal);
				Marshal.FreeHGlobal(hglobal2);
			}
			return (AddResubmitRequestStatus)result;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static MailSubmissionServiceRpcClient()
		{
			MailSubmissionServiceRpcClient.ServerUnavailable = 1722;
			MailSubmissionServiceRpcClient.RpcCallFailedDidNotExecute = 1727;
			MailSubmissionServiceRpcClient.RpcServerTooBusy = 1723;
		}

		public static int RpcServerTooBusy = 1723;

		public static int RpcCallFailedDidNotExecute = 1727;

		public static int ServerUnavailable = 1722;

		public static int EndpointNotRegistered = 1753;
	}
}
