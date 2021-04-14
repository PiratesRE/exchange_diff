using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum FolderTreeDataType
	{
		Undefined = -1,
		NormalFolder,
		SmartFolder,
		SharedFolder,
		ShortcutFolder,
		Header,
		Department,
		GroupMember,
		GSCalendar,
		Max
	}
}
