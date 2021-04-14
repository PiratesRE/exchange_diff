using System;

namespace Microsoft.Forefront.Reporting.OnDemandQuery
{
	internal enum OnDemandQueryLogFields
	{
		Timestamp,
		Event,
		RequestID,
		TenantID,
		Region,
		SubmissionDateTime,
		QueryType,
		QueryPriority,
		CallerType,
		QueryDefinition,
		BatchID,
		InBatchQueryID,
		CosmosJobID,
		MatchRowCounts,
		ResultRowCounts,
		ResultSize,
		ViewCounts,
		RrtryCount,
		ResultLocale,
		ErrorMsg
	}
}
