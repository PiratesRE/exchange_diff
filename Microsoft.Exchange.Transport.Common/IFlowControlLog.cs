using System;
using System.Collections.Generic;
using Microsoft.Exchange.Transport.LoggingCommon;

namespace Microsoft.Exchange.Transport
{
	internal interface IFlowControlLog
	{
		event Action<string> TrackSummary;

		void LogThrottle(ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData);

		void LogUnthrottle(ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, int impact, int observedValue, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData);

		void LogSummary(string sequenceNumber, ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, int observedValue, int impact, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData);

		void LogSummaryWarning(ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData);

		void LogWarning(ThrottlingResource resource, ThrottlingAction action, int threshold, TimeSpan thresholdInterval, ThrottlingScope scope, Guid externalOrganizationId, string sender, string recipient, string subject, ThrottlingSource source, bool loggingMode, IEnumerable<KeyValuePair<string, object>> extraData);

		void LogMaxLinesExceeded(string sequenceNumber, ThrottlingSource source, int threshold, int observedValue, IEnumerable<KeyValuePair<string, object>> extraData);
	}
}
