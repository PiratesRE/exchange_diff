using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public enum LoggerType
	{
		LongOperation,
		RopSummary,
		FullTextIndex,
		DiagnosticQuery,
		ReferenceData,
		HeavyClientActivity,
		BreadCrumbs,
		SyntheticCounters
	}
}
