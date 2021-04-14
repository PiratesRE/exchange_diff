using System;
using Microsoft.Exchange.EDiscovery.Export;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class SourceMailbox : ISource
	{
		public SourceMailbox(string id, string name, string legacyDN, Uri serviceEndpoint, string queryFilter)
		{
			Util.ThrowIfNullOrEmpty(id, "id");
			Util.ThrowIfNullOrEmpty(name, "name");
			Util.ThrowIfNullOrEmpty(legacyDN, "legacyDN");
			Util.ThrowIfNull(serviceEndpoint, "serviceEndpoint");
			Util.ThrowIfNullOrEmpty(queryFilter, "queryFilter");
			this.Id = id;
			this.Name = name;
			this.LegacyExchangeDN = legacyDN;
			this.ServiceEndpoint = serviceEndpoint;
			this.SourceFilter = queryFilter;
		}

		public string Id { get; private set; }

		public string Name { get; private set; }

		public Uri ServiceEndpoint { get; internal set; }

		public string LegacyExchangeDN { get; private set; }

		public string SourceFilter { get; private set; }
	}
}
