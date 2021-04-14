using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class ServicesContainer : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ServicesContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ServicesContainer.mostDerivedClass;
			}
		}

		internal static readonly string DefaultName = "Services";

		private static ServicesContainerSchema schema = ObjectSchema.GetInstance<ServicesContainerSchema>();

		private static string mostDerivedClass = "container";
	}
}
