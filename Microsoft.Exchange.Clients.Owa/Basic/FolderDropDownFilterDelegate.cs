using System;
using Microsoft.Exchange.Clients.Owa.Core.Controls;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa.Basic
{
	internal delegate bool FolderDropDownFilterDelegate(FolderList folderList, StoreObjectId folderId);
}
