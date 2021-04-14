using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	public class MsoOnboardingService : OnboardingService
	{
		public static string GetSiteName()
		{
			return LocalSiteCache.LocalSite.Name;
		}

		public static ServiceEndpoint GetMsoEndpoint()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 71, "GetMsoEndpoint", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\MSOOnboardingService.cs");
			ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
			return endpointContainer.GetEndpoint("MSOSyncEndpoint");
		}

		public static string GetErrorStringForResultcode(ResultCode? result)
		{
			if (result != null && result.Value == ResultCode.Success)
			{
				return string.Empty;
			}
			string result2;
			if (result == null)
			{
				result2 = Strings.SetServiceInstanceMapReturnedNull;
			}
			else
			{
				string description;
				switch (result.Value)
				{
				case ResultCode.PartitionUnavailable:
					description = Strings.SetServiceInstanceMapResultCodePartitionUnavailable;
					break;
				case ResultCode.ObjectNotFound:
					description = Strings.SetServiceInstanceMapResultCodeObjectNotFound;
					break;
				case ResultCode.UnspecifiedError:
					description = Strings.SetServiceInstanceMapResultCodeUnspecifiedError;
					break;
				default:
					description = Strings.SetServiceInstanceMapResultCodeUnknownError;
					break;
				}
				result2 = Strings.SetServiceInstanceMapResultFormat(result.Value.ToString(), description);
			}
			return result2;
		}

		protected override IFederatedServiceOnboarding CreateService()
		{
			ServiceEndpoint serviceEndpoint = null;
			try
			{
				serviceEndpoint = MsoOnboardingService.GetMsoEndpoint();
			}
			catch (Exception innerException)
			{
				throw new CouldNotCreateMsoOnboardingServiceException(Strings.CouldNotGetMsoEndpoint, innerException);
			}
			IFederatedServiceOnboarding result;
			try
			{
				EndpointAddress remoteAddress = new EndpointAddress(serviceEndpoint.Uri, new AddressHeader[0]);
				FederatedServiceOnboardingClient federatedServiceOnboardingClient = new FederatedServiceOnboardingClient(new WSHttpBinding(SecurityMode.Transport)
				{
					Security = 
					{
						Transport = 
						{
							ClientCredentialType = HttpClientCredentialType.Certificate
						}
					},
					MaxBufferPoolSize = 5242880L,
					MaxReceivedMessageSize = 5242880L
				}, remoteAddress);
				X509Certificate2 certificate = TlsCertificateInfo.FindFirstCertWithSubjectDistinguishedName(serviceEndpoint.CertificateSubject);
				federatedServiceOnboardingClient.ClientCredentials.ClientCertificate.Certificate = certificate;
				result = federatedServiceOnboardingClient;
			}
			catch (Exception ex)
			{
				throw new CouldNotCreateMsoOnboardingServiceException(ex.Message, ex);
			}
			return result;
		}

		public const string MSOSyncEndpointName = "MSOSyncEndpoint";

		private const int DefaultMaxBufferPoolSize = 5242880;

		private const int DefaultMaxReceivedMessageSize = 5242880;
	}
}
