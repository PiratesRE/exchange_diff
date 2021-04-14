using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADAddressType : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADAddressType.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADAddressType.mostDerivedClass;
			}
		}

		public Version FileVersion
		{
			get
			{
				return (Version)this[ADAddressTypeSchema.FileVersion];
			}
		}

		public string ProxyGeneratorDll
		{
			get
			{
				return (string)this[ADAddressTypeSchema.ProxyGeneratorDll];
			}
		}

		private static ADAddressTypeSchema schema = ObjectSchema.GetInstance<ADAddressTypeSchema>();

		private static string mostDerivedClass = "AddrType";

		internal static readonly ADObjectId ContainerId = new ADObjectId("CN=Address-Types,CN=Addressing");
	}
}
