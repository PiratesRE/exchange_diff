using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal interface IResponseTracker
	{
		List<ResponseTrackerItem> Items { get; }

		ResponseTrackerItem TrackRequest(TestId testId, RequestTarget requestTarget, HttpWebRequestWrapper request);

		void TrackSentRequest(ResponseTrackerItem item, HttpWebRequestWrapper request);

		void TrackResolvedRequest(HttpWebRequestWrapper request);

		void TrackResponse(ResponseTrackerItem item, HttpWebResponseWrapper response);

		void TrackFailedResponse(HttpWebResponseWrapper response, ScenarioException exception);

		void TrackFailedTcpConnection(HttpWebRequestWrapper request, Exception exception);

		void TrackFailedRequest(TestId testId, RequestTarget requestTarget, HttpWebRequestWrapper request, Exception exception);

		void TrackItemCausingScenarioTimeout(ResponseTrackerItem item, Exception exception);
	}
}
