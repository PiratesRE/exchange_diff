using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal enum NavigationNodeType
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
