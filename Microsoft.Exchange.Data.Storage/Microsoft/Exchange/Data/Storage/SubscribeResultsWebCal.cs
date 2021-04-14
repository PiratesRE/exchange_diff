using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SubscribeResultsWebCal : SubscribeResults
	{
		internal SubscribeResultsWebCal(SharingDataType dataType, string initiatorSmtpAddress, string initiatorName, string remoteFolderName, Uri url, StoreObjectId localFolderId, bool localFolderCreated, LocalizedString localFolderName) : base(dataType, initiatorSmtpAddress, initiatorName, remoteFolderName, localFolderId, localFolderCreated, localFolderName)
		{
			this.Url = url;
		}

		public Uri Url { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				", Url=",
				this.Url,
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
