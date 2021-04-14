using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class VoiceMailSearchFolder : DefaultUMSearchFolder
	{
		internal VoiceMailSearchFolder(MailboxSession itemStore) : base(itemStore)
		{
		}

		protected override DefaultFolderType DefaultFolderType
		{
			get
			{
				return DefaultFolderType.UMVoicemail;
			}
		}
	}
}
