using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskPlugins.NoOp
{
	internal class NoOpApplicationPlugin : IApplicationPlugin
	{
		public WorkPayload Preprocess(ComplianceMessage target, WorkPayload payload)
		{
			return payload;
		}

		public WorkPayload DoWork(ComplianceMessage target, WorkPayload payload)
		{
			return payload;
		}

		public ResultBase RecordResult(ResultBase existing, WorkPayload addition)
		{
			return existing;
		}
	}
}
