using System;
using System.Collections.Concurrent;

namespace Microsoft.Exchange.Clients.Owa2.Server.Web
{
	public class SlabManifestCollectionFactory
	{
		public static SlabManifestCollection GetInstance(string owaVersion)
		{
			if (!SlabManifestCollectionFactory.slabManifestCollections.ContainsKey(owaVersion))
			{
				SlabManifestCollection value = SlabManifestCollection.Create(owaVersion);
				SlabManifestCollectionFactory.slabManifestCollections.TryAdd(owaVersion, value);
			}
			return SlabManifestCollectionFactory.slabManifestCollections[owaVersion];
		}

		private static ConcurrentDictionary<string, SlabManifestCollection> slabManifestCollections = new ConcurrentDictionary<string, SlabManifestCollection>();
	}
}
