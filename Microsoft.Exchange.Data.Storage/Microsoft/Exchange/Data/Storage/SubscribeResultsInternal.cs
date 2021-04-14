using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SubscribeResultsInternal : SubscribeResults
	{
		internal SubscribeResultsInternal(SharingDataType dataType, string initiatorSmtpAddress, string initiatorName, string remoteFolderName, StoreObjectId remoteFolderId, byte[] remoteMailboxId) : base(dataType, initiatorSmtpAddress, initiatorName, remoteFolderName, null, false, null)
		{
			this.RemoteFolderId = remoteFolderId;
			this.RemoteMailboxId = remoteMailboxId;
		}

		public StoreObjectId RemoteFolderId { get; private set; }

		public byte[] RemoteMailboxId { get; private set; }

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.ToString(),
				", RemoteFolderId=",
				this.RemoteFolderId,
				", RemoteMailboxId=",
				HexConverter.ByteArrayToHexString(this.RemoteMailboxId)
			});
		}
	}
}
