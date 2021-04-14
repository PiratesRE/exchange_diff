using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class SoapServiceHostFactory : ServiceHostFactory
	{
		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			return new SoapServiceHostFactory.EcpSoapServiceHost(serviceType, baseAddresses);
		}

		private class EcpSoapServiceHost : EcpServiceHostBase
		{
			public EcpSoapServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
			{
			}

			protected override Binding CreateBinding(Uri address)
			{
				WSHttpBinding wshttpBinding = new WSHttpBinding(address.Scheme);
				if (VirtualDirectoryConfiguration.IsClientCertificateRequired(address))
				{
					wshttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
				}
				else
				{
					wshttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
				}
				return wshttpBinding;
			}
		}
	}
}
