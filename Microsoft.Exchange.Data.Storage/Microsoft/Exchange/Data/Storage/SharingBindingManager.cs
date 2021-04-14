using System;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharingBindingManager : SharingItemManagerBase<SharingBindingData>
	{
		public SharingBindingManager(MailboxSession mailboxSession)
		{
			this.mailboxSession = mailboxSession;
		}

		public SharingBindingData GetSharingBindingDataInFolder(StoreId folderId)
		{
			Util.ThrowOnNullArgument(folderId, "folderId");
			object[] rawBindingQueryInFolder = this.GetRawBindingQueryInFolder(folderId);
			if (rawBindingQueryInFolder != null)
			{
				return this.CreateDataObjectFromItem(rawBindingQueryInFolder);
			}
			return null;
		}

		public void CreateOrUpdateSharingBinding(SharingBindingData bindingData)
		{
			Util.ThrowOnNullArgument(bindingData, "bindingData");
			Util.ThrowOnNullArgument(bindingData.LocalFolderId, "bindingData.LocalFolderId");
			object[] rawBindingQueryInFolder = this.GetRawBindingQueryInFolder(bindingData.LocalFolderId);
			if (rawBindingQueryInFolder != null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingBindingData>((long)this.GetHashCode(), "{0}: updating binding message {1}", this.mailboxSession.MailboxOwner, bindingData);
				SharingBindingData sharingBindingData = this.CreateDataObjectFromItem(rawBindingQueryInFolder);
				if (SharingBindingData.EqualContent(sharingBindingData, bindingData))
				{
					return;
				}
				using (Item item = MessageItem.Bind(this.mailboxSession, sharingBindingData.Id, SharingBindingManager.QueryBindingColumns))
				{
					this.SaveBindingMessage(item, bindingData);
					return;
				}
			}
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingBindingData>((long)this.GetHashCode(), "{0}: creating binding message {1}", this.mailboxSession.MailboxOwner, bindingData);
			using (Item item2 = MessageItem.CreateAssociated(this.mailboxSession, bindingData.LocalFolderId))
			{
				item2[BindingItemSchema.SharingInstanceGuid] = Guid.NewGuid();
				this.SaveBindingMessage(item2, bindingData);
			}
		}

		protected override void StampItemFromDataObject(Item item, SharingBindingData bindingData)
		{
			item[BindingItemSchema.SharingInitiatorName] = bindingData.InitiatorName;
			item[BindingItemSchema.SharingInitiatorSmtp] = bindingData.InitiatorSmtpAddress;
			item[BindingItemSchema.SharingRemoteName] = bindingData.RemoteFolderName;
			item[BindingItemSchema.SharingRemoteFolderId] = bindingData.RemoteFolderId;
			item[BindingItemSchema.SharingLocalName] = bindingData.LocalFolderName;
			if (bindingData.LastSyncTimeUtc != null)
			{
				item[BindingItemSchema.SharingLastSync] = new ExDateTime(ExTimeZone.UtcTimeZone, bindingData.LastSyncTimeUtc.Value.ToUniversalTime());
			}
			item[BindingItemSchema.SharingRemoteType] = (item[BindingItemSchema.SharingLocalType] = bindingData.DataType.ContainerClass);
			StoreObjectId storeObjectId = StoreId.GetStoreObjectId(bindingData.LocalFolderId);
			bindingData.LocalFolderId = storeObjectId;
			item[BindingItemSchema.SharingLocalUid] = storeObjectId.ToHexEntryId();
			item[BindingItemSchema.SharingLocalFolderEwsId] = StoreId.StoreIdToEwsId(this.mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, storeObjectId);
			int num = 11;
			if (bindingData.IsDefaultFolderShared)
			{
				num |= 131072;
			}
			item[BindingItemSchema.SharingFlavor] = num;
			item[BindingItemSchema.SharingRoamLog] = SharingContextRoamLog.UnroamedBinding;
			item[BindingItemSchema.SharingStatus] = SharingContextStatus.Configured;
			item[BindingItemSchema.SharingProviderGuid] = SharingBindingManager.ExternalSharingProviderGuid;
			item[BindingItemSchema.SharingProviderName] = "Microsoft Exchange";
			item[BindingItemSchema.SharingProviderUrl] = "http://www.microsoft.com/exchange/";
			item[StoreObjectSchema.ItemClass] = "IPM.Sharing.Binding.In";
		}

		protected override SharingBindingData CreateDataObjectFromItem(object[] properties)
		{
			VersionedId versionedId = SharingItemManagerBase<SharingBindingData>.TryGetPropertyRef<VersionedId>(properties, 1);
			if (versionedId == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal>((long)this.GetHashCode(), "{0}: Binding is missing ID", this.mailboxSession.MailboxOwner);
				return null;
			}
			string text = SharingItemManagerBase<SharingBindingData>.TryGetPropertyRef<string>(properties, 12);
			if (text == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Binding {1} is missing sharingLocalType", this.mailboxSession.MailboxOwner, versionedId);
				return null;
			}
			SharingDataType sharingDataType = SharingDataType.FromContainerClass(text);
			if (sharingDataType == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId, string>((long)this.GetHashCode(), "{0}: Binding {1} has invalid sharingLocalType: {2}", this.mailboxSession.MailboxOwner, versionedId, text);
				return null;
			}
			string text2 = SharingItemManagerBase<SharingBindingData>.TryGetPropertyRef<string>(properties, 8);
			if (text2 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Binding {1} is missing initiatorName", this.mailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text3 = SharingItemManagerBase<SharingBindingData>.TryGetPropertyRef<string>(properties, 9);
			if (text3 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Binding {1} is missing initiatorSmtpAddress", this.mailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text4 = SharingItemManagerBase<SharingBindingData>.TryGetPropertyRef<string>(properties, 7);
			if (text4 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Binding {1} is missing remoteFolderName", this.mailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text5 = SharingItemManagerBase<SharingBindingData>.TryGetPropertyRef<string>(properties, 18);
			if (text5 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Binding {1} is missing remoteFolderId", this.mailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text6 = SharingItemManagerBase<SharingBindingData>.TryGetPropertyRef<string>(properties, 10);
			if (text6 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Binding {1} is missing localFolderName", this.mailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text7 = SharingItemManagerBase<SharingBindingData>.TryGetPropertyRef<string>(properties, 11);
			if (text7 == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Binding {1} is missing localFolderUid", this.mailboxSession.MailboxOwner, versionedId);
				return null;
			}
			StoreObjectId localFolderId;
			try
			{
				localFolderId = StoreObjectId.FromHexEntryId(text7, sharingDataType.StoreObjectType);
			}
			catch (CorruptDataException)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Binding {1} has invalid localFolderUid", this.mailboxSession.MailboxOwner, versionedId);
				return null;
			}
			int? num = SharingItemManagerBase<SharingBindingData>.TryGetPropertyVal<int>(properties, 3);
			if (num == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Binding {1} is missing flavor", this.mailboxSession.MailboxOwner, versionedId);
				return null;
			}
			bool isDefaultFolderShared = 0 != (num.Value & 131072);
			DateTime? lastSyncTimeUtc = null;
			ExDateTime? exDateTime = SharingItemManagerBase<SharingBindingData>.TryGetPropertyVal<ExDateTime>(properties, 15);
			if (exDateTime == null)
			{
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: Binding {1} is missing lastSyncTime", this.mailboxSession.MailboxOwner, versionedId);
			}
			else
			{
				lastSyncTimeUtc = new DateTime?((DateTime)exDateTime.Value.ToUtc());
			}
			return new SharingBindingData(versionedId, sharingDataType, text2, text3, text4, text5, text6, localFolderId, isDefaultFolderShared, lastSyncTimeUtc);
		}

		private void SaveBindingMessage(Item item, SharingBindingData bindingData)
		{
			this.StampItemFromDataObject(item, bindingData);
			item.Save(SaveMode.NoConflictResolution);
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingBindingData>((long)this.GetHashCode(), "{0}: Saved binding message: {1}", this.mailboxSession.MailboxOwner, bindingData);
		}

		private object[] GetRawBindingQueryInFolder(StoreId folderId)
		{
			using (Folder folder = Folder.Bind(this.mailboxSession, folderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, null, SharingBindingManager.QueryBindingColumns))
				{
					if (queryResult.SeekToCondition(SeekReference.OriginBeginning, SharingBindingManager.SharingProviderGuidFilter))
					{
						object[][] rows = queryResult.GetRows(2);
						if (rows.Length > 0)
						{
							if (rows.Length > 1)
							{
								ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, StoreId>((long)this.GetHashCode(), "{0}: There should be only one external sharing binding message associated with a folder, but more than one were found in folder: {1}", this.mailboxSession.MailboxOwner, folderId);
							}
							else
							{
								object[] array = rows[0];
								string x = SharingItemManagerBase<SharingBindingData>.TryGetPropertyRef<string>(array, 2);
								if (StringComparer.OrdinalIgnoreCase.Equals(x, "IPM.Sharing.Binding.In"))
								{
									return array;
								}
							}
						}
						else
						{
							ExTraceGlobals.SharingTracer.TraceDebug((long)this.GetHashCode(), "The total Items length retrieved from the Query is not greater than zero");
						}
					}
					else
					{
						ExTraceGlobals.SharingTracer.TraceDebug<SeekReference, ComparisonFilter>((long)this.GetHashCode(), "Query.SeekToCondition returned false for SeekReference.ForwardFromBeginning {0} and SharingProviderGuidFilter {1}", SeekReference.OriginBeginning, SharingBindingManager.SharingProviderGuidFilter);
					}
				}
			}
			return null;
		}

		private const string ExternalSharingProviderName = "Microsoft Exchange";

		private const string ExternalSharingProviderUrl = "http://www.microsoft.com/exchange/";

		public static readonly Guid ExternalSharingProviderGuid = new Guid("{0006F0C0-0000-0000-C000-000000000046}");

		private static readonly PropertyDefinition[] QueryBindingColumns = new PropertyDefinition[]
		{
			BindingItemSchema.SharingProviderGuid,
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			BindingItemSchema.SharingFlavor,
			BindingItemSchema.SharingStatus,
			BindingItemSchema.SharingProviderName,
			BindingItemSchema.SharingProviderUrl,
			BindingItemSchema.SharingRemoteName,
			BindingItemSchema.SharingInitiatorName,
			BindingItemSchema.SharingInitiatorSmtp,
			BindingItemSchema.SharingLocalName,
			BindingItemSchema.SharingLocalUid,
			BindingItemSchema.SharingLocalType,
			BindingItemSchema.SharingInstanceGuid,
			BindingItemSchema.SharingRemoteType,
			BindingItemSchema.SharingLastSync,
			BindingItemSchema.SharingRoamLog,
			BindingItemSchema.SharingBindingEid,
			BindingItemSchema.SharingRemoteFolderId,
			BindingItemSchema.SharingLocalFolderEwsId
		};

		private static readonly ComparisonFilter SharingProviderGuidFilter = new ComparisonFilter(ComparisonOperator.Equal, BindingItemSchema.SharingProviderGuid, SharingBindingManager.ExternalSharingProviderGuid);

		private MailboxSession mailboxSession;

		private enum QueryBindingColumnsIndex
		{
			SharingProviderGuid,
			Id,
			ItemClass,
			SharingFlavor,
			SharingStatus,
			SharingProviderName,
			SharingProviderUrl,
			SharingRemoteName,
			SharingInitiatorName,
			SharingInitiatorSmtp,
			SharingLocalName,
			SharingLocalUid,
			SharingLocalType,
			SharingInstanceGuid,
			SharingRemoteType,
			SharingLastSync,
			SharingRoamLog,
			SharingBindingEid,
			SharingRemoteFolderId,
			SharingLocalFolderEwsId,
			Count
		}
	}
}
