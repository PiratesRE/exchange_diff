using System;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	internal static class LiveConfiguration
	{
		public static EnhancedTimeSpan DefaultFederatedMetadataTimeout
		{
			get
			{
				return EnhancedTimeSpan.OneDay;
			}
		}

		internal static Uri GetLiveIdFederationMetadataEpr(FederationTrust.NamespaceProvisionerType provisionerType)
		{
			switch (provisionerType)
			{
			case FederationTrust.NamespaceProvisionerType.LiveDomainServices:
				return LiveConfiguration.GetEndpoint(ServiceEndpointId.LiveFederationMetadata);
			case FederationTrust.NamespaceProvisionerType.LiveDomainServices2:
				return LiveConfiguration.GetEndpoint(ServiceEndpointId.MsoFederationMetadata);
			default:
				throw new ArgumentException("provisionerType");
			}
		}

		internal static Uri GetDomainServicesEpr()
		{
			return LiveConfiguration.GetEndpoint(ServiceEndpointId.DomainPartnerManageDelegation);
		}

		internal static Uri GetDomainServices2Epr()
		{
			return LiveConfiguration.GetEndpoint(ServiceEndpointId.DomainPartnerManageDelegation2);
		}

		internal static WebProxy GetWebProxy(WriteVerboseDelegate writeVerbose)
		{
			WebProxy result;
			try
			{
				WebProxy webProxy = null;
				Server localServer = LocalServerCache.LocalServer;
				if (localServer != null && localServer.InternetWebProxy != null)
				{
					writeVerbose(Strings.WebProxy(localServer.InternetWebProxy.ToString()));
					webProxy = new WebProxy(localServer.InternetWebProxy);
				}
				result = webProxy;
			}
			catch (NotSupportedException ex)
			{
				throw new LiveDomainServicesException(Strings.CannotSetProxy(ex.Message), ex);
			}
			return result;
		}

		private static Uri GetEndpoint(string serviceEndpointId)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 129, "GetEndpoint", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\FederationProvisioning\\LiveConfiguration.cs");
			ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
			ServiceEndpoint endpoint = endpointContainer.GetEndpoint(serviceEndpointId);
			return new Uri(endpoint.Uri.AbsoluteUri, UriKind.Absolute);
		}

		public const string LiveExchangeProgramId = "ExchangeConnector";

		public const string DefaultTokenPolicyUri = "EX_MBI_FED_SSL";
	}
}
