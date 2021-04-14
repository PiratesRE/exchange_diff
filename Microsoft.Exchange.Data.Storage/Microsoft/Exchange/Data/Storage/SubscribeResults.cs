using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SubscribeResults
	{
		public SubscribeResults(SharingDataType dataType, string initiatorSmtpAddress, string initiatorName, string remoteFolderName, StoreObjectId localFolderId, bool localFolderCreated, string localFolderName)
		{
			this.DataType = dataType;
			this.InitiatorSmtpAddress = initiatorSmtpAddress;
			this.InitiatorName = initiatorName;
			this.RemoteFolderName = remoteFolderName;
			this.LocalFolderId = localFolderId;
			this.LocalFolderCreated = localFolderCreated;
			this.LocalFolderName = localFolderName;
		}

		public SharingDataType DataType { get; private set; }

		public string InitiatorSmtpAddress { get; private set; }

		public string InitiatorName { get; private set; }

		public string RemoteFolderName { get; private set; }

		public StoreObjectId LocalFolderId { get; private set; }

		public string LocalFolderName { get; private set; }

		public bool LocalFolderCreated { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"DataType=",
				this.DataType,
				", InitiatorSmtpAddress=",
				this.InitiatorSmtpAddress,
				", InitiatorName=",
				this.InitiatorName,
				", RemoteFolderName=",
				this.RemoteFolderName
			});
		}
	}
}
