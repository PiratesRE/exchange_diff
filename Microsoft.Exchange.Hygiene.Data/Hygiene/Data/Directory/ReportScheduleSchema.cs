using System;
using System.Data.SqlTypes;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ReportScheduleSchema
	{
		public static ProviderPropertyDefinition Id = ADObjectSchema.Id;

		public static ProviderPropertyDefinition TenantId = new HygienePropertyDefinition("OrganizationalUnitRoot", typeof(Guid));

		public static HygienePropertyDefinition Enabled = new HygienePropertyDefinition("Enabled", typeof(bool), true, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static ProviderPropertyDefinition ScheduleName = ADObjectSchema.RawName;

		public static HygienePropertyDefinition ScheduleFrequency = new HygienePropertyDefinition("ScheduleFrequency", typeof(ReportScheduleFrequencyType), ReportScheduleFrequencyType.Daily, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition ScheduleMask = new HygienePropertyDefinition("ScheduleMask", typeof(byte));

		public static HygienePropertyDefinition ScheduleStartTime = new HygienePropertyDefinition("ScheduleStartTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static ProviderPropertyDefinition ReportName = new HygienePropertyDefinition("ReportName", typeof(string));

		public static HygienePropertyDefinition ReportFormat = new HygienePropertyDefinition("ReportFormat", typeof(ReportFormatType), ReportFormatType.CSV, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static ProviderPropertyDefinition ReportSubject = new HygienePropertyDefinition("ReportSubject", typeof(string));

		public static ProviderPropertyDefinition ReportRecipients = new HygienePropertyDefinition("ReportRecipients", typeof(string));

		public static HygienePropertyDefinition ReportFilter = new HygienePropertyDefinition("ReportFilter", typeof(string));

		public static HygienePropertyDefinition ReportLanguage = new HygienePropertyDefinition("ReportLanguage", typeof(string));

		public static HygienePropertyDefinition BatchId = new HygienePropertyDefinition("BatchId", typeof(Guid?));

		public static HygienePropertyDefinition LastScheduleTime = new HygienePropertyDefinition("LastScheduleTime", typeof(DateTime), DateTime.Today, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition LastExecutionTime = new HygienePropertyDefinition("LastExecutionTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition LastExecutionStatus = new HygienePropertyDefinition("LastExecutionStatus", typeof(ReportExecutionStatusType), ReportExecutionStatusType.None, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition LastExecutionContextId = new HygienePropertyDefinition("LastExecutionContextId", typeof(Guid));

		public static HygienePropertyDefinition CurrentExecutionStatus = new HygienePropertyDefinition("CurrentExecutionStatus", typeof(ReportExecutionStatusType), ReportExecutionStatusType.None, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition CurrentExecutionContextId = new HygienePropertyDefinition("CurrentExecutionContextId", typeof(Guid));

		public static HygienePropertyDefinition ScheduleDateTimeFilterQueryProp = new HygienePropertyDefinition("ScheduleDateTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
