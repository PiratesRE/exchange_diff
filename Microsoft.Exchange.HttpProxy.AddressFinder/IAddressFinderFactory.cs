using System;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal interface IAddressFinderFactory
	{
		IAddressFinder CreateAddressFinder(ProtocolType protocolType, string urlAbsolutePath);
	}
}
