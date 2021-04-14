using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class LostAndFound : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return LostAndFound.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return LostAndFound.MostDerivedClass;
			}
		}

		private static LostAndFoundSchema schema = ObjectSchema.GetInstance<LostAndFoundSchema>();

		internal static string MostDerivedClass = "lostandfound";
	}
}
