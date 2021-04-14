using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct AssistantsTags
	{
		public const int AssistantsRpcServer = 0;

		public const int DatabaseInfo = 1;

		public const int DatabaseManager = 2;

		public const int ErrorHandler = 3;

		public const int EventAccess = 4;

		public const int EventController = 5;

		public const int EventDispatcher = 6;

		public const int EventBasedAssistantCollection = 7;

		public const int TimeBasedAssistantController = 8;

		public const int TimeBasedDatabaseDriver = 9;

		public const int TimeBasedDatabaseJob = 10;

		public const int TimeBasedDatabaseWindowJob = 11;

		public const int TimeBasedDatabaseDemandJob = 12;

		public const int TimeBasedDriverManager = 13;

		public const int OnlineDatabase = 14;

		public const int PoisonControl = 15;

		public const int Throttle = 16;

		public const int PFD = 17;

		public const int Governor = 18;

		public const int QueueProcessor = 19;

		public const int FaultInjection = 20;

		public const int ProbeTimeBasedAssistant = 21;

		public static Guid guid = new Guid("EDC33045-05FB-4abb-A608-AEE572BC3C5F");
	}
}
