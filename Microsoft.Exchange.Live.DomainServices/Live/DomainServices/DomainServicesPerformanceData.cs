using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.LatencyDetection;

namespace Microsoft.Exchange.Live.DomainServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DomainServicesPerformanceData
	{
		public static PerformanceDataProvider DomainServicesConnection
		{
			get
			{
				if (DomainServicesPerformanceData.domainServicesConnection == null)
				{
					DomainServicesPerformanceData.domainServicesConnection = new PerformanceDataProvider("DomainServices Connection");
				}
				return DomainServicesPerformanceData.domainServicesConnection;
			}
		}

		public static PerformanceDataProvider DomainServicesCall
		{
			get
			{
				if (DomainServicesPerformanceData.domainServicesCall == null)
				{
					DomainServicesPerformanceData.domainServicesCall = new PerformanceDataProvider("DomainServices Call");
				}
				return DomainServicesPerformanceData.domainServicesCall;
			}
		}

		public static IDisposable StartDomainServicesCallRequest()
		{
			return DomainServicesPerformanceData.DomainServicesCall.StartRequestTimer();
		}

		[ThreadStatic]
		private static PerformanceDataProvider domainServicesConnection;

		[ThreadStatic]
		private static PerformanceDataProvider domainServicesCall;
	}
}
