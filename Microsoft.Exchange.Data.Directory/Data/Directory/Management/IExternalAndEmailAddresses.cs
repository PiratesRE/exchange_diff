using System;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal interface IExternalAndEmailAddresses
	{
		ProxyAddress ExternalEmailAddress { get; set; }

		ProxyAddressCollection EmailAddresses { get; set; }
	}
}
