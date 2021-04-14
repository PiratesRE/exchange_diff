using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublishingSubscriptionData : ISharingSubscriptionData<PublishingSubscriptionKey>, ISharingSubscriptionData
	{
		internal PublishingSubscriptionData(string dataType, Uri publishingUrl, string remoteFolderName, StoreObjectId localFolderId) : this(null, dataType, publishingUrl, remoteFolderName, localFolderId)
		{
		}

		internal PublishingSubscriptionData(VersionedId id, string dataType, Uri publishingUrl, string remoteFolderName, StoreObjectId localFolderId)
		{
			this.Id = id;
			this.DataType = SharingDataType.FromPublishName(dataType);
			this.PublishingUrl = publishingUrl;
			this.RemoteFolderName = remoteFolderName;
			this.LocalFolderId = localFolderId;
			this.Key = new PublishingSubscriptionKey(this.PublishingUrl);
		}

		public VersionedId Id { get; private set; }

		public SharingDataType DataType { get; private set; }

		public Uri PublishingUrl { get; private set; }

		public string RemoteFolderName { get; private set; }

		public StoreObjectId LocalFolderId { get; set; }

		public PublishingSubscriptionKey Key { get; private set; }

		public bool UrlNeedsExpansion
		{
			get
			{
				return this.PublishingUrl != null && this.PublishingUrl.Scheme == "holidays";
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"DataType=",
				this.DataType,
				", PublishingUrl=",
				this.PublishingUrl.OriginalString,
				", RemoteFolderName=",
				this.RemoteFolderName,
				", LocalFolderId=",
				this.LocalFolderId
			});
		}
	}
}
