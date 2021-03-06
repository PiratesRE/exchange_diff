using System;

namespace Microsoft.Mapi
{
	internal enum CodeLid
	{
		None,
		RemoteEventsBeg = 1494,
		RemoteRequestId = 2006,
		RemoteCtxOverflow = 1238,
		RemoteEventsEnd = 1750,
		EcDoConnect_RpcCalled = 21017,
		EcDoConnect_RpcReturned = 29209,
		EcDoConnect_RpcException = 19961,
		EcDoConnectEx_RpcCalled = 23065,
		EcDoConnectEx_RpcReturned = 17913,
		EcDoConnectEx_RpcException = 28153,
		EcDoRpc_RpcCalled = 31257,
		EcDoRpc_RpcReturned = 16921,
		EcDoRpc_RpcException = 24057,
		EcDoRpcExt2_RpcCalled = 18969,
		EcDoRpcExt2_RpcReturned = 27161,
		EcDoRpcExt2_RpcException = 32249,
		EMSMDB_EcDoDisconnect_RpcCalled = 38951,
		EMSMDB_EcDoDisconnect_RpcReturned = 55335,
		EMSMDB_EcDoDisconnect_RpcException = 43047,
		EMSMDB_EcDoConnectEx_RpcCalled = 59431,
		EMSMDB_EcDoConnectEx_RpcReturned = 34855,
		EMSMDB_EcDoConnectEx_RpcException = 51239,
		EMSMDB_EcDoRpcExt2_RpcCalled = 45095,
		EMSMDB_EcDoRpcExt2_RpcReturned = 61479,
		EMSMDB_EcDoRpcExt2_RpcException = 36903,
		EMSMDB_EcDoAsyncConnectEx_RpcCalled = 35879,
		EMSMDB_EcDoAsyncConnectEx_RpcReturned = 52263,
		EMSMDB_EcDoAsyncConnectEx_RpcException = 46119,
		EMSMDB_EcDoAsyncWaitEx_RpcCalled = 62503,
		EMSMDB_EcDoAsyncWaitEx_RpcReturned = 37927,
		EMSMDB_EcDoAsyncWaitEx_RpcException = 54311,
		EMSMDBMT_EcDoDisconnect_RpcCalled = 40999,
		EMSMDBMT_EcDoDisconnect_RpcReturned = 57383,
		EMSMDBMT_EcDoDisconnect_RpcException = 32807,
		EMSMDBMT_EcDoConnectEx_RpcCalled = 49191,
		EMSMDBMT_EcDoConnectEx_RpcReturned = 48679,
		EMSMDBMT_EcDoConnectEx_RpcException = 65063,
		EMSMDBMT_EcDoRpcExt2_RpcCalled = 40487,
		EMSMDBMT_EcDoRpcExt2_RpcReturned = 56871,
		EMSMDBMT_EcDoRpcExt2_RpcException = 44583,
		EMSMDBMT_EcDoAsyncConnectEx_RpcCalled = 42023,
		EMSMDBMT_EcDoAsyncConnectEx_RpcReturned = 58407,
		EMSMDBMT_EcDoAsyncConnectEx_RpcException = 33831,
		EMSMDBMT_EcDoAsyncWaitEx_RpcCalled = 50215,
		EMSMDBMT_EcDoAsyncWaitEx_RpcReturned = 47143,
		EMSMDBMT_EcDoAsyncWaitEx_RpcException = 63527,
		EMSMDBPOOL_EcPoolWaitForNotificationsAsync_RpcCalled = 36391,
		EMSMDBPOOL_EcPoolWaitForNotificationsAsync_RpcReturned = 52775,
		EMSMDBPOOL_EcPoolWaitForNotificationsAsync_RpcException = 46631,
		EMSMDBPOOL_EcPoolConnect_RpcCalled = 38439,
		EMSMDBPOOL_EcPoolConnect_RpcReturned = 54823,
		EMSMDBPOOL_EcPoolConnect_RpcException = 42535,
		EMSMDBPOOL_EcPoolCloseSession_RpcCalled = 58919,
		EMSMDBPOOL_EcPoolCloseSession_RpcReturned = 34343,
		EMSMDBPOOL_EcPoolCloseSession_RpcException = 50727,
		EMSMDBPOOL_EcPoolCreateSession_RpcCalled = 47655,
		EMSMDBPOOL_EcPoolCreateSession_RpcReturned = 64039,
		EMSMDBPOOL_EcPoolCreateSession_RpcException = 39463,
		EMSMDBPOOL_EcPoolSessionDoRpc_RpcCalled = 55847,
		EMSMDBPOOL_EcPoolSessionDoRpc_RpcReturned = 43559,
		EMSMDBPOOL_EcPoolSessionDoRpc_RpcException = 59943,
		ResponseParseStart = 23226,
		ResponseRop = 27962,
		ResponseRopError = 17082,
		ResponseParseDone = 31418,
		ResponseParseFailure = 21817,
		RequestRop = 26426,
		EcPoolEnter_DeadPool = 39793,
		EcCreatePool_DeadPool = 50551,
		EcConnectToServerPool_DeadPool = 60017,
		EEInfo_EnumerationFailure = 14232,
		EEInfo_NextRecordFailure = 12184,
		EEInfo_ComputerName = 16280,
		EEInfo_PID = 8600,
		EEInfo_GenerationTime = 12696,
		EEInfo_GeneratingComponent = 10648,
		EEInfo_Status = 14744,
		EEInfo_DetectionLocation = 9624,
		EEInfo_Flags = 13720,
		EEInfo_NumberOfParameters = 11672,
		EEInfo_Parameter_Ansi = 15768,
		EEInfo_Parameter_Unicode = 8856,
		EEInfo_Parameter_Long = 12952,
		EEInfo_Parameter_Short = 10904,
		EEInfo_Parameter_Pointer = 15000,
		EEInfo_Parameter_Truncated = 9880,
		EEInfo_Parameter_Binary = 13976,
		EEInfo_Parameter_Unknown = 11928,
		ServerVersion_EcDoConnectEx = 51056,
		ClientVersion_EcDoConnectEx = 50544,
		ServerVersion_EcDoRpcExt2 = 50032,
		ClientVersion_EcDoRpcExt2 = 52176
	}
}
