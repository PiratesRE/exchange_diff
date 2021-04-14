using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class AddressRewriteConfigContainer : ADContainer
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AddressRewriteConfigContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AddressRewriteConfigContainer.mostDerivedClass;
			}
		}

		private static AddressRewriteConfigContainerSchema schema = ObjectSchema.GetInstance<AddressRewriteConfigContainerSchema>();

		private static string mostDerivedClass = "msExchAddressRewriteConfiguration";
	}
}
