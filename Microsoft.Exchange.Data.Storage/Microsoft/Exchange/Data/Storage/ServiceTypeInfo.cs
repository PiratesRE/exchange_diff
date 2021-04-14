using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class ServiceTypeInfo
	{
		private ServiceTypeInfo(Type type, ServiceType serviceType, Delegate tryCreateServiceDelegate)
		{
			this.type = type;
			this.serviceType = serviceType;
			this.tryCreateServiceDelegate = tryCreateServiceDelegate;
		}

		internal static Service CreateHttpService(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod)
		{
			foreach (ServiceTypeInfo serviceTypeInfo in ServiceTypeInfo.serviceTypeInfos)
			{
				ServiceTypeInfo.TryCreateHttpServiceDelegate tryCreateHttpServiceDelegate = serviceTypeInfo.tryCreateServiceDelegate as ServiceTypeInfo.TryCreateHttpServiceDelegate;
				Service result;
				if (tryCreateHttpServiceDelegate != null && tryCreateHttpServiceDelegate(virtualDirectory, serverInfo, url, clientAccessType, authenticationMethod, out result))
				{
					return result;
				}
			}
			throw new InvalidOperationException(ServerStrings.ExInvalidServiceType);
		}

		internal static Service CreateEmailTransportService(MiniEmailTransport emailTransport, TopologyServerInfo serverInfo, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod)
		{
			foreach (ServiceTypeInfo serviceTypeInfo in ServiceTypeInfo.serviceTypeInfos)
			{
				ServiceTypeInfo.TryCreateEmailTransportServiceDelegate tryCreateEmailTransportServiceDelegate = serviceTypeInfo.tryCreateServiceDelegate as ServiceTypeInfo.TryCreateEmailTransportServiceDelegate;
				Service result;
				if (tryCreateEmailTransportServiceDelegate != null && tryCreateEmailTransportServiceDelegate(emailTransport, serverInfo, clientAccessType, authenticationMethod, out result))
				{
					return result;
				}
			}
			throw new InvalidOperationException(ServerStrings.ExInvalidServiceType);
		}

		internal static Service CreateSmtpService(MiniReceiveConnector smtpReceiveConnector, TopologyServerInfo serverInfo, ClientAccessType clientAccessType)
		{
			foreach (ServiceTypeInfo serviceTypeInfo in ServiceTypeInfo.serviceTypeInfos)
			{
				ServiceTypeInfo.TryCreateSmtpServiceDelegate tryCreateSmtpServiceDelegate = serviceTypeInfo.tryCreateServiceDelegate as ServiceTypeInfo.TryCreateSmtpServiceDelegate;
				if (tryCreateSmtpServiceDelegate != null)
				{
					Service service;
					Service result;
					if (tryCreateSmtpServiceDelegate(smtpReceiveConnector, serverInfo, clientAccessType, out service))
					{
						result = service;
					}
					else
					{
						result = null;
					}
					return result;
				}
			}
			throw new InvalidOperationException(ServerStrings.ExInvalidServiceType);
		}

		internal static ServiceType GetServiceType(Type type)
		{
			foreach (ServiceTypeInfo serviceTypeInfo in ServiceTypeInfo.serviceTypeInfos)
			{
				if (type.Equals(serviceTypeInfo.type))
				{
					return serviceTypeInfo.serviceType;
				}
			}
			return ServiceType.Invalid;
		}

		private static readonly ServiceTypeInfo[] serviceTypeInfos = new ServiceTypeInfo[]
		{
			new ServiceTypeInfo(typeof(WebServicesService), ServiceType.WebServices, new ServiceTypeInfo.TryCreateHttpServiceDelegate(WebServicesService.TryCreateWebServicesService)),
			new ServiceTypeInfo(typeof(MobileSyncService), ServiceType.MobileSync, new ServiceTypeInfo.TryCreateHttpServiceDelegate(MobileSyncService.TryCreateMobileSyncService)),
			new ServiceTypeInfo(typeof(OwaService), ServiceType.OutlookWebAccess, new ServiceTypeInfo.TryCreateHttpServiceDelegate(OwaService.TryCreateOwaService)),
			new ServiceTypeInfo(typeof(RpcHttpService), ServiceType.RpcHttp, new ServiceTypeInfo.TryCreateHttpServiceDelegate(RpcHttpService.TryCreateRpcHttpService)),
			new ServiceTypeInfo(typeof(MapiHttpService), ServiceType.MapiHttp, new ServiceTypeInfo.TryCreateHttpServiceDelegate(MapiHttpService.TryCreateMapiHttpService)),
			new ServiceTypeInfo(typeof(OabService), ServiceType.OfflineAddressBook, new ServiceTypeInfo.TryCreateHttpServiceDelegate(OabService.TryCreateOabService)),
			new ServiceTypeInfo(typeof(AvailabilityForeignConnectorService), ServiceType.AvailabilityForeignConnector, new ServiceTypeInfo.TryCreateHttpServiceDelegate(AvailabilityForeignConnectorService.TryCreateAvailabilityForeignConnectorService)),
			new ServiceTypeInfo(typeof(EcpService), ServiceType.ExchangeControlPanel, new ServiceTypeInfo.TryCreateHttpServiceDelegate(EcpService.TryCreateEcpService)),
			new ServiceTypeInfo(typeof(E12UnifiedMessagingService), ServiceType.UnifiedMessaging, new ServiceTypeInfo.TryCreateHttpServiceDelegate(E12UnifiedMessagingService.TryCreateUnifiedMessagingService)),
			new ServiceTypeInfo(typeof(Pop3Service), ServiceType.Pop3, new ServiceTypeInfo.TryCreateEmailTransportServiceDelegate(Pop3Service.TryCreatePop3Service)),
			new ServiceTypeInfo(typeof(Imap4Service), ServiceType.Imap4, new ServiceTypeInfo.TryCreateEmailTransportServiceDelegate(Imap4Service.TryCreateImap4Service)),
			new ServiceTypeInfo(typeof(SmtpService), ServiceType.Smtp, new ServiceTypeInfo.TryCreateSmtpServiceDelegate(SmtpService.TryCreateSmtpService)),
			new ServiceTypeInfo(typeof(HttpService), ServiceType.Invalid, new ServiceTypeInfo.TryCreateHttpServiceDelegate(HttpService.TryCreateHttpService)),
			new ServiceTypeInfo(typeof(EmailTransportService), ServiceType.Invalid, new ServiceTypeInfo.TryCreateEmailTransportServiceDelegate(EmailTransportService.TryCreateEmailTransportService))
		};

		private readonly Type type;

		private readonly ServiceType serviceType;

		private readonly Delegate tryCreateServiceDelegate;

		internal delegate bool TryCreateHttpServiceDelegate(MiniVirtualDirectory virtualDirectory, TopologyServerInfo serverInfo, Uri url, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service);

		internal delegate bool TryCreateEmailTransportServiceDelegate(MiniEmailTransport emailTransport, TopologyServerInfo serverInfo, ClientAccessType clientAccessType, AuthenticationMethod authenticationMethod, out Service service);

		internal delegate bool TryCreateSmtpServiceDelegate(MiniReceiveConnector smtpReceiveConnector, TopologyServerInfo serverInfo, ClientAccessType clientAccessType, out Service service);
	}
}
