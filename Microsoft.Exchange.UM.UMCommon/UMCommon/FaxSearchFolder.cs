using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class FaxSearchFolder : DefaultUMSearchFolder
	{
		internal FaxSearchFolder(MailboxSession itemStore) : base(itemStore)
		{
		}

		protected override DefaultFolderType DefaultFolderType
		{
			get
			{
				return DefaultFolderType.UMFax;
			}
		}
	}
}
