using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal interface ICountAndRatePairCounter
	{
		void AddValue(long value);

		void UpdateAverage();

		void GetDiagnosticInfo(XElement parent);
	}
}
