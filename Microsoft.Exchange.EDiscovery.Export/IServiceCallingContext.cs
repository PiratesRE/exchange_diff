using System;
using System.Web.Services.Protocols;

namespace Microsoft.Exchange.EDiscovery.Export
{
	internal interface IServiceCallingContext<ServiceBindingType> where ServiceBindingType : HttpWebClientProtocol, IServiceBinding, new()
	{
		ServiceBindingType CreateServiceBinding(Uri serviceUrl);

		bool AuthorizeServiceBinding(ServiceBindingType binding);

		void SetServiceApiContext(ServiceBindingType binding, string mailboxEmailAddress);

		void SetServiceUrl(ServiceBindingType binding, Uri targetUrl);

		void SetServiceUrlAffinity(ServiceBindingType binding, Uri targetUrl);
	}
}
