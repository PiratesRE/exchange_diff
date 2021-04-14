using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct InfoWorker_CommonTags
	{
		public const int SingleInstanceItem = 0;

		public const int MeetingSuggestions = 1;

		public const int WorkingHours = 2;

		public const int OOF = 3;

		public const int ELC = 4;

		public const int ResourceBooking = 5;

		public const int PFD = 6;

		public const int TraceContext = 7;

		public const int AutoTagging = 8;

		public const int Search = 9;

		public const int MWI = 10;

		public const int TopN = 11;

		public const int FaultInjection = 12;

		public const int PublicFolderFreeBusyData = 13;

		public const int UserPhotos = 14;

		public static Guid guid = new Guid("3A8BB7C6-6298-45eb-BE95-1A3AF02F7FFA");
	}
}
