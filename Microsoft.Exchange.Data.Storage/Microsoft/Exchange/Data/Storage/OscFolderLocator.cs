using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OscFolderLocator
	{
		public OscFolderLocator(IMailboxSession session) : this(session, new XSOFactory())
		{
		}

		internal OscFolderLocator(IMailboxSession session, IXSOFactory xsoFactory)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(xsoFactory, "xsoFactory");
			this.session = session;
			this.xsoFactory = xsoFactory;
		}

		public StoreObjectId Find(string provider, string userId)
		{
			Util.ThrowOnNullOrEmptyArgument(provider, "provider");
			Util.ThrowOnNullOrEmptyArgument(userId, "userId");
			string networkId;
			if (OscProviderRegistry.TryGetNetworkId(provider, out networkId))
			{
				return this.Find(provider, userId, networkId);
			}
			return this.Find(provider, userId, string.Empty);
		}

		internal StoreObjectId Find(string provider, string userId, string networkId)
		{
			Util.ThrowOnNullOrEmptyArgument(provider, "provider");
			Util.ThrowOnNullOrEmptyArgument(userId, "userId");
			Guid guid;
			if (!OscProviderRegistry.TryGetGuidFromName(provider, out guid))
			{
				OscFolderLocator.Tracer.TraceError<string>((long)this.GetHashCode(), "Folder locator: cannot find folder for unknown provider: {0}", provider);
				throw new ObjectNotFoundException(ServerStrings.UnknownOscProvider(provider));
			}
			OscNetworkMoniker arg = new OscNetworkMoniker(guid, networkId, userId);
			foreach (IStorePropertyBag storePropertyBag in new OscProviderCandidateFolderEnumerator(this.session, guid, this.xsoFactory))
			{
				VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(FolderSchema.Id, null);
				if (valueOrDefault == null)
				{
					OscFolderLocator.Tracer.TraceError((long)this.GetHashCode(), "Folder locator: skipping folder with invalid id");
				}
				else
				{
					OscFolderLocator.Tracer.TraceDebug<string, OscNetworkMoniker, StoreObjectId>((long)this.GetHashCode(), "Folder locator: looking for ContactSync FAI for provider '{0}' and moniker '{1}' in folder '{2}'", provider, arg, valueOrDefault.ObjectId);
					foreach (IStorePropertyBag item in new OscContactSyncFAIEnumerator(this.session, valueOrDefault.ObjectId, this.xsoFactory))
					{
						foreach (OscNetworkMoniker oscNetworkMoniker in new OscFolderContactSourcesEnumerator(item))
						{
							OscFolderLocator.Tracer.TraceDebug<OscNetworkMoniker, StoreObjectId>((long)this.GetHashCode(), "Folder locator: found network moniker '{0}' in folder '{1}'", oscNetworkMoniker, valueOrDefault.ObjectId);
							if (arg.Equals(oscNetworkMoniker))
							{
								OscFolderLocator.Tracer.TraceDebug<string, StoreObjectId>((long)this.GetHashCode(), "Folder locator: found folder for provider '{0}'.  Folder id is '{1}'", provider, valueOrDefault.ObjectId);
								return valueOrDefault.ObjectId;
							}
						}
					}
				}
			}
			OscFolderLocator.Tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Folder locator: folder for provider: {0}; user id: {1}; network id: {2}; not found.", provider, userId, networkId);
			throw new ObjectNotFoundException(ServerStrings.OscFolderForProviderNotFound(provider));
		}

		private static readonly Trace Tracer = ExTraceGlobals.OutlookSocialConnectorInteropTracer;

		private readonly IXSOFactory xsoFactory;

		private readonly IMailboxSession session;
	}
}
