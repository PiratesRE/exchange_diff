using System;
using System.Data.SqlTypes;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class OnDemandQueryRequestSchema
	{
		public static ProviderPropertyDefinition Id = ADObjectSchema.Id;

		public static ProviderPropertyDefinition RequestId = ADObjectSchema.RawName;

		public static ProviderPropertyDefinition Container = DalHelper.ContainerProp;

		public static HygienePropertyDefinition QueryDefinition = new HygienePropertyDefinition("QueryDefinition", typeof(string));

		public static HygienePropertyDefinition RequestStatus = new HygienePropertyDefinition("RequestStatus", typeof(OnDemandQueryRequestStatus), OnDemandQueryRequestStatus.NotStarted, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition TenantId = new HygienePropertyDefinition("OrganizationalUnitRoot", typeof(Guid));

		public static HygienePropertyDefinition SubmissionTime = new HygienePropertyDefinition("SubmissionTime", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition BatchId = new HygienePropertyDefinition("ReportBatchId", typeof(Guid?));

		public static HygienePropertyDefinition Region = new HygienePropertyDefinition("Region", typeof(string));

		public static HygienePropertyDefinition QuerySubject = new HygienePropertyDefinition("QuerySubject", typeof(string));

		public static HygienePropertyDefinition QueryType = new HygienePropertyDefinition("QueryType", typeof(OnDemandQueryType), OnDemandQueryType.MTSummary, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition QueryPriority = new HygienePropertyDefinition("QueryPriority", typeof(OnDemandQueryPriority), OnDemandQueryPriority.Normal, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition CallerType = new HygienePropertyDefinition("CallerType", typeof(OnDemandQueryCallerType), OnDemandQueryCallerType.Customer, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition ResultSize = new HygienePropertyDefinition("ResultSize", typeof(long), 0L, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition MatchRowCounts = new HygienePropertyDefinition("MatchRowCounts", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition ResultRowCounts = new HygienePropertyDefinition("ResultRowCounts", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition ViewCounts = new HygienePropertyDefinition("ViewCounts", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition ResultUri = new HygienePropertyDefinition("ResultUri", typeof(string));

		public static HygienePropertyDefinition CosmosResultUri = new HygienePropertyDefinition("CosmosResultUri", typeof(string));

		public static HygienePropertyDefinition CosmosJobId = new HygienePropertyDefinition("CosmosJobId", typeof(Guid?));

		public static HygienePropertyDefinition InBatchQueryId = new HygienePropertyDefinition("InBatchQueryId", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition NotificationEmail = new HygienePropertyDefinition("NotificationEmail", typeof(string));

		public static HygienePropertyDefinition RetryCount = new HygienePropertyDefinition("RetryCount", typeof(int), 0, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static HygienePropertyDefinition ResultLocale = new HygienePropertyDefinition("ResultLocale", typeof(string));
	}
}
