using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_HATags
	{
		public const int Eseback = 0;

		public const int LogReplayStatus = 1;

		public const int BlockModeCollector = 2;

		public const int BlockModeMessageStream = 3;

		public const int BlockModeSender = 4;

		public const int JetHADatabase = 5;

		public const int LastLogWriter = 6;

		public const int FaultInjection = 20;

		public static Guid guid = new Guid("0081576A-CB7C-4aba-9BB4-7D0B44290C2C");
	}
}
