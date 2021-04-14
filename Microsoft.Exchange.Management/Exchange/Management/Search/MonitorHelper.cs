using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Monitoring;

namespace Microsoft.Exchange.Management.Search
{
	internal class MonitorHelper
	{
		internal string MOMEventSource
		{
			get
			{
				return "MSExchange Monitoring ExchangeSearch";
			}
		}

		internal void AddMonitoringEvent(SearchTestResult searchTestResult, LocalizedString localizedString)
		{
			searchTestResult.DetailEvents.Add(new MonitoringEvent(this.MOMEventSource, 1002, EventTypeEnumeration.Information, localizedString, searchTestResult.Database));
			this.PushMessage(localizedString);
		}

		internal void PushMessage(LocalizedString msg)
		{
			this.messageStack.Push(msg);
		}

		internal LocalizedString PopMessage()
		{
			return this.messageStack.Pop();
		}

		internal LocalizedString PeekMessage()
		{
			return this.messageStack.Peek();
		}

		internal bool HasMessage()
		{
			return this.messageStack.Count > 0;
		}

		private const string momEventSource = "MSExchange Monitoring ExchangeSearch";

		private Stack<LocalizedString> messageStack = new Stack<LocalizedString>(5);
	}
}
