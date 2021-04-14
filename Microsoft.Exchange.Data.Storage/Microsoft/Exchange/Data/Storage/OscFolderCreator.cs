using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OscFolderCreator
	{
		public OscFolderCreator(MailboxSession session) : this(session, new XSOFactory())
		{
		}

		internal OscFolderCreator(MailboxSession session, IXSOFactory xsoFactory)
		{
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(xsoFactory, "xsoFactory");
			if (session.MailboxOwner != null && session.MailboxOwner.MailboxInfo.IsArchive)
			{
				throw new ArgumentException("Archive mailbox is not supported.", "session");
			}
			this.session = session;
			this.xsoFactory = xsoFactory;
			this.networkToFolderMap = new Dictionary<OscNetworkMoniker, OscFolderCreateResult>();
		}

		public OscFolderCreateResult Create(string provider, string userId)
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

		internal OscFolderCreateResult Create(string provider, Guid providerGuid, string userId, string networkId)
		{
			OscNetworkMoniker oscNetworkMoniker = new OscNetworkMoniker(providerGuid, networkId, userId);
			OscFolderCreateResult oscFolderCreateResult;
			if (this.networkToFolderMap.TryGetValue(oscNetworkMoniker, out oscFolderCreateResult))
			{
				OscFolderCreator.Tracer.TraceDebug((long)this.GetHashCode(), "Folder creator: folder for provider '{0}' (GUID={1}), user id '{2}', and network id '{3}' found in cache.  Folder is '{4}'.", new object[]
				{
					provider,
					providerGuid,
					userId,
					networkId,
					oscFolderCreateResult
				});
				return oscFolderCreateResult;
			}
			OscFolderCreateResult result;
			try
			{
				StoreObjectId storeObjectId = this.FindExistingFolder(provider, userId, networkId);
				OscFolderCreator.Tracer.TraceDebug((long)this.GetHashCode(), "Folder creator: folder for provider '{0}' (GUID={1}), user id '{2}', and network id '{3}' ALREADY exists with id '{4}'.", new object[]
				{
					provider,
					providerGuid,
					userId,
					networkId,
					storeObjectId
				});
				result = this.AddFolderToCache(new OscFolderCreateResult(storeObjectId, false), oscNetworkMoniker);
			}
			catch (ObjectNotFoundException)
			{
				OscFolderCreator.Tracer.TraceDebug((long)this.GetHashCode(), "Folder creator: folder for provider '{0}' (GUID={1}), user id '{2}', and network id '{3}' doesn't exist.", new object[]
				{
					provider,
					providerGuid,
					userId,
					networkId
				});
				StoreObjectId folderId = this.CreateWhenFolderDoesntExist(provider, providerGuid, userId, networkId);
				result = this.AddFolderToCache(new OscFolderCreateResult(folderId, true), oscNetworkMoniker);
			}
			return result;
		}

		private StoreObjectId FindExistingFolder(string provider, string userId, string networkId)
		{
			OscFolderCreator.Tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "Folder creator: looking for existing folder for provider: {0}; user id: {1}; network id: {2}", provider, userId, networkId);
			return new OscFolderLocator(this.session, this.xsoFactory).Find(provider, userId, networkId);
		}

		private StoreObjectId CreateWhenFolderDoesntExist(string provider, Guid providerGuid, string userId, string networkId)
		{
			StoreObjectId parentFolderId = this.GetParentFolderId(providerGuid);
			foreach (string text in new OscFolderDisplayNameGenerator(providerGuid, 10))
			{
				try
				{
					return this.CreateFolderWithDisplayName(text, parentFolderId, provider, providerGuid, userId, networkId);
				}
				catch (ObjectExistedException)
				{
					OscFolderCreator.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Folder creator: caught ObjectExistedException when attempting to create folder with display name '{0}' for provider '{1}'", text, provider);
				}
			}
			throw new CannotCreateOscFolderBecauseOfConflictException(provider);
		}

		private StoreObjectId CreateFolderWithDisplayName(string displayName, StoreObjectId parentFolder, string provider, Guid providerGuid, string userId, string networkId)
		{
			StoreObjectId objectId;
			using (Folder folder = Folder.Create(this.session, parentFolder, StoreObjectType.ContactsFolder, displayName, CreateMode.CreateNew))
			{
				folder[FolderSchema.IsPeopleConnectSyncFolder] = true;
				folder[FolderSchema.ExtendedFolderFlags] = ExtendedFolderFlags.ReadOnly;
				folder.Save();
				folder.Load(new PropertyDefinition[]
				{
					FolderSchema.Id
				});
				this.CreateContactSyncFAI(folder.Id.ObjectId, displayName, new OscNetworkMoniker(providerGuid, networkId, userId));
				this.session.ContactFolders.MyContactFolders.Add(folder.Id.ObjectId);
				OscFolderCreator.Tracer.TraceDebug((long)this.GetHashCode(), "Folder creator: successfully created folder with display name '{0}' and id '{1}' for provider '{2}' (GUID={3}), user id '{4}', and network id '{5}'", new object[]
				{
					displayName,
					folder.Id.ObjectId,
					provider,
					providerGuid,
					userId,
					networkId
				});
				objectId = folder.Id.ObjectId;
			}
			return objectId;
		}

		private void CreateContactSyncFAI(StoreObjectId folderId, string folderDisplayName, OscNetworkMoniker networkMoniker)
		{
			using (MessageItem messageItem = MessageItem.CreateAssociated(this.session, folderId))
			{
				messageItem.ClassName = "IPM.Microsoft.OSC.ContactSync";
				messageItem[MessageItemSchema.OscContactSources] = new string[]
				{
					networkMoniker.ToString()
				};
				messageItem.Save(SaveMode.ResolveConflicts);
				OscFolderCreator.Tracer.TraceDebug<string, StoreObjectId, OscNetworkMoniker>((long)this.GetHashCode(), "Folder creator: successfully created ContactSync FAI in folder='{0}' (ID='{1}') with network moniker='{2}'", folderDisplayName, folderId, networkMoniker);
			}
		}

		private StoreObjectId GetParentFolderId(Guid provider)
		{
			return this.session.GetDefaultFolderId(OscProviderRegistry.GetParentFolder(provider));
		}

		private OscFolderCreateResult AddFolderToCache(OscFolderCreateResult folder, OscNetworkMoniker networkMoniker)
		{
			this.networkToFolderMap[networkMoniker] = folder;
			return folder;
		}

		private static readonly Trace Tracer = ExTraceGlobals.OutlookSocialConnectorInteropTracer;

		private readonly IXSOFactory xsoFactory;

		private readonly MailboxSession session;

		private readonly Dictionary<OscNetworkMoniker, OscFolderCreateResult> networkToFolderMap;
	}
}
