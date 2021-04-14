using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class PeopleFilterGroupPriorities
	{
		public static bool IsMyContactsFolder(int sortGroupPriority)
		{
			return sortGroupPriority == 1;
		}

		public static bool ShouldBeIncludedInMyContactsFolder(int sortGroupPriority)
		{
			return sortGroupPriority > 1 && sortGroupPriority < 10;
		}

		public const int CurrentVersion = 2;

		public const int MyContactsFolder = 1;

		public const int DefaultContactsFolder = 2;

		public const int NetworkFolder = 3;

		public const int QuickContactsFolder = 4;

		public const int UserCreatedFolder = 5;

		public const int OtherFolder = 10;

		public const int GlobalAddressList = 1000;

		public const int AllRoomsAddressList = 1001;

		public const int AllUsersAddressList = 1002;

		public const int AllDistributionListsAddressList = 1003;

		public const int AllContactsAddressList = 1004;

		public const int AllModernGroupsAddressList = 1009;

		public const int OtherAddressList = 1010;

		public const int PublicFolders = 2000;
	}
}
