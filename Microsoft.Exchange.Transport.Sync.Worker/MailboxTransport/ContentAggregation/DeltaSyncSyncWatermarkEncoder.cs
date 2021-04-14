using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols.DeltaSync;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DeltaSyncSyncWatermarkEncoder
	{
		public string Encode(string folderSyncKey, string emailSyncKey)
		{
			SyncUtilities.ThrowIfArgumentNull("folderSyncKey", folderSyncKey);
			SyncUtilities.ThrowIfArgumentNull("emailSyncKey", emailSyncKey);
			return string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
			{
				this.elementEncoder.Encode(folderSyncKey),
				this.elementEncoder.Encode(emailSyncKey)
			});
		}

		public void Decode(string toDecode, SyncLogSession syncLogSession, out string decodedFolderSyncKey, out string decodedEmailSyncKey)
		{
			SyncUtilities.ThrowIfArgumentNull("toDecode", toDecode);
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			if (toDecode.Length == 0)
			{
				this.SetToDefaultSyncKeys(out decodedFolderSyncKey, out decodedEmailSyncKey);
				syncLogSession.LogDebugging((TSLID)510UL, "DeltaSyncSyncWatermarkEncoder.Decode: toDecode was empty string. Using defaults.", new object[0]);
				return;
			}
			int offset;
			if (!this.elementEncoder.TryDecodeElementFrom(toDecode, 0, out decodedFolderSyncKey, out offset))
			{
				this.SetToDefaultSyncKeys(out decodedFolderSyncKey, out decodedEmailSyncKey);
				syncLogSession.LogError((TSLID)511UL, "DeltaSyncSyncWatermarkEncoder.Decode: unable to parse folder sync key. Using defaults.", new object[0]);
				return;
			}
			int num;
			if (!this.elementEncoder.TryDecodeElementFrom(toDecode, offset, out decodedEmailSyncKey, out num))
			{
				this.SetToDefaultSyncKeys(out decodedFolderSyncKey, out decodedEmailSyncKey);
				syncLogSession.LogError((TSLID)512UL, "DeltaSyncSyncWatermarkEncoder.Decode: unable to parse email sync key. Using defaults.", new object[0]);
			}
		}

		private void SetToDefaultSyncKeys(out string decodedFolderSyncKey, out string decodedEmailSyncKey)
		{
			decodedFolderSyncKey = DeltaSyncCommon.DefaultSyncKey;
			decodedEmailSyncKey = DeltaSyncCommon.DefaultSyncKey;
		}

		private readonly SyncWatermarkElementEncoder elementEncoder = new SyncWatermarkElementEncoder();
	}
}
