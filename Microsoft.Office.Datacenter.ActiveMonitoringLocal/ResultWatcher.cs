using System;
using System.Diagnostics.Eventing.Reader;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class ResultWatcher<TResult> : CrimsonWatcher<TResult> where TResult : WorkItemResult, IPersistence, new()
	{
		public ResultWatcher() : this(null, null, true, null)
		{
		}

		public ResultWatcher(string serviceName, EventBookmark bookmark, bool isReadExistingEvents) : this(serviceName, bookmark, isReadExistingEvents, null)
		{
		}

		public ResultWatcher(string serviceName, EventBookmark bookmark, bool isReadExistingEvents, string channelName) : base(bookmark, isReadExistingEvents, channelName)
		{
			this.ServiceName = serviceName;
			base.QueryUserPropertyCondition = "(IsNotified=1)";
		}

		public string ServiceName { get; set; }

		public ResultWatcher<TResult>.ResultArrivedDelegate ResultArrivedCallback { get; set; }

		protected override void ResultArrivedHandler(TResult result)
		{
			if (this.ResultArrivedCallback != null)
			{
				this.ResultArrivedCallback(result);
			}
		}

		protected override string GetDefaultXPathQuery()
		{
			return CrimsonHelper.BuildXPathQueryString(base.ChannelName, this.ServiceName, base.QueryStartTime, base.QueryEndTime, base.QueryUserPropertyCondition);
		}

		private static readonly string resultClassName = typeof(TResult).Name;

		public delegate void ResultArrivedDelegate(TResult result);
	}
}
