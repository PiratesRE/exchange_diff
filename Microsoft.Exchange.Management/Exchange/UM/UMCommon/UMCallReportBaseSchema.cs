using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class UMCallReportBaseSchema : SimpleProviderObjectSchema
	{
		protected static SimpleProviderPropertyDefinition CreatePropertyDefinition(string propertyName, Type propertyType, object defaultValue)
		{
			return new SimpleProviderPropertyDefinition(propertyName, ExchangeObjectVersion.Exchange2010, propertyType, PropertyDefinitionFlags.None, defaultValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		public static SimpleProviderPropertyDefinition NMOS = UMCallReportBaseSchema.CreatePropertyDefinition("NMOS", typeof(float?), null);

		public static SimpleProviderPropertyDefinition NMOSDegradation = UMCallReportBaseSchema.CreatePropertyDefinition("NMOSDegradation", typeof(float?), null);

		public static SimpleProviderPropertyDefinition PercentPacketLoss = UMCallReportBaseSchema.CreatePropertyDefinition("PercentPacketLoss", typeof(float?), null);

		public static SimpleProviderPropertyDefinition Jitter = UMCallReportBaseSchema.CreatePropertyDefinition("Jitter", typeof(float?), null);

		public static SimpleProviderPropertyDefinition RoundTripMilliseconds = UMCallReportBaseSchema.CreatePropertyDefinition("RoundTripMilliseconds", typeof(float?), null);

		public static SimpleProviderPropertyDefinition BurstLossDurationMilliseconds = UMCallReportBaseSchema.CreatePropertyDefinition("BurstLossDurationMilliseconds", typeof(float?), null);
	}
}
