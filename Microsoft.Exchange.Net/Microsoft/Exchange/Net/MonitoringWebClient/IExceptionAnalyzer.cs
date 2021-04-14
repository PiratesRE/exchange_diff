using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal interface IExceptionAnalyzer
	{
		void Analyze(TestId currentTestStep, HttpWebRequestWrapper request, HttpWebResponseWrapper response, Exception exception, IResponseTracker responseTracker, Action<ScenarioException> trackingDelegate);

		RequestTarget GetRequestTarget(HttpWebRequestWrapper request);

		HttpWebResponseWrapperException VerifyResponse(HttpWebRequestWrapper request, HttpWebResponseWrapper response, CafeErrorPageValidationRules cafeErrorPageValidationRules);

		ScenarioException GetExceptionForScenarioTimeout(TimeSpan maxAllowedTime, TimeSpan totalTime, ResponseTrackerItem item);

		List<string> GetHostNames(RequestTarget requestTarget);
	}
}
