using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharingSubscriptionData : ISharingSubscriptionData<SharingSubscriptionKey>, ISharingSubscriptionData
	{
		internal SharingSubscriptionData(string dataType, string sharerIdentity, string sharerName, string remoteFolderId, string remoteFolderName, bool isPrimary, Uri sharerIdentityFederationUri, Uri sharingUrl, StoreObjectId localFolderId, string sharingKey, string subscriberIdentity) : this(null, dataType, sharerIdentity, sharerName, remoteFolderId, remoteFolderName, isPrimary, sharerIdentityFederationUri, sharingUrl, localFolderId, sharingKey, subscriberIdentity)
		{
		}

		internal SharingSubscriptionData(VersionedId id, string dataType, string sharerIdentity, string sharerName, string remoteFolderId, string remoteFolderName, bool isPrimary, Uri sharerIdentityFederationUri, Uri sharingUrl, StoreObjectId localFolderId, string sharingKey, string subscriberIdentity)
		{
			this.Id = id;
			this.DataType = SharingDataType.FromExternalName(dataType);
			this.SharerIdentity = sharerIdentity;
			this.SharerName = sharerName;
			this.RemoteFolderId = remoteFolderId;
			this.RemoteFolderName = remoteFolderName;
			this.IsPrimary = isPrimary;
			this.SharerIdentityFederationUri = sharerIdentityFederationUri;
			this.SharingUrl = sharingUrl;
			this.LocalFolderId = localFolderId;
			this.SharingKey = sharingKey;
			this.SubscriberIdentity = subscriberIdentity;
			this.Key = new SharingSubscriptionKey(this.SharerIdentity, this.RemoteFolderId);
		}

		public VersionedId Id { get; private set; }

		public SharingDataType DataType { get; private set; }

		public string SharerIdentity { get; private set; }

		public string SharerName { get; private set; }

		public string RemoteFolderId { get; private set; }

		public string RemoteFolderName { get; private set; }

		public bool IsPrimary { get; private set; }

		public string SharingKey { get; private set; }

		public string SubscriberIdentity { get; private set; }

		public Uri SharerIdentityFederationUri { get; set; }

		public Uri SharingUrl { get; set; }

		public StoreObjectId LocalFolderId { get; set; }

		public SharingSubscriptionKey Key { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"DataType=",
				this.DataType,
				", SharerIdentity=",
				this.SharerIdentity,
				", SharerName=",
				this.SharerName,
				", RemoteFolderId=",
				this.RemoteFolderId,
				", RemoteFolderName=",
				this.RemoteFolderName,
				", IsPrimary=",
				this.IsPrimary.ToString(),
				", SharerIdentityFederationUri=",
				this.SharerIdentityFederationUri,
				", SharingUrl=",
				this.SharingUrl,
				", LocalFolderId=",
				this.LocalFolderId,
				", SharingKey=",
				this.SharingKey,
				", SubscriberIdentity=",
				this.SubscriberIdentity
			});
		}

		internal void CopyFrom(SharingSubscriptionData other)
		{
			if (this.Id == null || other.Id != null || !this.Key.Equals(other.Key))
			{
				throw new InvalidOperationException();
			}
			this.DataType = other.DataType;
			this.SharerName = other.SharerName;
			this.RemoteFolderName = other.RemoteFolderName;
			this.IsPrimary = other.IsPrimary;
			this.SharerIdentityFederationUri = other.SharerIdentityFederationUri;
			this.SharingUrl = other.SharingUrl;
			this.SharingKey = other.SharingKey;
			this.SubscriberIdentity = other.SubscriberIdentity;
		}
	}
}
