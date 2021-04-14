using System;

namespace Microsoft.Exchange.Search.Core.Abstraction
{
	internal interface IOperatorPerfCounter
	{
		void IncrementPerfcounter(OperatorPerformanceCounter performanceCounter);

		void IncrementPerfcounterBy(OperatorPerformanceCounter performanceCounter, long value);

		void DecrementPerfcounter(OperatorPerformanceCounter performanceCounter);
	}
}
