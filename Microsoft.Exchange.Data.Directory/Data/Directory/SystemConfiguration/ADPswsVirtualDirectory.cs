using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADPswsVirtualDirectory : ADPowerShellCommonVirtualDirectory
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADPswsVirtualDirectory.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		private static readonly ADPswsVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADPswsVirtualDirectorySchema>();
	}
}
