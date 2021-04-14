using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMCallSummaryReportSchema : UMCallReportBaseSchema
	{
		private static SimpleProviderPropertyDefinition CreateUlongPropertyDefinition(string propertyName)
		{
			return new SimpleProviderPropertyDefinition(propertyName, ExchangeObjectVersion.Exchange2010, typeof(ulong), PropertyDefinitionFlags.PersistDefaultValue, 0UL, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
		}

		public static SimpleProviderPropertyDefinition AutoAttendant = UMCallSummaryReportSchema.CreateUlongPropertyDefinition("AutoAttendant");

		public static SimpleProviderPropertyDefinition FailedOrRejectedCalls = UMCallSummaryReportSchema.CreateUlongPropertyDefinition("FailedOrRejectedCalls");

		public static SimpleProviderPropertyDefinition Fax = UMCallSummaryReportSchema.CreateUlongPropertyDefinition("Fax");

		public static SimpleProviderPropertyDefinition MissedCalls = UMCallSummaryReportSchema.CreateUlongPropertyDefinition("MissedCalls");

		public static SimpleProviderPropertyDefinition OtherCalls = UMCallSummaryReportSchema.CreateUlongPropertyDefinition("OtherCalls");

		public static SimpleProviderPropertyDefinition Outbound = UMCallSummaryReportSchema.CreateUlongPropertyDefinition("Outbound");

		public static SimpleProviderPropertyDefinition SubscriberAccess = UMCallSummaryReportSchema.CreateUlongPropertyDefinition("SubscriberAccess");

		public static SimpleProviderPropertyDefinition VoiceMessages = UMCallSummaryReportSchema.CreateUlongPropertyDefinition("VoiceMessages");

		public static SimpleProviderPropertyDefinition TotalCalls = UMCallSummaryReportSchema.CreateUlongPropertyDefinition("TotalCalls");

		public static SimpleProviderPropertyDefinition TotalAudioQualityCallsSampled = UMCallSummaryReportSchema.CreateUlongPropertyDefinition("TotalAudioQualityCallsSampled");

		public static SimpleProviderPropertyDefinition Date = UMCallReportBaseSchema.CreatePropertyDefinition("Date", typeof(string), string.Empty);

		public static SimpleProviderPropertyDefinition UMDialPlanName = UMCallReportBaseSchema.CreatePropertyDefinition("UMDialPlanName", typeof(string), string.Empty);

		public static SimpleProviderPropertyDefinition UMIPGatewayName = UMCallReportBaseSchema.CreatePropertyDefinition("UMIPGatewayName", typeof(string), string.Empty);
	}
}
