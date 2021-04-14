using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class StampGroupContainer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return StampGroupContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return StampGroupContainer.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public static readonly string DefaultName = "Stamp Groups";

		private static StampGroupContainerSchema schema = ObjectSchema.GetInstance<StampGroupContainerSchema>();

		private static string mostDerivedClass = "msExchMDBAvailabilityGroupContainer";
	}
}
