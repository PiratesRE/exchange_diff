using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.QueueDigest
{
	internal abstract class GetQueueDigestImpl
	{
		public abstract void ResolveForForest();

		public abstract void ResolveDag(DatabaseAvailabilityGroup dag);

		public abstract void ResolveAdSite(ADSite adSite);

		public abstract void ResolveServer(Server server);

		public abstract void ProcessRecord();
	}
}
