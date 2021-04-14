using System;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization
{
	public enum ComplianceStructureId : byte
	{
		MessageEnvelope = 1,
		Target,
		Payload,
		WorkPayload,
		DistributionPayload,
		AggregationPayload,
		StatusPayload,
		ComplianceSearchCondition,
		ComplianceSearchData,
		ComplianceSearch,
		FaultRecord,
		FaultDefinition,
		GeneralStructure = 98,
		ApplicationPayload
	}
}
