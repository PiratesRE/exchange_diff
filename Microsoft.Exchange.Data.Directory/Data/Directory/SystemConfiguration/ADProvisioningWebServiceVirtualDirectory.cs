using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADProvisioningWebServiceVirtualDirectory : ADExchangeServiceVirtualDirectory
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADProvisioningWebServiceVirtualDirectory.MostDerivedClass;
			}
		}

		public static readonly string MostDerivedClass = "msExchVirtualDirectory";
	}
}
