using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class ExchangeSessionAwareClientsHelper
	{
		static ExchangeSessionAwareClientsHelper()
		{
			if (!ExchangeSessionAwareClientsHelper.webRequestCreator.IsDisabled)
			{
				WebRequest.RegisterPrefix("http://", ExchangeSessionAwareClientsHelper.webRequestCreator);
				WebRequest.RegisterPrefix("https://", ExchangeSessionAwareClientsHelper.webRequestCreator);
			}
		}

		public static IMailboxReplicationProxyService CreateChannel(MailboxReplicationProxyClient proxyClient)
		{
			ChannelFactory<IMailboxReplicationProxyService> channelFactory = proxyClient.ChannelFactory;
			EndpointAddress address = proxyClient.Endpoint.Address;
			if (address.Uri != null && (address.Uri.Scheme == Uri.UriSchemeHttps || address.Uri.Scheme == Uri.UriSchemeHttp))
			{
				if (!ExchangeSessionAwareClientsHelper.webRequestCreator.IsDisabled)
				{
					UriBuilder uriBuilder = new UriBuilder(address.Uri);
					uriBuilder.Path = proxyClient.RequestContext.Id.ToString();
					if (proxyClient.UseCertificateToAuthenticate)
					{
						string config = ConfigBase<MRSConfigSchema>.GetConfig<string>("ProxyClientCertificateSubject");
						try
						{
							channelFactory.Credentials.ClientCertificate.Certificate = TlsCertificateInfo.FindFirstCertWithSubjectDistinguishedName(config, false);
						}
						catch (ArgumentException ex)
						{
							throw new CertificateLoadErrorException(config, ex.Message, ex);
						}
					}
					return channelFactory.CreateChannel(address, uriBuilder.Uri);
				}
				CustomBinding customBinding = channelFactory.Endpoint.Binding as CustomBinding;
				if (customBinding != null)
				{
					HttpsTransportBindingElement httpsTransportBindingElement = customBinding.Elements.Find<HttpsTransportBindingElement>();
					if (httpsTransportBindingElement != null)
					{
						httpsTransportBindingElement.AllowCookies = true;
					}
				}
			}
			return channelFactory.CreateChannel(address);
		}

		private static readonly CookieAwareWebRequestCreator webRequestCreator = new CookieAwareWebRequestCreator();
	}
}
