using System;
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
	public class MsoSyncService : SyncService
	{
		public static ServiceEndpoint GetMsoEndpoint()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 66, "GetMsoEndpoint", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\MsoSyncService.cs");
			ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
			return endpointContainer.GetEndpoint("MSOSyncEndpoint");
		}

		protected override DirectorySyncClient CreateService()
		{
			ServiceEndpoint serviceEndpoint = null;
			try
			{
				serviceEndpoint = MsoSyncService.GetMsoEndpoint();
			}
			catch (Exception innerException)
			{
				throw new CouldNotCreateMsoSyncServiceException(Strings.CouldNotGetMsoEndpoint, innerException);
			}
			DirectorySyncClient result;
			try
			{
				EndpointAddress remoteAddress = new EndpointAddress(serviceEndpoint.Uri, new AddressHeader[0]);
				result = new DirectorySyncClient(new WSHttpBinding(SecurityMode.Transport)
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
				}, remoteAddress)
				{
					ClientCredentials = 
					{
						ClientCertificate = 
						{
							Certificate = TlsCertificateInfo.FindFirstCertWithSubjectDistinguishedName(serviceEndpoint.CertificateSubject)
						}
					}
				};
			}
			catch (Exception ex)
			{
				throw new CouldNotCreateMsoSyncServiceException(ex.Message, ex);
			}
			return result;
		}

		public const string MSOSyncEndpointName = "MSOSyncEndpoint";

		private const string SyncWSBindingName = "SyncServiceBinding";

		private const int DefaultMaxBufferPoolSize = 5242880;

		private const int DefaultMaxReceivedMessageSize = 5242880;
	}
}
