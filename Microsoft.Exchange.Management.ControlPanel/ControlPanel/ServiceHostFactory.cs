using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class ServiceHostFactory : ServiceHostFactory
	{
		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			return new ServiceHostFactory.EcpServiceHost(serviceType, baseAddresses);
		}

		private class EcpServiceHost : EcpServiceHostBase
		{
			public EcpServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
			{
			}

			protected override Binding CreateBinding(Uri address)
			{
				WebHttpBinding webHttpBinding = new WebHttpBinding(address.Scheme);
				if (VirtualDirectoryConfiguration.IsClientCertificateRequired(address))
				{
					webHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
				}
				return webHttpBinding;
			}

			protected override void ApplyServiceEndPointConfiguration(ServiceEndpoint serviceEndpoint)
			{
				base.ApplyServiceEndPointConfiguration(serviceEndpoint);
				serviceEndpoint.Behaviors.Insert(0, ServiceHostFactory.EcpServiceHost.webScriptEnablingBehavior);
			}

			private static readonly WebScriptEnablingBehavior webScriptEnablingBehavior = new WebScriptEnablingBehavior();
		}
	}
}
