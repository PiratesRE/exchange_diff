using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(ConfigScopes.Global)]
	[Serializable]
	public class SitesContainer : ADNonExchangeObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return SitesContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return SitesContainer.mostDerivedClass;
			}
		}

		internal static readonly string DefaultName = "Sites";

		private static SitesContainerSchema schema = ObjectSchema.GetInstance<SitesContainerSchema>();

		private static string mostDerivedClass = "sitesContainer";
	}
}
