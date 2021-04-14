using System;
using System.Collections.Generic;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Resolver;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks
{
	internal class SendBlock : FinalBlockBase<IEnumerable<ComplianceMessage>>
	{
		protected override void ProcessFinal(IEnumerable<ComplianceMessage> input)
		{
			IRoutingManager routingManager;
			FaultDefinition faultDefinition;
			ITargetResolver targetResolver;
			if (Registry.Instance.TryGetInstance<IRoutingManager>(RegistryComponent.TaskDistribution, TaskDistributionComponent.RoutingManager, out routingManager, out faultDefinition, "ProcessFinal", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\SendBlock.cs", 35) && Registry.Instance.TryGetInstance<ITargetResolver>(RegistryComponent.TaskDistribution, TaskDistributionComponent.TargetResolver, out targetResolver, out faultDefinition, "ProcessFinal", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Blocks\\SendBlock.cs", 37))
			{
				foreach (ComplianceMessage message in targetResolver.Resolve(input))
				{
					routingManager.SendMessage(message);
				}
			}
		}
	}
}
