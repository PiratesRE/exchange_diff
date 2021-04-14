using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct DxStoreTags
	{
		public const int Access = 0;

		public const int Manager = 1;

		public const int Instance = 2;

		public const int PaxosMessage = 3;

		public const int HealthChecker = 4;

		public const int StateMachine = 5;

		public const int Truncator = 6;

		public const int Snapshot = 7;

		public const int LocalStore = 8;

		public const int Utils = 9;

		public const int Config = 10;

		public const int Mesh = 11;

		public const int AccessClient = 12;

		public const int ManagerClient = 13;

		public const int InstanceClient = 14;

		public const int StoreRead = 15;

		public const int StoreWrite = 16;

		public const int AccessEntryPoint = 18;

		public const int ManagerEntryPoint = 19;

		public const int InstanceEntryPoint = 20;

		public const int RunOperation = 21;

		public const int CommitAck = 22;

		public const int EventLogger = 23;

		public static Guid guid = new Guid("{3C3F940E-234C-442E-A30B-A78F146F8C10}");
	}
}
