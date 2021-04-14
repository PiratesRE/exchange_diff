using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Management.Analysis
{
	internal interface IResultAccessor
	{
		void SetSource(AnalysisMember source);

		void SetParent(Result parent);

		void SetStartTime(ExDateTime startTime);

		void SetStopTime(ExDateTime stopTime);
	}
}
