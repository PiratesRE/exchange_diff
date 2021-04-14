using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OscSyncLockLocator
	{
		public OscSyncLockLocator(IMailboxSession session) : this(session, new XSOFactory())
		{
		}

		internal OscSyncLockLocator(IMailboxSession session, IXSOFactory xsoFactory)
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
			Guid providerGuid;
			if (!OscProviderRegistry.TryGetGuidFromName(provider, out providerGuid))
			{
				OscSyncLockLocator.Tracer.TraceError<string>((long)this.GetHashCode(), "SyncLock locator: cannot find for unknown provider: {0}", provider);
				throw new ObjectNotFoundException(ServerStrings.UnknownOscProvider(provider));
			}
			OscNetworkMoniker oscNetworkMoniker = new OscNetworkMoniker(providerGuid, networkId, userId);
			foreach (IStorePropertyBag storePropertyBag in new OscSyncLockEnumerator(this.session, this.session.GetDefaultFolderId(DefaultFolderType.Contacts), this.xsoFactory))
			{
				VersionedId valueOrDefault = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
				if (valueOrDefault == null)
				{
					OscSyncLockLocator.Tracer.TraceError((long)this.GetHashCode(), "SyncLock locator: skipping SyncLock because its id is missing.");
				}
				else
				{
					string text = this.ExtractNetworkMonikerFromSyncLock(storePropertyBag);
					OscSyncLockLocator.Tracer.TraceDebug<StoreObjectId, string>((long)this.GetHashCode(), "SyncLock locator: SyncLock with id '{0}' has network moniker '{1}'", valueOrDefault.ObjectId, text);
					if (oscNetworkMoniker.Equals(text))
					{
						OscSyncLockLocator.Tracer.TraceDebug((long)this.GetHashCode(), "SyncLock locator: found for provider '{0}', user id '{1}', and network id '{2}'.  Id is '{3}'", new object[]
						{
							provider,
							userId,
							networkId,
							valueOrDefault.ObjectId
						});
						return valueOrDefault.ObjectId;
					}
				}
			}
			OscSyncLockLocator.Tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "SyncLock locator: for provider '{0}', user id '{1}', and network id '{2}' not found.", provider, userId, networkId);
			throw new ObjectNotFoundException(ServerStrings.OscSyncLockNotFound(provider, userId, networkId));
		}

		private string ExtractNetworkMonikerFromSyncLock(IStorePropertyBag syncLock)
		{
			string text = syncLock.TryGetProperty(StoreObjectSchema.ItemClass) as string;
			if (string.IsNullOrEmpty(text) || text.Length < "IPM.Microsoft.OSC.SyncLock.".Length)
			{
				return string.Empty;
			}
			return text.Substring("IPM.Microsoft.OSC.SyncLock.".Length);
		}

		private static readonly Trace Tracer = ExTraceGlobals.OutlookSocialConnectorInteropTracer;

		private readonly IXSOFactory xsoFactory;

		private readonly IMailboxSession session;
	}
}
