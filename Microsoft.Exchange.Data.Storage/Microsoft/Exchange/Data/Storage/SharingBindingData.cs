using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharingBindingData
	{
		public static SharingBindingData CreateSharingBindingData(SharingDataType dataType, string initiatorName, string initiatorSmtpAddress, string remoteFolderName, string remoteFolderId, string localFolderName, StoreObjectId localFolderId, bool isDefaultFolderShared)
		{
			Util.ThrowOnNullArgument(dataType, "dataType");
			Util.ThrowOnNullOrEmptyArgument(initiatorName, "initiatorName");
			if (!SmtpAddress.IsValidSmtpAddress(initiatorSmtpAddress))
			{
				throw new ArgumentException("initiatorSmtpAddress");
			}
			Util.ThrowOnNullOrEmptyArgument(remoteFolderName, "remoteFolderName");
			Util.ThrowOnNullOrEmptyArgument(remoteFolderId, "remoteFolderId");
			Util.ThrowOnNullOrEmptyArgument(localFolderName, "localFolderName");
			Util.ThrowOnNullArgument(localFolderId, "localFolderId");
			return new SharingBindingData(null, dataType, initiatorName, initiatorSmtpAddress, remoteFolderName, remoteFolderId, localFolderName, localFolderId, isDefaultFolderShared, null);
		}

		internal SharingBindingData(VersionedId itemId, SharingDataType dataType, string initiatorName, string initiatorSmtpAddress, string remoteFolderName, string remoteFolderId, string localFolderName, StoreObjectId localFolderId, bool isDefaultFolderShared, DateTime? lastSyncTimeUtc)
		{
			this.Id = itemId;
			this.DataType = dataType;
			this.InitiatorName = initiatorName;
			this.InitiatorSmtpAddress = initiatorSmtpAddress;
			this.RemoteFolderName = remoteFolderName;
			this.RemoteFolderId = remoteFolderId;
			this.LocalFolderName = localFolderName;
			this.LocalFolderId = localFolderId;
			this.IsDefaultFolderShared = isDefaultFolderShared;
			this.LastSyncTimeUtc = lastSyncTimeUtc;
		}

		internal static bool EqualContent(SharingBindingData left, SharingBindingData right)
		{
			return left != null && right != null && left.InitiatorName == right.InitiatorName && left.InitiatorSmtpAddress == right.InitiatorSmtpAddress && left.IsDefaultFolderShared == right.IsDefaultFolderShared && object.Equals(left.LastSyncTimeUtc, right.LastSyncTimeUtc) && object.Equals(left.LocalFolderId, right.LocalFolderId) && left.LocalFolderName == right.LocalFolderName && left.RemoteFolderName == right.RemoteFolderName && left.RemoteFolderId == right.RemoteFolderId;
		}

		public VersionedId Id { get; private set; }

		public SharingDataType DataType { get; private set; }

		public string InitiatorName { get; private set; }

		public string InitiatorSmtpAddress { get; private set; }

		public string RemoteFolderName { get; private set; }

		public string RemoteFolderId { get; private set; }

		public string LocalFolderName { get; private set; }

		public StoreObjectId LocalFolderId { get; set; }

		public bool IsDefaultFolderShared { get; private set; }

		public DateTime? LastSyncTimeUtc { get; set; }

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"InitiatorName=",
				this.InitiatorName,
				", InitiatorSmtpAddress=",
				this.InitiatorSmtpAddress,
				", RemoteFolderName=",
				this.RemoteFolderName,
				", RemoteFolderId=",
				this.RemoteFolderId,
				", LocalFolderName=",
				this.LocalFolderName,
				", LocalFolderId=",
				this.LocalFolderId.ToBase64String(),
				", IsDefaultFolderShared=",
				this.IsDefaultFolderShared.ToString(),
				", LastSyncTimeUtc=",
				this.LastSyncTimeUtc.ToString()
			});
		}
	}
}
