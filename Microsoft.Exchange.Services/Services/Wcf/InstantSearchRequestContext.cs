using System;
using System.Threading;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class InstantSearchRequestContext
	{
		public InstantSearchRequestContext(PerformInstantSearchRequest request, IInstantSearchNotificationHandler notificationHandler, SearchPerfMarkerContainer perfMarkerContainer)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			if (notificationHandler == null)
			{
				throw new ArgumentNullException("notificationHandler");
			}
			if (perfMarkerContainer == null)
			{
				throw new ArgumentNullException("perfMarkerContainer");
			}
			this.request = request;
			this.notificationHandler = notificationHandler;
			this.PerfMarkers = perfMarkerContainer;
		}

		public IInstantSearchNotificationHandler NotificationHandler
		{
			get
			{
				return this.notificationHandler;
			}
		}

		public string[] SearchTerms { get; set; }

		public PerformInstantSearchRequest Request
		{
			get
			{
				return this.request;
			}
		}

		public SearchPerfMarkerContainer PerfMarkers { get; private set; }

		internal ManualResetEvent SearchResultsReceivedEvent { get; set; }

		public PerformInstantSearchResponse Response { get; set; }

		public string Error { get; set; }

		public bool ResponseSent { get; set; }

		private readonly PerformInstantSearchRequest request;

		private readonly IInstantSearchNotificationHandler notificationHandler;
	}
}
