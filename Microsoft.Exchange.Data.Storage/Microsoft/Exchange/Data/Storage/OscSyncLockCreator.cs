using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OscSyncLockCreator
	{
		public OscSyncLockCreator(IMailboxSession session) : this(session, new XSOFactory())
		{
		}

		internal OscSyncLockCreator(IMailboxSession session, IXSOFactory xsoFactory)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(xsoFactory, "xsoFactory");
			if (session.MailboxOwner != null && session.MailboxOwner.MailboxInfo.IsArchive)
			{
				throw new ArgumentException("Archive mailbox is not supported.", "session");
			}
			this.session = session;
			this.xsoFactory = xsoFactory;
		}

		public StoreObjectId Create(string provider, string userId)
		{
			Util.ThrowOnNullOrEmptyArgument(provider, "provider");
			Util.ThrowOnNullOrEmptyArgument(userId, "userId");
			Guid guidFromName = OscProviderRegistry.GetGuidFromName(provider);
			string networkId;
			if (OscProviderRegistry.TryGetNetworkId(provider, out networkId))
			{
				return this.Create(provider, guidFromName, userId, networkId);
			}
			return this.Create(provider, guidFromName, userId, string.Empty);
		}

		private StoreObjectId Create(string provider, Guid providerGuid, string userId, string networkId)
		{
			new OscNetworkMoniker(providerGuid, networkId, userId);
			StoreObjectId result;
			try
			{
				result = this.SetOscSyncEnabledOnServer(this.FindExisting(provider, userId, networkId));
			}
			catch (ObjectNotFoundException)
			{
				OscSyncLockCreator.Tracer.TraceDebug((long)this.GetHashCode(), "SyncLock creator: SyncLock for provider '{0}' (GUID={1}), user id '{2}', and network id '{3}' doesn't exist.", new object[]
				{
					provider,
					providerGuid,
					userId,
					networkId
				});
				result = this.CreateWhenDoesntExist(provider, providerGuid, userId, networkId);
			}
			return result;
		}

		private StoreObjectId FindExisting(string provider, string userId, string networkId)
		{
			OscSyncLockCreator.Tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "SyncLock creator: looking for existing SyncLock for provider: {0}; user id: {1}; network id: {2}", provider, userId, networkId);
			return new OscSyncLockLocator(this.session, this.xsoFactory).Find(provider, userId, networkId);
		}

		private StoreObjectId SetOscSyncEnabledOnServer(StoreObjectId syncLockId)
		{
			StoreObjectId result;
			using (IMessageItem messageItem = this.xsoFactory.BindToMessage(this.session, syncLockId, OscSyncLockCreator.PropertiesToLoadFromSyncLock))
			{
				if (messageItem.GetValueOrDefault<bool>(MessageItemSchema.OscSyncEnabledOnServer, false))
				{
					OscSyncLockCreator.Tracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "SyncLock creator: OscSyncEnabledOnServer is already TRUE in SyncLock with id '{0}'", syncLockId);
					result = syncLockId;
				}
				else
				{
					OscSyncLockCreator.Tracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "SyncLock creator: setting OscSyncEnabledOnServer to TRUE and saving SyncLock with id '{0}'", syncLockId);
					messageItem.OpenAsReadWrite();
					messageItem[MessageItemSchema.OscSyncEnabledOnServer] = true;
					messageItem.Save(SaveMode.ResolveConflicts);
					result = syncLockId;
				}
			}
			return result;
		}

		private StoreObjectId CreateWhenDoesntExist(string provider, Guid providerGuid, string userId, string networkId)
		{
			StoreObjectId objectId;
			using (IMessageItem messageItem = this.xsoFactory.CreateMessageAssociated(this.session, this.session.GetDefaultFolderId(DefaultFolderType.Contacts)))
			{
				string text = "IPM.Microsoft.OSC.SyncLock." + new OscNetworkMoniker(providerGuid, networkId, userId).ToString();
				messageItem.ClassName = text;
				messageItem.Subject = text;
				messageItem[MessageItemSchema.OscSyncEnabledOnServer] = true;
				messageItem.Save(SaveMode.ResolveConflicts);
				messageItem.Load(new StorePropertyDefinition[]
				{
					ItemSchema.Id
				});
				OscSyncLockCreator.Tracer.TraceDebug((long)this.GetHashCode(), "SyncLock creator: successfully created SyncLock for provider '{0}' (GUID={1}), user id '{2}', and network id '{3}';  item class is '{4}'", new object[]
				{
					provider,
					providerGuid,
					userId,
					networkId,
					text
				});
				objectId = ((VersionedId)messageItem[ItemSchema.Id]).ObjectId;
			}
			return objectId;
		}

		private static readonly Trace Tracer = ExTraceGlobals.OutlookSocialConnectorInteropTracer;

		private static readonly PropertyDefinition[] PropertiesToLoadFromSyncLock = new StorePropertyDefinition[]
		{
			MessageItemSchema.OscSyncEnabledOnServer
		};

		private readonly IXSOFactory xsoFactory;

		private readonly IMailboxSession session;
	}
}
