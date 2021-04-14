using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DatabaseAvailabilityGroupContainer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return DatabaseAvailabilityGroupContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DatabaseAvailabilityGroupContainer.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public static readonly string DefaultName = "Database Availability Groups";

		private static DatabaseAvailabilityGroupContainerSchema schema = ObjectSchema.GetInstance<DatabaseAvailabilityGroupContainerSchema>();

		private static string mostDerivedClass = "msExchMDBAvailabilityGroupContainer";
	}
}
