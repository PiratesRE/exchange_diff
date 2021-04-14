using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks
{
	internal class EchoRequestBlock : BlockBase<ComplianceMessage, ComplianceMessage>
	{
		public override ComplianceMessage Process(ComplianceMessage input)
		{
			return input;
		}
	}
}
