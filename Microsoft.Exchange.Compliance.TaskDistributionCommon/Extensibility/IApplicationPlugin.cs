using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility
{
	internal interface IApplicationPlugin
	{
		WorkPayload Preprocess(ComplianceMessage target, WorkPayload payload);

		WorkPayload DoWork(ComplianceMessage target, WorkPayload payload);

		ResultBase RecordResult(ResultBase existing, WorkPayload addition);
	}
}
