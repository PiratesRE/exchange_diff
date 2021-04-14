using System;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Diagnostics;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionFabric.Routing;

namespace Microsoft.Exchange.Compliance.TaskDistributionFabric.Dataflow
{
	internal abstract class MessageReceiverBase
	{
		public virtual bool ReceiveMessage(ComplianceMessage message)
		{
			IRoutingManager routingManager;
			FaultDefinition faultDefinition;
			if (!Registry.Instance.TryGetInstance<IRoutingManager>(RegistryComponent.TaskDistribution, TaskDistributionComponent.RoutingManager, out routingManager, out faultDefinition, "ReceiveMessage", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Dataflow\\MessageReceiverBase.cs", 39))
			{
				return false;
			}
			if (!routingManager.ReceiveMessage(message))
			{
				return true;
			}
			Task task = this.ReceiveMessageInternal(message);
			if (task != null)
			{
				task.ContinueWith(delegate(Task t)
				{
					if (t.IsFaulted)
					{
						ExceptionHandler.FaultMessage(message, FaultDefinition.FromException(t.Exception, false, false, "ReceiveMessage", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionFabric\\Dataflow\\MessageReceiverBase.cs", 52), true);
					}
					routingManager.ProcessedMessage(message);
				});
				return true;
			}
			return false;
		}

		protected abstract Task ReceiveMessageInternal(ComplianceMessage message);
	}
}
