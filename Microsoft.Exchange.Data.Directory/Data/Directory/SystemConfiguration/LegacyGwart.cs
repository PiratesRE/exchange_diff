using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class LegacyGwart : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return LegacyGwart.schema;
			}
		}

		public DateTime? GwartLastModified
		{
			get
			{
				return (DateTime?)this[LegacyGwartSchema.GwartLastModified];
			}
			internal set
			{
				this[LegacyGwartSchema.GwartLastModified] = value;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return LegacyGwart.mostDerivedClass;
			}
		}

		private static LegacyGwartSchema schema = ObjectSchema.GetInstance<LegacyGwartSchema>();

		private static string mostDerivedClass = "siteAddressing";

		public static readonly string DefaultName = "Legacy GWART";
	}
}
