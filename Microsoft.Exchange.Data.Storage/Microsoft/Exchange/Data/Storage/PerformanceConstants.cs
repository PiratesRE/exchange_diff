using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class PerformanceConstants
	{
		[Conditional("PerfInvestigation")]
		internal static void AssertEfficientPattern(bool condition, string formatString, params object[] parameters)
		{
			ExAssert.RetailAssert(condition, formatString, parameters);
		}

		internal const int MemoryPropertyBagDefaultCapacity = 8;

		internal const int DependencyEstimatesCapacity = 256;

		internal const int DefaultDependencyEstimate = 128;

		internal const int AverageDisplayNameLengthSwag = 15;

		internal const int AverageEmailAddressLengthSwag = 15;

		internal const int AverageRoutingTypeLengthSwag = 4;

		internal const int AverageServerNameSwag = 20;

		internal const int CorrelationQueryFetchCount = 50;

		internal const int MalformedSingleAppointmentFetchCount = 50;

		internal const int GoodSingleAppointmentFetchCount = 100;

		internal const int StreamDataCacheSize = 4096;

		internal const int StreamBodyBufferSize = 65536;

		internal const int StreamAttachmentBufferSize = 131072;
	}
}
