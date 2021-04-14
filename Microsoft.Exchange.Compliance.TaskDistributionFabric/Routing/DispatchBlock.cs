using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Client;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Instrumentation;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Resolver;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Utility;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Blocks;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing
{
	internal class DispatchBlock : FinalBlockBase<IEnumerable<ComplianceMessage>>
	{
		protected override void ProcessFinal(IEnumerable<ComplianceMessage> input)
		{
			ITargetResolver resolver = null;
			IRoutingManager routingManager = null;
			IEnumerable<ComplianceMessage> messages = input;
			IMessageSender sender;
			FaultDefinition faultDefinition;
			if (Registry.Instance.TryGetInstance<IRoutingManager>(RegistryComponent.TaskDistribution, TaskDistributionComponent.RoutingManager, out routingManager, out faultDefinition, "ProcessFinal", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\DispatchBlock.cs", 42) && Registry.Instance.TryGetInstance<ITargetResolver>(RegistryComponent.TaskDistribution, TaskDistributionComponent.TargetResolver, out resolver, out faultDefinition, "ProcessFinal", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\DispatchBlock.cs", 44) && Registry.Instance.TryGetInstance<IMessageSender>(RegistryComponent.Client, MessageHelper.GetClientType(input), out sender, out faultDefinition, "ProcessFinal", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\DispatchBlock.cs", 46))
			{
				ExceptionHandler.Proxy.TryRun(delegate
				{
					sender.SendMessageAsync(this.SetDispatchingStatus(resolver.Resolve(messages))).ContinueWith(delegate(Task<bool[]> task)
					{
						this.SetDispatchedStatus(messages, task.Result, routingManager);
					});
				}, TaskDistributionSettings.RemoteExecutionTime, out faultDefinition, null, null, default(CancellationToken), null, "ProcessFinal", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Routing\\Cache\\DispatchBlock.cs", 48);
			}
			if (faultDefinition != null)
			{
				foreach (ComplianceMessage message in messages)
				{
					ExceptionHandler.FaultMessage(message, faultDefinition, true);
				}
			}
		}

		private IEnumerable<ComplianceMessage> SetDispatchingStatus(IEnumerable<ComplianceMessage> messages)
		{
			foreach (ComplianceMessage message in messages)
			{
				MessageLogger.Instance.LogMessageDispatching(message);
				yield return message;
			}
			yield break;
		}

		private void SetDispatchedStatus(IEnumerable<ComplianceMessage> messages, bool[] messageStatuses, IRoutingManager routingManager)
		{
			foreach (Tuple<ComplianceMessage, bool> tuple in messages.Zip(messageStatuses, (ComplianceMessage message, bool status) => new Tuple<ComplianceMessage, bool>(message, status)))
			{
				if (tuple.Item2)
				{
					routingManager.DispatchedMessage(tuple.Item1);
				}
			}
		}
	}
}
