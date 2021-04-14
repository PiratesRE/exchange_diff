using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PublicFolderContentMailboxInfo
	{
		public PublicFolderContentMailboxInfo(string contentMailboxInfo)
		{
			this.isValid = (GuidHelper.TryParseGuid(contentMailboxInfo, out this.mailboxGuid) && this.mailboxGuid != Guid.Empty);
			this.contentMailboxInfo = contentMailboxInfo;
		}

		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public override string ToString()
		{
			return this.contentMailboxInfo;
		}

		private readonly Guid mailboxGuid;

		private readonly bool isValid;

		private readonly string contentMailboxInfo;
	}
}
