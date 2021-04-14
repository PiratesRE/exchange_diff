using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SubscribeResultsExternal : SubscribeResults
	{
		internal SubscribeResultsExternal(SharingDataType dataType, string initiatorSmtpAddress, string initiatorName, string remoteFolderName, string remoteFolderId, StoreObjectId localFolderId, bool localFolderCreated, LocalizedString localFolderName) : base(dataType, initiatorSmtpAddress, initiatorName, remoteFolderName, localFolderId, localFolderCreated, localFolderName)
		{
			this.RemoteFolderId = remoteFolderId;
		}

		public string RemoteFolderId { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				", RemoteFolderId=",
				this.RemoteFolderId,
				", LocalFolderId=",
				base.LocalFolderId,
				", LocalFolderName=",
				base.LocalFolderName,
				", LocalFolderCreated=",
				base.LocalFolderCreated.ToString()
			});
		}
	}
}
