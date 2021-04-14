using System;
using System.Collections.Generic;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model
{
	internal class FanoutParameters
	{
		public GroupId GroupId { get; set; }

		public SearchSource Source { get; set; }

		internal class FanoutState
		{
			public IExchangeProxy Proxy { get; set; }

			public IList<FanoutParameters> Parameters { get; set; }
		}
	}
}
