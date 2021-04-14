using System;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharingSubscriptionManager : SharingSubscriptionManagerBase<SharingSubscriptionKey, SharingSubscriptionData>
	{
		public SharingSubscriptionManager(MailboxSession mailboxSession) : base(mailboxSession, "IPM.ExternalSharingSubscription", SharingSubscriptionManager.ItemProperties)
		{
		}

		public SharingSubscriptionData GetPrimary(string dataType, string sharerIdentity)
		{
			base.CheckDisposed("GetPrimary");
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, string, string>((long)this.GetHashCode(), "{0}: looking for subscription of {1} of {2}", base.MailboxSession.MailboxOwner, dataType, sharerIdentity);
			object[] array = this.FindPrimaryByDataType(dataType, sharerIdentity);
			if (array != null)
			{
				SharingSubscriptionData sharingSubscriptionData = this.CreateDataObjectFromItem(array);
				ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, SharingSubscriptionData>((long)this.GetHashCode(), "{0}: found subscription {1}", base.MailboxSession.MailboxOwner, sharingSubscriptionData);
				return sharingSubscriptionData;
			}
			ExTraceGlobals.SharingTracer.TraceDebug<IExchangePrincipal, string, string>((long)this.GetHashCode(), "{0}: No subscription was found: {1} of {2}.", base.MailboxSession.MailboxOwner, dataType, sharerIdentity);
			return null;
		}

		private object[] FindPrimaryByDataType(string dataType, string sharerIdentity)
		{
			return base.FindFirst((object[] properties) => StringComparer.OrdinalIgnoreCase.Equals(dataType, SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 4)) && StringComparer.OrdinalIgnoreCase.Equals(sharerIdentity, SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 5)) && SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyVal<bool>(properties, 9) == true);
		}

		protected override object[] FindFirstByKey(SharingSubscriptionKey subscriptionKey)
		{
			string sharerIdentity = subscriptionKey.SharerIdentity;
			string remoteFolderId = subscriptionKey.RemoteFolderId;
			return base.FindFirst((object[] properties) => StringComparer.OrdinalIgnoreCase.Equals(sharerIdentity, SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 5)) && StringComparer.OrdinalIgnoreCase.Equals(remoteFolderId, SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 7)));
		}

		protected override void StampItemFromDataObject(Item item, SharingSubscriptionData subscriptionData)
		{
			item[SharingSchema.ExternalSharingDataType] = subscriptionData.DataType.ExternalName;
			item[SharingSchema.ExternalSharingSharerIdentity] = subscriptionData.SharerIdentity;
			item[SharingSchema.ExternalSharingSharerName] = subscriptionData.SharerName;
			item[SharingSchema.ExternalSharingRemoteFolderId] = subscriptionData.RemoteFolderId;
			item[SharingSchema.ExternalSharingRemoteFolderName] = subscriptionData.RemoteFolderName;
			item[SharingSchema.ExternalSharingIsPrimary] = subscriptionData.IsPrimary;
			item[SharingSchema.ExternalSharingSharerIdentityFederationUri] = subscriptionData.SharerIdentityFederationUri.ToString();
			item[SharingSchema.ExternalSharingUrl] = subscriptionData.SharingUrl.ToString();
			item[SharingSchema.ExternalSharingLocalFolderId] = subscriptionData.LocalFolderId.GetBytes();
			item[SharingSchema.ExternalSharingSharingKey] = subscriptionData.SharingKey;
			item[SharingSchema.ExternalSharingSubscriberIdentity] = subscriptionData.SubscriberIdentity;
		}

		protected override SharingSubscriptionData CreateDataObjectFromItem(object[] properties)
		{
			VersionedId versionedId = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<VersionedId>(properties, 1);
			if (versionedId == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal>((long)this.GetHashCode(), "{0}: subscription is missing ID", base.MailboxSession.MailboxOwner);
				return null;
			}
			string text = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 4);
			if (string.IsNullOrEmpty(text))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingDataType", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text2 = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 5);
			if (string.IsNullOrEmpty(text2))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingSharerIdentity", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text3 = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 6);
			if (string.IsNullOrEmpty(text3))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingSharerName", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text4 = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 7);
			if (string.IsNullOrEmpty(text4))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingRemoteFolderId", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text5 = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 8);
			if (string.IsNullOrEmpty(text5))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingRemoteFolderName", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			bool? flag = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyVal<bool>(properties, 9);
			if (flag == null)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingIsPrimary", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			bool value = flag.Value;
			string text6 = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 10);
			if (string.IsNullOrEmpty(text6))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingSharerIdentityFederationUri", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			if (!Uri.IsWellFormedUriString(text6, UriKind.Absolute))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId, string>((long)this.GetHashCode(), "{0}: subscription {1} has invalid ExternalSharingSharerIdentityFederationUri: {2}", base.MailboxSession.MailboxOwner, versionedId, text6);
				return null;
			}
			string text7 = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 11);
			if (string.IsNullOrEmpty(text7))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingUrl", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			if (!Uri.IsWellFormedUriString(text7, UriKind.Absolute))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId, string>((long)this.GetHashCode(), "{0}: subscription {1} has invalid ExternalSharingUrl: {2}", base.MailboxSession.MailboxOwner, versionedId, text7);
				return null;
			}
			byte[] array = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<byte[]>(properties, 2);
			if (array == null || array.Length == 0)
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingLocalFolderId", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text8 = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 12);
			if (string.IsNullOrEmpty(text8))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingSharingKey", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			string text9 = SharingItemManagerBase<SharingSubscriptionData>.TryGetPropertyRef<string>(properties, 13);
			if (string.IsNullOrEmpty(text9))
			{
				ExTraceGlobals.SharingTracer.TraceError<IExchangePrincipal, VersionedId>((long)this.GetHashCode(), "{0}: subscription {1} is missing ExternalSharingSubscriberIdentity", base.MailboxSession.MailboxOwner, versionedId);
				return null;
			}
			return new SharingSubscriptionData(versionedId, text, text2, text3, text4, text5, value, new Uri(text6), new Uri(text7), StoreObjectId.Deserialize(array), text8, text9);
		}

		private static readonly PropertyDefinition[] ItemProperties = new PropertyDefinition[]
		{
			StoreObjectSchema.LastModifiedTime,
			SharingSchema.ExternalSharingDataType,
			SharingSchema.ExternalSharingSharerIdentity,
			SharingSchema.ExternalSharingSharerName,
			SharingSchema.ExternalSharingRemoteFolderId,
			SharingSchema.ExternalSharingRemoteFolderName,
			SharingSchema.ExternalSharingIsPrimary,
			SharingSchema.ExternalSharingSharerIdentityFederationUri,
			SharingSchema.ExternalSharingUrl,
			SharingSchema.ExternalSharingSharingKey,
			SharingSchema.ExternalSharingSubscriberIdentity
		};

		private enum ItemPropertiesIndex
		{
			LastModifiedTime = 3,
			ExternalSharingDataType,
			ExternalSharingSharerIdentity,
			ExternalSharingSharerName,
			ExternalSharingRemoteFolderId,
			ExternalSharingRemoteFolderName,
			ExternalSharingIsPrimary,
			ExternalSharingSharerIdentityFederationUri,
			ExternalSharingUrl,
			ExternalSharingSharingKey,
			ExternalSharingSubscriberIdentity
		}
	}
}
