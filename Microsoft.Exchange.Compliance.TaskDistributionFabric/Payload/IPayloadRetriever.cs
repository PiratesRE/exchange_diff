using System;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Payload
{
	internal interface IPayloadRetriever
	{
		bool TryGetPayload<T>(ComplianceSerializationDescription<T> description, byte[] blob, out T payload, out FaultDefinition faultDefinition) where T : Payload, new();

		IEnumerable<T> GetAllPayloads<T>(ComplianceSerializationDescription<T> description, byte[] blob, out FaultDefinition faultDefinition) where T : Payload, new();
	}
}
