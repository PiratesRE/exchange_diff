using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADBuiltinDomain : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ADBuiltinDomain.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADBuiltinDomain.MostDerivedClass;
			}
		}

		private static ADBuiltinDomainSchema schema = ObjectSchema.GetInstance<ADBuiltinDomainSchema>();

		internal static string MostDerivedClass = "builtinDomain";
	}
}
