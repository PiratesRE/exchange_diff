using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class ClientExceptionLoggerEvent : ILogEvent
	{
		internal ClientExceptionLoggerEvent(ClientWatson watson)
		{
			this.watson = watson;
			this.datapointProperties = new Dictionary<string, object>
			{
				{
					"TIME",
					watson.Time
				},
				{
					"MSG",
					HttpUtility.UrlEncode(watson.Message)
				},
				{
					"URL",
					watson.Url
				},
				{
					"LOC",
					HttpUtility.UrlEncode(watson.Location)
				},
				{
					"REQID",
					watson.RequestId
				},
				{
					"ST",
					HttpUtility.UrlEncode(watson.StackTrace)
				}
			};
		}

		public string EventId
		{
			get
			{
				return this.watson.ErrorType;
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return this.datapointProperties;
		}

		private readonly ClientWatson watson;

		private Dictionary<string, object> datapointProperties;
	}
}
