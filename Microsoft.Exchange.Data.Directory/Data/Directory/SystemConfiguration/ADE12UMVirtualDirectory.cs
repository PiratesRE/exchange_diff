using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADE12UMVirtualDirectory : ADExchangeServiceVirtualDirectory
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADE12UMVirtualDirectory.MostDerivedClass;
			}
		}

		public static readonly string MostDerivedClass = "msExchUMVirtualDirectory";
	}
}
