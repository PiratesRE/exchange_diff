using System;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Microsoft.Exchange.Services.Wcf
{
	public class EWSServiceHostFactory : WebServiceHostFactory
	{
		public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
		{
			ServiceHost serviceHost = new ServiceHost(typeof(EWSService), baseAddresses);
			serviceHost.Description.Endpoints[0].Behaviors.Add(new EwsWebHttpBehavior());
			return serviceHost;
		}

		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			return base.CreateServiceHost(serviceType, baseAddresses);
		}
	}
}
