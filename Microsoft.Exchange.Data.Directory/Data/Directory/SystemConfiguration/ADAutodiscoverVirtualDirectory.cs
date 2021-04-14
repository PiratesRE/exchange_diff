using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class ADAutodiscoverVirtualDirectory : ADExchangeServiceVirtualDirectory
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADAutodiscoverVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADAutodiscoverVirtualDirectory.MostDerivedClass;
			}
		}

		private static readonly ADAutodiscoverVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADAutodiscoverVirtualDirectorySchema>();

		public static readonly string MostDerivedClass = "msExchAutodiscoverVirtualDirectory";
	}
}
