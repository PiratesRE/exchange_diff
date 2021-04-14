using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PeriodicFailureEvent : ILogEvent
	{
		public PeriodicFailureEvent(string activityContextId, string tenantName, string requestUrl, Exception exceptionToFilterBy, string flightInfo)
		{
			this.activityContextId = activityContextId;
			this.tenantName = this.TranslateStringValueToLog(tenantName);
			this.requestUrl = this.TranslateStringValueToLog(requestUrl);
			this.exceptionToFilterBy = this.TranslateStringValueToLog(exceptionToFilterBy.GetTraceFormatter().ToString());
			this.flightInfo = this.TranslateStringValueToLog(flightInfo);
		}

		public string EventId
		{
			get
			{
				return "AppError";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("ACTID", this.activityContextId),
				new KeyValuePair<string, object>("TNAME", HttpUtility.UrlEncode(this.tenantName)),
				new KeyValuePair<string, object>("URL", HttpUtility.UrlEncode(this.requestUrl)),
				new KeyValuePair<string, object>("EX", HttpUtility.UrlEncode(this.exceptionToFilterBy)),
				new KeyValuePair<string, object>("INFO", HttpUtility.UrlEncode(this.flightInfo))
			};
		}

		private string TranslateStringValueToLog(string value)
		{
			if (value == null)
			{
				return "<null>";
			}
			if (value == string.Empty)
			{
				return "<empty>";
			}
			return value;
		}

		private const string eventId = "AppError";

		private readonly string activityContextId;

		private readonly string tenantName;

		private readonly string requestUrl;

		private readonly string exceptionToFilterBy;

		private readonly string flightInfo;
	}
}
