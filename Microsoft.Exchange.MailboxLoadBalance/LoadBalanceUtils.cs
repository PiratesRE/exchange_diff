using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel.Description;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class LoadBalanceUtils
	{
		public static IList<Guid> GetNonMovableOrgsList(ILoadBalanceSettings loadBalanceSettings)
		{
			IList<Guid> list = new List<Guid>();
			foreach (string input in loadBalanceSettings.NonMovableOrganizationIds.Split(new string[]
			{
				";"
			}, StringSplitOptions.RemoveEmptyEntries))
			{
				Guid item;
				if (Guid.TryParse(input, out item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static object GetSyncRoot<TElement>(this ICollection<TElement> collection)
		{
			return ((ICollection)collection).SyncRoot;
		}

		public static void SetDataContractSerializerBehavior(ContractDescription contract)
		{
			foreach (OperationDescription operationDescription in contract.Operations)
			{
				if (operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>() != null)
				{
					operationDescription.Behaviors.Remove<DataContractSerializerOperationBehavior>();
				}
				LoadBalanceDataContractSerializationBehavior item = new LoadBalanceDataContractSerializationBehavior(operationDescription);
				operationDescription.Behaviors.Add(item);
			}
		}

		internal static void UpdateAndLogServiceEndpoint(ILogger logger, ServiceEndpoint endpoint)
		{
			LoadBalanceUtils.SetDataContractSerializerBehavior(endpoint.Contract);
			logger.LogVerbose("Connected endpoint {0} with binding {1} and contract {2} with session mode {3}", new object[]
			{
				endpoint.Address,
				endpoint.Binding.Name,
				endpoint.Contract.Name,
				endpoint.Contract.SessionMode
			});
			foreach (OperationDescription operationDescription in endpoint.Contract.Operations)
			{
				logger.LogVerbose("Operation:: {0}, {1}, {2}", new object[]
				{
					operationDescription.Name,
					operationDescription.ProtectionLevel,
					operationDescription.Behaviors.Find<DataContractSerializerOperationBehavior>()
				});
				foreach (MessageDescription messageDescription in operationDescription.Messages)
				{
					logger.LogVerbose("Operation[{0}]::Message({1}, {2})", new object[]
					{
						operationDescription.Name,
						messageDescription.Action,
						messageDescription.MessageType
					});
				}
			}
		}
	}
}
