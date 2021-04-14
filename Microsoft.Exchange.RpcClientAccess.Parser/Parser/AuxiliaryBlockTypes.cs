using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal enum AuxiliaryBlockTypes : byte
	{
		PerfRequestId = 1,
		PerfClientInfo,
		PerfServerInfo,
		PerfSessionInfo,
		PerfDefMdbSuccess,
		PerfDefGcSuccess,
		PerfMdbSuccess,
		PerfGcSuccess,
		PerfFailure,
		ClientControl,
		PerfProcessInfo,
		PerfBgDefMdbSuccess,
		PerfBgDefGcSuccess,
		PerfBgMdbSuccess,
		PerfBgGcSuccess,
		PerfBgFailure,
		PerfFgDefMdbSuccess,
		PerfFgDefGcSuccess,
		PerfFgMdbSuccess,
		PerfFgGcSuccess,
		PerfFgFailure,
		OsInfo,
		ExOrgInfo,
		DiagCtxReqId = 64,
		DiagCtxCtxData,
		DiagCtxMapiServer,
		MapiEndpoint,
		PerRpcStatistics,
		ClientSessionInfo,
		ServerCapabilities,
		DiagCtxClientId,
		EndpointCapabilities,
		ExceptionTrace,
		ClientConnectionInfo,
		ServerSessionInfo,
		SetMonitoringContext,
		ClientActivity,
		ProtocolDeviceIdentification,
		MonitoringActivity,
		ServerInformation,
		IdentityCorrelationInfo
	}
}
