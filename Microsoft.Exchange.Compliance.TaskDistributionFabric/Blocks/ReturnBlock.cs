using System;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks
{
	internal class ReturnBlock : FinalBlockBase<ComplianceMessage>
	{
		protected override void ProcessFinal(ComplianceMessage input)
		{
			IRoutingManager routingManager;
			FaultDefinition faultDefinition;
			if (Registry.Instance.TryGetInstance<IRoutingManager>(RegistryComponent.TaskDistribution, TaskDistributionComponent.RoutingManager, out routingManager, out faultDefinition, "ProcessFinal", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\ReturnBlock.cs", 32))
			{
				routingManager.ReturnMessage(input);
			}
			if (faultDefinition != null)
			{
				ExceptionHandler.FaultMessage(input, faultDefinition, true);
			}
		}
	}
}
