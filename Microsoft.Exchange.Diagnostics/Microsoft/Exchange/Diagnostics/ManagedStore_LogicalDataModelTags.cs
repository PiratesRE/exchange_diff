using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_LogicalDataModelTags
	{
		public const int Folder = 0;

		public const int Events = 1;

		public const int ConversationsSummary = 2;

		public const int ConversationsDetailed = 3;

		public const int SearchFolderSearchCriteria = 4;

		public const int SearchFolderPopulation = 5;

		public const int Categorizations = 6;

		public const int GetViewsProperties = 7;

		public const int DatabaseSizeCheck = 8;

		public const int SearchFolderAgeOut = 9;

		public const int ReadEvents = 10;

		public const int EventCounterBounds = 11;

		public const int Quota = 18;

		public const int SubobjectCleanup = 19;

		public const int FaultInjection = 20;

		public static Guid guid = new Guid("702edbba-c134-43b8-b01d-6aed04823af3");
	}
}
