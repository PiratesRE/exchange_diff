using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MRSProxyRequestContext
	{
		public MRSProxyRequestContext()
		{
			this.Id = Guid.NewGuid();
			this.HttpHeaders = new Dictionary<string, string>();
			MRSProxyRequestContext.activeProxyClientContexts[this.Id] = this;
		}

		public Guid Id { get; private set; }

		public Dictionary<string, string> HttpHeaders { get; private set; }

		public Uri EndpointUri { get; set; }

		public Cookie BackendCookie { get; set; }

		public static MRSProxyRequestContext Find(Guid id)
		{
			MRSProxyRequestContext result;
			if (MRSProxyRequestContext.activeProxyClientContexts.TryGetValue(id, out result))
			{
				return result;
			}
			return null;
		}

		public void Unregister()
		{
			MRSProxyRequestContext mrsproxyRequestContext;
			MRSProxyRequestContext.activeProxyClientContexts.TryRemove(this.Id, out mrsproxyRequestContext);
		}

		private static readonly ConcurrentDictionary<Guid, MRSProxyRequestContext> activeProxyClientContexts = new ConcurrentDictionary<Guid, MRSProxyRequestContext>();
	}
}
