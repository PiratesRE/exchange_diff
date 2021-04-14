using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal sealed class AddressFinderFactory : IAddressFinderFactory
	{
		static AddressFinderFactory()
		{
			RoutingHintAddressFinder routingHintAddressFinder = new RoutingHintAddressFinder();
			LogonUserAddressFinder logonUserAddressFinder = new LogonUserAddressFinder();
			AddressFinderFactory.DefaultAddressFinder = new CompositeAddressFinder(new IAddressFinder[]
			{
				routingHintAddressFinder,
				logonUserAddressFinder
			});
			AddressFinderFactory.EasAddressFinder = AddressFinderFactory.DefaultAddressFinder;
			AddressFinderFactory.MapiAddressFinder = new CompositeAddressFinder(new IAddressFinder[]
			{
				new MapiAddressFinder(),
				AddressFinderFactory.DefaultAddressFinder
			});
			AddressFinderFactory.RpcHttpAddressFinder = new CompositeAddressFinder(new IAddressFinder[]
			{
				new RpcHttpAddressFinder(),
				logonUserAddressFinder
			});
			AddressFinderFactory.EmptyAddressFinder = new CompositeAddressFinder(new IAddressFinder[0]);
			CompositeAddressFinder compositeAddressFinder = new CompositeAddressFinder(new IAddressFinder[]
			{
				new ExplicitLogonAddressFinder(),
				AddressFinderFactory.DefaultAddressFinder
			});
			AddressFinderFactory.EwsAddressFinder = new CompositeAddressFinder(new IAddressFinder[]
			{
				new EwsAddressFinder(),
				compositeAddressFinder
			});
			AddressFinderFactory.EwsODataAddressFinder = new CompositeAddressFinder(new IAddressFinder[]
			{
				new EwsODataAddressFinder(),
				AddressFinderFactory.EwsAddressFinder
			});
			AddressFinderFactory.EwsUserPhotoAddressFinder = new CompositeAddressFinder(new IAddressFinder[]
			{
				new EwsUserPhotoAddressFinder(),
				AddressFinderFactory.EwsAddressFinder
			});
		}

		private AddressFinderFactory()
		{
		}

		public static IAddressFinderFactory GetInstance()
		{
			if (AddressFinderFactory.instance == null)
			{
				lock (AddressFinderFactory.addressFinderFactoryLock)
				{
					if (AddressFinderFactory.instance == null)
					{
						AddressFinderFactory.instance = new AddressFinderFactory();
					}
				}
			}
			return AddressFinderFactory.instance;
		}

		IAddressFinder IAddressFinderFactory.CreateAddressFinder(ProtocolType protocolType, string urlAbsolutePath)
		{
			ArgumentValidator.ThrowIfNull("urlAbsolutePath", urlAbsolutePath);
			switch (protocolType)
			{
			case ProtocolType.Eas:
				return AddressFinderFactory.EasAddressFinder;
			case ProtocolType.Ecp:
				break;
			case ProtocolType.Ews:
				if (ProtocolHelper.IsEwsGetUserPhotoRequest(urlAbsolutePath))
				{
					return AddressFinderFactory.EwsUserPhotoAddressFinder;
				}
				if (ProtocolHelper.IsEwsODataRequest(urlAbsolutePath))
				{
					return AddressFinderFactory.EwsODataAddressFinder;
				}
				if (ProtocolHelper.IsWsSecurityRequest(urlAbsolutePath) || ProtocolHelper.IsPartnerAuthRequest(urlAbsolutePath) || ProtocolHelper.IsX509CertAuthRequest(urlAbsolutePath))
				{
					return AddressFinderFactory.EmptyAddressFinder;
				}
				return AddressFinderFactory.EwsAddressFinder;
			default:
				if (protocolType == ProtocolType.RpcHttp)
				{
					return AddressFinderFactory.RpcHttpAddressFinder;
				}
				if (protocolType == ProtocolType.Mapi)
				{
					return AddressFinderFactory.MapiAddressFinder;
				}
				break;
			}
			return AddressFinderFactory.DefaultAddressFinder;
		}

		private static readonly IAddressFinder DefaultAddressFinder;

		private static readonly IAddressFinder EasAddressFinder;

		private static readonly IAddressFinder EmptyAddressFinder;

		private static readonly IAddressFinder EwsAddressFinder;

		private static readonly IAddressFinder EwsODataAddressFinder;

		private static readonly IAddressFinder EwsUserPhotoAddressFinder;

		private static readonly IAddressFinder MapiAddressFinder;

		private static readonly IAddressFinder RpcHttpAddressFinder;

		private static volatile AddressFinderFactory instance;

		private static object addressFinderFactoryLock = new object();
	}
}
