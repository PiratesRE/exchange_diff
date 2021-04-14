using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol
{
	public enum ComplianceMessageType : byte
	{
		None,
		StartWork,
		StartJob,
		RecordResult,
		Status,
		RetrieveRequest,
		EchoRequest
	}
}
