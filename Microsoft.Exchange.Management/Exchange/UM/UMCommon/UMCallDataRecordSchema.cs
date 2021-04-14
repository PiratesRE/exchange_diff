using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCommon
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class UMCallDataRecordSchema : UMCallReportBaseSchema
	{
		public static SimpleProviderPropertyDefinition Date = new SimpleProviderPropertyDefinition("Date", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.PersistDefaultValue, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static SimpleProviderPropertyDefinition Duration = UMCallReportBaseSchema.CreatePropertyDefinition("Duration", typeof(TimeSpan), TimeSpan.MinValue);

		public static SimpleProviderPropertyDefinition AudioCodec = UMCallReportBaseSchema.CreatePropertyDefinition("AudioCodec", typeof(string), string.Empty);

		public static SimpleProviderPropertyDefinition DialPlan = UMCallReportBaseSchema.CreatePropertyDefinition("DialPlan", typeof(string), string.Empty);

		public static SimpleProviderPropertyDefinition CallType = UMCallReportBaseSchema.CreatePropertyDefinition("CallType", typeof(string), string.Empty);

		public static SimpleProviderPropertyDefinition CallingNumber = UMCallReportBaseSchema.CreatePropertyDefinition("CallingNumber", typeof(string), string.Empty);

		public static SimpleProviderPropertyDefinition CalledNumber = UMCallReportBaseSchema.CreatePropertyDefinition("CalledNumber", typeof(string), string.Empty);

		public static SimpleProviderPropertyDefinition Gateway = UMCallReportBaseSchema.CreatePropertyDefinition("Gateway", typeof(string), string.Empty);

		public static SimpleProviderPropertyDefinition UserMailboxName = UMCallReportBaseSchema.CreatePropertyDefinition("UserMailboxName", typeof(string), string.Empty);
	}
}
