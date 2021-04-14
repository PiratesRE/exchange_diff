using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ManifestItemReadUnread : ManifestChangeBase
	{
		internal ManifestItemReadUnread(byte[] idsetReadUnread, bool isRead)
		{
			this.idsetReadUnread = idsetReadUnread;
			this.isRead = isRead;
		}

		public byte[] IdsetReadUnread
		{
			get
			{
				return this.idsetReadUnread;
			}
		}

		public bool IsRead
		{
			get
			{
				return this.isRead;
			}
		}

		private readonly byte[] idsetReadUnread;

		private readonly bool isRead;
	}
}
