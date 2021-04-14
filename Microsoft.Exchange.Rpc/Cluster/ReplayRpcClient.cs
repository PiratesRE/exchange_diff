using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal class ReplayRpcClient : RpcClientBase
	{
		public ReplayRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RequestSuspend(Guid guid, string suspendComment)
		{
			int num = 0;
			ushort* ptr = null;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "Entering ReplayRpcClient::RequestSuspend() for guid: {0}, suspendComment: {1}", guid, suspendComment);
			try
			{
				ptr = <Module>.StringToUnmanaged(suspendComment);
				_GUID guid2 = <Module>.ToGUID(ref guid);
				num = <Module>.cli_RequestSuspend(base.BindingHandle, guid2, ptr, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, int>((long)this.GetHashCode(), "ReplayRpcClient::RequestSuspend() for guid: {0}, FAILED with HR=0x{1:x}", guid, num);
					ReplayRpcException.ThrowRpcException(num, "cli_RequestSuspend");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RequestSuspend");
			}
			finally
			{
				<Module>.FreeString(ptr);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RequestResume(Guid guid)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Entering ReplayRpcClient::RequestResume() for guid: {0}", guid);
			try
			{
				_GUID guid2 = <Module>.ToGUID(ref guid);
				num = <Module>.cli_RequestResume(base.BindingHandle, guid2, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, int>((long)this.GetHashCode(), "ReplayRpcClient::RequestResume() for guid: {0}, FAILED with HR=0x{1:x}", guid, num);
					ReplayRpcException.ThrowRpcException(num, "cli_RequestResume");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RequestResume");
			}
			finally
			{
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo GetCopyStatusEx2(RpcGetDatabaseCopyStatusFlags collectionFlags, Guid[] sgGuids, ref RpcDatabaseCopyStatus[] sgStatuses)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			_GUID* ptr = null;
			byte* ptr2 = null;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			sgStatuses = null;
			try
			{
				ptr = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.FromGuidArray(sgGuids, &num2);
				num = <Module>.cli_GetCopyStatusEx2(base.BindingHandle, (uint)collectionFlags, num2, ptr, &num3, &ptr2, &num4, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_GetCopyStatusEx2");
				}
				ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<int, int>((long)this.GetHashCode(), "ReplayRpcClient::GetCopyStatusEx2() returned {0} status objects for a total of {1} bytes.", num4, num3);
				if (num3 > 0 && num4 > 0 && ptr2 != null)
				{
					RpcDatabaseCopyStatus[] array = SerializationServices.Deserialize<RpcDatabaseCopyStatus[]>(ptr2, num3);
					sgStatuses = array;
					if (array.Length != num4)
					{
						num = -2147024883;
						ReplayRpcException.ThrowRpcException(-2147024883, "cli_GetCopyStatusEx2");
					}
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_GetCopyStatusEx2");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccGetCopyStatusEx4(RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids, ref RpcDatabaseCopyStatus2[] dbStatuses)
		{
			Exception ex = null;
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			_GUID* ptr = null;
			byte* ptr2 = null;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			dbStatuses = null;
			ex = null;
			try
			{
				ptr = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.FromGuidArray(dbGuids, &num2);
				num = <Module>.cli_RpcsGetCopyStatusEx4(base.BindingHandle, (uint)collectionFlags2, num2, ptr, &num3, &ptr2, &num4, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "RpccGetCopyStatusEx4");
				}
				ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<int, int>((long)this.GetHashCode(), "ReplayRpcClient::RpccGetCopyStatusEx4() returned {0} status objects for a total of {1} bytes.", num4, num3);
				if (num3 > 0 && num4 > 0 && ptr2 != null)
				{
					if (!SerializationServices.TryDeserialize<RpcDatabaseCopyStatus2[]>(ptr2, num3, ref dbStatuses, ref ex))
					{
						num = -2147024883;
						ReplayRpcException.ThrowRpcException(-2147024883, "RpccGetCopyStatusEx4");
					}
					if (dbStatuses.Length != num4)
					{
						num = -2147024883;
						ReplayRpcException.ThrowRpcException(-2147024883, "RpccGetCopyStatusEx4");
					}
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				if (ex != null)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceError<Exception>((long)this.GetHashCode(), "ReplayRpcClient::RpccGetCopyStatusEx4 Deserialize failed: {0}", ex);
					throw new RpcException(string.Format("Deserialization failed: {0}", ex.Message), 13, ex);
				}
				ReplayRpcException.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "RpccGetCopyStatusEx4");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccGetCopyStatusBasic(RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids, ref RpcDatabaseCopyStatusBasic[] dbStatuses)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			_GUID* ptr = null;
			byte* ptr2 = null;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			dbStatuses = null;
			try
			{
				ptr = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.FromGuidArray(dbGuids, &num2);
				num = <Module>.cli_RpcsGetCopyStatusBasic(base.BindingHandle, (uint)collectionFlags2, num2, ptr, &num3, &ptr2, &num4, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_RpcsGetCopyStatusBasic");
				}
				ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<int, int>((long)this.GetHashCode(), "ReplayRpcClient::RpccGetCopyStatusBasic() returned {0} status objects for a total of {1} bytes.", num4, num3);
				if (num3 > 0 && num4 > 0 && ptr2 != null)
				{
					RpcDatabaseCopyStatusBasic[] array = SerializationServices.Deserialize<RpcDatabaseCopyStatusBasic[]>(ptr2, num3);
					dbStatuses = array;
					if (array.Length != num4)
					{
						num = -2147024883;
						ReplayRpcException.ThrowRpcException(-2147024883, "cli_RpcsGetCopyStatusBasic");
					}
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpcsGetCopyStatusBasic");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RunConfigurationUpdater()
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			try
			{
				num = <Module>.cli_RunConfigurationUpdater(base.BindingHandle, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_RunConfigurationUpdater");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RunConfigurationUpdater");
			}
			finally
			{
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccPrepareDatabaseSeedAndBegin(RpcSeederArgs seederArgs, ref RpcSeederStatus seederStatus)
		{
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			tagSeederStatus2 tagSeederStatus;
			<Module>.InitSeederStatus(&tagSeederStatus);
			tagSeederArgs3 tagSeederArgs;
			int num = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.MToUSeederArgs(seederArgs, &tagSeederArgs);
			if (num < 0)
			{
				ReplayRpcException.ThrowRpcException(num, "RpccPrepareDatabaseSeedAndBegin");
				return null;
			}
			try
			{
				num = <Module>.cli_RpcsClusterPrepareDatabaseSeedAndBegin3(base.BindingHandle, &tagSeederArgs, &tagErrorExceptionInfo, &tagSeederStatus);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_RpcsClusterPrepareDatabaseSeedAndBegin3");
				}
				seederStatus = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.UToMSeederStatus(ref tagSeederStatus);
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpcsClusterPrepareDatabaseSeedAndBegin3");
			}
			finally
			{
				<Module>.FreeSeederStatus(&tagSeederStatus);
				<Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.FreeSeederArgs(&tagSeederArgs);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccPrepareDatabaseSeedAndBegin4(RpcSeederArgs seederArgs, ref RpcSeederStatus seederStatus)
		{
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			tagSeederStatus2 tagSeederStatus;
			<Module>.InitSeederStatus(&tagSeederStatus);
			tagSeederArgs4 tagSeederArgs;
			int num = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.MToUSeederArgs(seederArgs, &tagSeederArgs);
			if (num < 0)
			{
				ReplayRpcException.ThrowRpcException(num, "RpccPrepareDatabaseSeedAndBegin4");
				return null;
			}
			try
			{
				num = <Module>.cli_RpcsClusterPrepareDatabaseSeedAndBegin4(base.BindingHandle, &tagSeederArgs, &tagErrorExceptionInfo, &tagSeederStatus);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_RpcsClusterPrepareDatabaseSeedAndBegin4");
				}
				seederStatus = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.UToMSeederStatus(ref tagSeederStatus);
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpcsClusterPrepareDatabaseSeedAndBegin4");
			}
			finally
			{
				<Module>.FreeSeederStatus(&tagSeederStatus);
				<Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.FreeSeederArgs((tagSeederArgs3*)(&tagSeederArgs));
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccPrepareDatabaseSeedAndBegin5(RpcSeederArgs seederArgs, ref RpcSeederStatus seederStatus)
		{
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			tagSeederStatus2 tagSeederStatus;
			<Module>.InitSeederStatus(&tagSeederStatus);
			tagSeederArgs5 tagSeederArgs;
			int num = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.MToUSeederArgs(seederArgs, &tagSeederArgs);
			if (num < 0)
			{
				ReplayRpcException.ThrowRpcException(num, "RpccPrepareDatabaseSeedAndBegin5");
				return null;
			}
			try
			{
				num = <Module>.cli_RpcsClusterPrepareDatabaseSeedAndBegin5(base.BindingHandle, &tagSeederArgs, &tagErrorExceptionInfo, &tagSeederStatus);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_RpcsClusterPrepareDatabaseSeedAndBegin5");
				}
				seederStatus = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.UToMSeederStatus(ref tagSeederStatus);
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpcsClusterPrepareDatabaseSeedAndBegin5");
			}
			finally
			{
				<Module>.FreeSeederStatus(&tagSeederStatus);
				<Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.FreeSeederArgs((tagSeederArgs3*)(&tagSeederArgs));
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo CancelDbSeed(Guid guid)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			try
			{
				_GUID guid2 = <Module>.ToGUID(ref guid);
				num = <Module>.cli_CancelDbSeed(base.BindingHandle, guid2, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_CancelDbSeed");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_CancelDbSeed");
			}
			finally
			{
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo EndDbSeed(Guid guid)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			try
			{
				_GUID guid2 = <Module>.ToGUID(ref guid);
				num = <Module>.cli_EndDbSeed(base.BindingHandle, guid2, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_EndDbSeed");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_EndDbSeed");
			}
			finally
			{
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccGetDatabaseSeedStatus(Guid dbGuid, ref RpcSeederStatus pSeederStatus)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			tagSeederStatus2 tagSeederStatus;
			<Module>.InitSeederStatus(&tagSeederStatus);
			string routineName = "cli_RpccGetDbSeedStatus";
			try
			{
				_GUID guid = <Module>.ToGUID(ref dbGuid);
				num = <Module>.cli_RpcsClusterGetDatabaseSeedStatus(base.BindingHandle, guid, &tagSeederStatus, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, routineName);
				}
				pSeederStatus = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.UToMSeederStatus(ref tagSeederStatus);
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, routineName);
			}
			finally
			{
				<Module>.FreeSeederStatus(&tagSeederStatus);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo GetDagNetworkConfig(ref byte[] nets)
		{
			int num = 0;
			nets = null;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			byte* ptr = null;
			int num2 = 0;
			try
			{
				num = <Module>.cli_GetDagNetworkConfig(base.BindingHandle, &num2, &ptr, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_GetDagNetworkConfig");
				}
				if (num2 > 0 && ptr != null)
				{
					nets = <Module>.MakeManagedBytes(ptr, num2);
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_GetDagNetworkConfig");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo SetDagNetwork(byte[] networkChange)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			byte* ptr = null;
			int num2 = 0;
			try
			{
				num = <Module>.GetUnmanagedBytes(networkChange, &ptr, &num2);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "GetUnmanagedBytes");
				}
				num = <Module>.cli_SetDagNetwork(base.BindingHandle, num2, ptr, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_SetDagNetwork");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_SetDagNetwork");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo SetDagNetworkConfig(byte[] networkChange)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			byte* ptr = null;
			int num2 = 0;
			try
			{
				num = <Module>.GetUnmanagedBytes(networkChange, &ptr, &num2);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "GetUnmanagedBytes");
				}
				num = <Module>.cli_SetDagNetworkConfig(base.BindingHandle, num2, ptr, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_SetDagNetworkConfig");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_SetDagNetworkConfig");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RemoveDagNetwork(byte[] deleteRequest)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			byte* ptr = null;
			int num2 = 0;
			try
			{
				num = <Module>.GetUnmanagedBytes(deleteRequest, &ptr, &num2);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "GetUnmanagedBytes");
				}
				num = <Module>.cli_RemoveDagNetwork(base.BindingHandle, num2, ptr, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_RemoveDagNetwork");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RemoveDagNetwork");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RequestSuspend2(Guid guid, string suspendComment, uint flags)
		{
			int num = 0;
			ushort* ptr = null;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, string, uint>((long)this.GetHashCode(), "Entering ReplayRpcClient::RequestSuspend2() for guid: {0}, suspendComment: {1} (flags={2})", guid, suspendComment, flags);
			try
			{
				ptr = <Module>.StringToUnmanaged(suspendComment);
				_GUID guid2 = <Module>.ToGUID(ref guid);
				num = <Module>.cli_RequestSuspend2(base.BindingHandle, guid2, ptr, (int)flags, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, int>((long)this.GetHashCode(), "ReplayRpcClient::RequestSuspend2() for guid: {0}, FAILED with HR=0x{1:x}", guid, num);
					ReplayRpcException.ThrowRpcException(num, "cli_RequestSuspend2");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RequestSuspend2");
			}
			finally
			{
				<Module>.FreeString(ptr);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RequestSuspend3(Guid guid, string suspendComment, uint flags, uint actionInitiator)
		{
			int num = 0;
			ushort* ptr = null;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			object[] args = new object[]
			{
				guid,
				suspendComment,
				flags,
				actionInitiator
			};
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "Entering ReplayRpcClient::RequestSuspend3() for guid: {0}, suspendComment: {1} (flags={2}, actionInitiator={3})", args);
			try
			{
				ptr = <Module>.StringToUnmanaged(suspendComment);
				_GUID guid2 = <Module>.ToGUID(ref guid);
				num = <Module>.cli_RequestSuspend3(base.BindingHandle, guid2, ptr, (int)flags, (int)actionInitiator, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, int>((long)this.GetHashCode(), "ReplayRpcClient::RequestSuspend3() for guid: {0}, FAILED with HR=0x{1:x}", guid, num);
					ReplayRpcException.ThrowRpcException(num, "cli_RequestSuspend3");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RequestSuspend3");
			}
			finally
			{
				<Module>.FreeString(ptr);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RequestResume2(Guid guid, uint flags)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, uint>((long)this.GetHashCode(), "Entering ReplayRpcClient::RequestResume2() for guid: {0} (flags={1})", guid, flags);
			try
			{
				_GUID guid2 = <Module>.ToGUID(ref guid);
				num = <Module>.cli_RequestResume2(base.BindingHandle, guid2, (int)flags, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, int>((long)this.GetHashCode(), "ReplayRpcClient::RequestResume2() for guid: {0}, FAILED with HR=0x{1:x}", guid, num);
					ReplayRpcException.ThrowRpcException(num, "cli_RequestResume2");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RequestResume2");
			}
			finally
			{
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccDisableReplayLag(Guid dbGuid, string disableReason)
		{
			int num = 0;
			ushort* ptr = null;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "Entering ReplayRpcClient::RpccDisableReplayLag() for guid: {0}, disableREason: {1}", dbGuid, disableReason);
			try
			{
				ptr = <Module>.StringToUnmanaged(disableReason);
				_GUID guid = <Module>.ToGUID(ref dbGuid);
				num = <Module>.cli_RpcsDisableReplayLag(base.BindingHandle, guid, ptr, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceError<Guid, int>((long)this.GetHashCode(), "ReplayRpcClient::RpccDisableReplayLag() for guid: {0}, FAILED with HR=0x{1:x}", dbGuid, num);
					ReplayRpcException.ThrowRpcException(num, "cli_RpcsDisableReplayLag");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpcsDisableReplayLag");
			}
			finally
			{
				<Module>.FreeString(ptr);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccEnableReplayLag(Guid dbGuid)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid>((long)this.GetHashCode(), "Entering ReplayRpcClient::RpccEnableReplayLag() for guid: {0}", dbGuid);
			try
			{
				_GUID guid = <Module>.ToGUID(ref dbGuid);
				num = <Module>.cli_RpcsEnableReplayLag(base.BindingHandle, guid, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceError<Guid, int>((long)this.GetHashCode(), "ReplayRpcClient::RpccEnableReplayLag() for guid: {0}, FAILED with HR=0x{1:x}", dbGuid, num);
					ReplayRpcException.ThrowRpcException(num, "cli_RpcsEnableReplayLag");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpcsEnableReplayLag");
			}
			finally
			{
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccDisableReplayLag2(Guid dbGuid, string disableReason, uint actionInitiator)
		{
			int num = 0;
			ushort* ptr = null;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, string, uint>((long)this.GetHashCode(), "Entering ReplayRpcClient::RpccDisableReplayLag2() for guid: {0}, disableREason: {1}, actionInitiator: {2}", dbGuid, disableReason, actionInitiator);
			try
			{
				ptr = <Module>.StringToUnmanaged(disableReason);
				_GUID guid = <Module>.ToGUID(ref dbGuid);
				num = <Module>.cli_RpcsDisableReplayLag2(base.BindingHandle, guid, ptr, (int)actionInitiator, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceError<Guid, int>((long)this.GetHashCode(), "ReplayRpcClient::RpccDisableReplayLag2() for guid: {0}, FAILED with HR=0x{1:x}", dbGuid, num);
					ReplayRpcException.ThrowRpcException(num, "cli_RpcsDisableReplayLag2");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpcsDisableReplayLag2");
			}
			finally
			{
				<Module>.FreeString(ptr);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccEnableReplayLag2(Guid dbGuid, uint actionInitiator)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<Guid, uint>((long)this.GetHashCode(), "Entering ReplayRpcClient::RpccEnableReplayLag2() for guid: {0}, actionInitiator: {1}", dbGuid, actionInitiator);
			try
			{
				_GUID guid = <Module>.ToGUID(ref dbGuid);
				num = <Module>.cli_RpcsEnableReplayLag2(base.BindingHandle, guid, (int)actionInitiator, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceError<Guid, int>((long)this.GetHashCode(), "ReplayRpcClient::RpccEnableReplayLag2() for guid: {0}, FAILED with HR=0x{1:x}", dbGuid, num);
					ReplayRpcException.ThrowRpcException(num, "cli_RpcsEnableReplayLag2");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpcsEnableReplayLag2");
			}
			finally
			{
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccNotifyChangedReplayConfiguration(Guid guid, [MarshalAs(UnmanagedType.U1)] bool waitForCompletion, [MarshalAs(UnmanagedType.U1)] bool exitAfterEnqueueing, [MarshalAs(UnmanagedType.U1)] bool isHighPriority, int changeHint)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			try
			{
				_GUID guid2 = <Module>.ToGUID(ref guid);
				int num2 = (!isHighPriority) ? 0 : 1;
				int num3 = (!exitAfterEnqueueing) ? 0 : 1;
				int num4 = (!waitForCompletion) ? 0 : 1;
				num = <Module>.cli_RpcsNotifyChangedReplayConfiguration(base.BindingHandle, guid2, num4, num3, num2, changeHint, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_RpcsNotifyChangedReplayConfiguration");
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpcsNotifyChangedReplayConfiguration");
			}
			finally
			{
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccInstallFailoverClustering(out string verboseLog)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "Entering ReplayRpcClient::RpccInstallFailoverClustering()");
			ushort* pStr = null;
			try
			{
				num = <Module>.cli_RpcsClusterInstallFailoverClustering(base.BindingHandle, &pStr, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<int>((long)this.GetHashCode(), "ReplayRpcClient::RpccInstallFailoverClustering(), FAILED with hr=0x{0:x}", num);
					ReplayRpcException.ThrowRpcException(num, "cli_RpccInstallFailoverClustering");
				}
				verboseLog = <Module>.UToMString(pStr);
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpccInstallFailoverClustering");
			}
			finally
			{
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccCreateCluster(string clusterName, string firstNodeName, string[] ipAddresses, uint[] rgNetMasks, out string verboseLog)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "Entering ReplayRpcClient::RpccCreateCluster()");
			ushort* ptr = null;
			ushort* ptr2 = null;
			ushort* pStr = null;
			ushort** ptr3 = null;
			uint* ptr4 = null;
			int num2 = ipAddresses.Length;
			try
			{
				ptr = <Module>.StringToUnmanaged(clusterName);
				ptr2 = <Module>.StringToUnmanaged(firstNodeName);
				ulong num3 = (ulong)((long)num2);
				ulong num4 = num3;
				ushort** ptr5 = <Module>.@new((num4 > 2305843009213693951UL) ? ulong.MaxValue : (num4 * 8UL));
				ptr3 = ptr5;
				ulong num5 = num3;
				uint* ptr6 = <Module>.@new((num5 > 4611686018427387903UL) ? ulong.MaxValue : (num5 * 4UL));
				ptr4 = ptr6;
				if (ptr != null && ptr2 != null && ptr5 != null && ptr6 != null)
				{
					initblk(ptr5, 0, num3 * 8UL);
					for (int i = 0; i < num2; i++)
					{
						long num6 = (long)i;
						*(long*)(num6 * 8L / (long)sizeof(ushort*) + ptr3) = <Module>.StringToUnmanaged(ipAddresses[i]);
						*(int*)(num6 * 4L / (long)sizeof(uint) + ptr4) = (int)rgNetMasks[i];
					}
					try
					{
						num = <Module>.cli_RpcsClusterCreateCluster(base.BindingHandle, ptr, ptr2, num2, ptr3, ptr4, &pStr, &tagErrorExceptionInfo);
						if (num < 0)
						{
							ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<int>((long)this.GetHashCode(), "ReplayRpcClient::RpccCreateCluster(), FAILED with hr=0x{0:x}", num);
							ReplayRpcException.ThrowRpcException(num, "cli_RpccCreateCluster");
						}
						verboseLog = <Module>.UToMString(pStr);
						return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
					}
					catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
					{
						RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpccCreateCluster");
						goto IL_186;
					}
				}
				ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "ReplayRpcClient::RpccCreateCluster failed to allocate memory.");
				IL_186:;
			}
			finally
			{
				for (int j = 0; j < num2; j++)
				{
					<Module>.FreeString(*(long*)((long)j * 8L + ptr3 / 8));
				}
				<Module>.delete((void*)ptr3);
				<Module>.delete((void*)ptr4);
				<Module>.FreeString(ptr);
				<Module>.FreeString(ptr2);
				<Module>.FreeString(pStr);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccDestroyCluster(string clusterName, out string verboseLog)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "Entering ReplayRpcClient::RpccDestroyCluster()");
			ushort* pStr = null;
			ushort* ptr = null;
			try
			{
				ptr = <Module>.StringToUnmanaged(clusterName);
				if (ptr == null)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "ReplayRpcClient::RpccDestroyCluster failed to allocate memory.");
				}
				else
				{
					try
					{
						num = <Module>.cli_RpcsClusterDestroyCluster(base.BindingHandle, ptr, &pStr, &tagErrorExceptionInfo);
						if (num < 0)
						{
							ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<int>((long)this.GetHashCode(), "ReplayRpcClient::RpccDestroyCluster(), FAILED with hr=0x{0:x}", num);
							ReplayRpcException.ThrowRpcException(num, "cli_RpccDestroyCluster");
						}
						verboseLog = <Module>.UToMString(pStr);
						return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
					}
					catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
					{
						RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpccDestroyCluster");
					}
				}
			}
			finally
			{
				<Module>.FreeString(ptr);
				<Module>.FreeString(pStr);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccAddNodeToCluster(string newNode, out string verboseLog)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "Entering ReplayRpcClient::RpccAddNodeToCluster()");
			ushort* pStr = null;
			ushort* ptr = null;
			try
			{
				ptr = <Module>.StringToUnmanaged(newNode);
				if (ptr == null)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "ReplayRpcClient::RpccAddNodeToCluster failed to allocate memory.");
				}
				else
				{
					try
					{
						num = <Module>.cli_RpcsClusterAddNodeToCluster(base.BindingHandle, ptr, &pStr, &tagErrorExceptionInfo);
						if (num < 0)
						{
							ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<int>((long)this.GetHashCode(), "ReplayRpcClient::RpccAddNodeToCluster(), FAILED with hr=0x{0:x}", num);
							ReplayRpcException.ThrowRpcException(num, "cli_RpccAddNodeToCluster");
						}
						verboseLog = <Module>.UToMString(pStr);
						return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
					}
					catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
					{
						RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpccAddNodeToCluster");
					}
				}
			}
			finally
			{
				<Module>.FreeString(ptr);
				<Module>.FreeString(pStr);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccEvictNodeFromCluster(string convictedNode, out string verboseLog)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "Entering ReplayRpcClient::RpccEvictNodeFromCluster()");
			ushort* pStr = null;
			ushort* ptr = null;
			try
			{
				ptr = <Module>.StringToUnmanaged(convictedNode);
				if (ptr == null)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "ReplayRpcClient::RpccEvictNodeFromCluster failed to allocate memory.");
				}
				else
				{
					try
					{
						num = <Module>.cli_RpcsClusterEvictNodeFromCluster(base.BindingHandle, ptr, &pStr, &tagErrorExceptionInfo);
						if (num < 0)
						{
							ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<int>((long)this.GetHashCode(), "ReplayRpcClient::RpccEvictNodeFromCluster(), FAILED with hr=0x{0:x}", num);
							ReplayRpcException.ThrowRpcException(num, "cli_RpccEvictNodeFromCluster");
						}
						verboseLog = <Module>.UToMString(pStr);
						return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
					}
					catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
					{
						RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpccEvictNodeFromCluster");
					}
				}
			}
			finally
			{
				<Module>.FreeString(ptr);
				<Module>.FreeString(pStr);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccForceCleanupNode(out string verboseLog)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug((long)this.GetHashCode(), "Entering ReplayRpcClient::RpccForceCleanupNode()");
			ushort* pStr = null;
			try
			{
				num = <Module>.cli_RpcsClusterForceCleanupNode(base.BindingHandle, &pStr, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<int>((long)this.GetHashCode(), "ReplayRpcClient::RpccForceCleanupNode(), FAILED with hr=0x{0:x}", num);
					ReplayRpcException.ThrowRpcException(num, "cli_RpccForceCleanupNode");
				}
				verboseLog = <Module>.UToMString(pStr);
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpccForceCleanupNode");
			}
			finally
			{
				<Module>.FreeString(pStr);
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe RpcErrorExceptionInfo RpccGetCopyStatusWithHealthState(RpcGetDatabaseCopyStatusFlags2 collectionFlags2, Guid[] dbGuids, ref RpcCopyStatusContainer container)
		{
			int num = 0;
			tagErrorExceptionInfo tagErrorExceptionInfo;
			<Module>.InitErrorExceptionInfo(ref tagErrorExceptionInfo);
			_GUID* ptr = null;
			byte* ptr2 = null;
			int num2 = 0;
			int num3 = 0;
			container = null;
			try
			{
				ptr = <Module>.Microsoft.Exchange.Rpc.Cluster.?A0x9d18277a.FromGuidArray(dbGuids, &num2);
				num = <Module>.cli_RpcsGetCopyStatusWithHealthState(base.BindingHandle, (uint)collectionFlags2, num2, ptr, &num3, &ptr2, &tagErrorExceptionInfo);
				if (num < 0)
				{
					ReplayRpcException.ThrowRpcException(num, "cli_RpcsGetCopyStatusWithHealthState");
				}
				ExTraceGlobals.ReplayServiceRpcTracer.TraceDebug<int>((long)this.GetHashCode(), "ReplayRpcClient::cli_RpcsGetCopyStatusWithHealthState() returned a total of {0} bytes.", num3);
				if (num3 > 0 && ptr2 != null)
				{
					container = SerializationServices.Deserialize<RpcCopyStatusContainer>(ptr2, num3);
				}
				return <Module>.UToMErrorExceptionInfo(ref tagErrorExceptionInfo);
			}
			catch when (endfilter(<Module>.DwExRpcExceptionFilter(Marshal.GetExceptionCode()) != null))
			{
				RpcClientBase.ThrowRpcException((num >= 0) ? Marshal.GetExceptionCode() : num, "cli_RpcsGetCopyStatusWithHealthState");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
				<Module>.FreeErrorExceptionInfo(&tagErrorExceptionInfo);
			}
			return null;
		}
	}
}
