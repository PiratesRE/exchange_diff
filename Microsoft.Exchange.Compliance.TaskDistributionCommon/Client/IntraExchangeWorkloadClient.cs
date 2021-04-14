using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Contract;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Extensibility;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Client
{
	public class IntraExchangeWorkloadClient : WorkloadClientBase
	{
		public IntraExchangeWorkloadClient()
		{
			this.channelFactory = new ChannelFactory<IMessageReceiver>(WcfUtility.CreateIntraServiceBinding(true));
		}

		protected override async Task<IEnumerable<ComplianceMessage>> SendMessageAsyncInternal(IEnumerable<ComplianceMessage> messages)
		{
			string remoteServer = messages.First<ComplianceMessage>().MessageTarget.Server ?? Environment.MachineName;
			IMessageReceiver channel = null;
			byte[][] response = null;
			bool disposeReceiver = true;
			if (remoteServer.Equals("Loopback", StringComparison.InvariantCultureIgnoreCase))
			{
				FaultDefinition faultDefinition;
				if (Registry.Instance.TryGetInstance<IMessageReceiver>(RegistryComponent.Common, CommonComponent.MessageReceiver, out channel, out faultDefinition, "SendMessageAsyncInternal", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\TaskDistributionSystem\\TaskDistributionCommon\\Client\\IntraExchangeWorkloadClient.cs", 61))
				{
					disposeReceiver = false;
				}
				else
				{
					remoteServer = Environment.MachineName;
					channel = this.channelFactory.CreateChannel(WcfUtility.GetBackendServerEndpointAddress(remoteServer));
				}
			}
			else
			{
				channel = this.channelFactory.CreateChannel(WcfUtility.GetBackendServerEndpointAddress(remoteServer));
			}
			try
			{
				byte[][] messageBlobs = WcfUtility.GetMessageBlobs(messages);
				response = await channel.ReceiveMessagesAsync(messageBlobs);
			}
			finally
			{
				if (disposeReceiver)
				{
					ICommunicationObject communicationObject = (ICommunicationObject)channel;
					try
					{
						if (communicationObject.State != CommunicationState.Faulted)
						{
							communicationObject.Close();
						}
					}
					catch
					{
						communicationObject.Abort();
					}
				}
			}
			return WcfUtility.GetMessagesFromBlobs(response);
		}

		private ChannelFactory<IMessageReceiver> channelFactory;
	}
}
