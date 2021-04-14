using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Contract;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Protocol;
using Microsoft.Exchange.Compliance.TaskDistributionCommon.Serialization;

namespace Microsoft.Exchange.Compliance.TaskDistributionCommon.Client
{
	public class DriverClient : DriverClientBase
	{
		public DriverClient(string hostName, string certificateSubject)
		{
			this.endpointAddress = WcfUtility.GetInterServiceEndpointAddress(hostName);
			this.binding = WcfUtility.CreateInterServiceBinding();
			this.thumbprint = WcfUtility.GetThumbprint(certificateSubject);
		}

		private DriverClient.WcfProxyClient ProxyClient
		{
			get
			{
				if (this.proxyClient == null)
				{
					this.CreateProxy();
				}
				return this.proxyClient;
			}
		}

		public override async Task<ComplianceMessage> GetResponseAsync(ComplianceMessage message)
		{
			byte[] messageBlob = ComplianceSerializer.Serialize<ComplianceMessage>(ComplianceMessage.Description, message);
			byte[] responseBlob = await this.TakeActionWithRetryOnCommunicationException<byte[]>(async () => await this.ProxyClient.ProcessMessageAsync(messageBlob), true);
			return ComplianceSerializer.DeSerialize<ComplianceMessage>(ComplianceMessage.Description, responseBlob);
		}

		private async Task<TResult> TakeActionWithRetryOnCommunicationException<TResult>(Func<Task<TResult>> action, bool firstTry)
		{
			TResult response = default(TResult);
			bool recreatedProxy = false;
			try
			{
				response = await action();
			}
			catch (Exception ex)
			{
				if (!(ex is CommunicationException) && !(ex is TimeoutException))
				{
					throw;
				}
				this.CreateProxy();
				recreatedProxy = true;
			}
			TResult result;
			if (recreatedProxy && firstTry)
			{
				result = await this.TakeActionWithRetryOnCommunicationException<TResult>(action, false);
			}
			else
			{
				result = response;
			}
			return result;
		}

		private void CreateProxy()
		{
			if (this.proxyClient != null)
			{
				try
				{
					this.proxyClient.Close();
				}
				catch (Exception ex)
				{
					this.proxyClient.Abort();
					if (!(ex is CommunicationException) && !(ex is TimeoutException))
					{
						throw;
					}
				}
			}
			DriverClient.WcfProxyClient wcfProxyClient = new DriverClient.WcfProxyClient(this.binding, this.endpointAddress);
			wcfProxyClient.ClientCredentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, this.thumbprint);
			wcfProxyClient.Open();
			this.proxyClient = wcfProxyClient;
		}

		private readonly string thumbprint;

		private readonly Binding binding;

		private readonly EndpointAddress endpointAddress;

		private volatile DriverClient.WcfProxyClient proxyClient;

		private class WcfProxyClient : ClientBase<IMessageProcessor>, IMessageProcessor
		{
			public WcfProxyClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
			{
			}

			public Task<byte[]> ProcessMessageAsync(byte[] message)
			{
				return base.Channel.ProcessMessageAsync(message);
			}
		}
	}
}
