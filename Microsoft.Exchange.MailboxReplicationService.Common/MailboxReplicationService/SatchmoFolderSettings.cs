using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	internal class SatchmoFolderSettings : ItemPropertiesBase
	{
		public int FolderTypeInt;

		public DateTime LastWrite;

		public int? LatestFolderChangeSequenceNumber;

		public int ViewTypeInt;

		public bool IsArchive;

		public byte[] UserPreference;

		public int? UIDValidity;

		public int ImapStateInt;

		public int? LatestCreateSequenceNumberInFolder;

		public DateTime? LastDateReceivedHeaderProcessedForIMAP;

		public Guid? LastMessageIdHeaderProcessedForIMAP;

		public int? LastCreateSequenceNumberInFolderProcessedForIMAP;
	}
}
