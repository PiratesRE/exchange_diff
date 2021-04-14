using System;

namespace Microsoft.Exchange.MailboxLoadBalance.Directory
{
	internal interface IDirectoryListener
	{
		void ObjectLoaded(DirectoryObject directoryObject);
	}
}
