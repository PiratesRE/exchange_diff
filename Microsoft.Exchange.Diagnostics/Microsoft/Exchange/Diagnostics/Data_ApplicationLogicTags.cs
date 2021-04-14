using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct Data_ApplicationLogicTags
	{
		public const int FaultInjection = 0;

		public const int FreeBusy = 1;

		public const int Extension = 2;

		public const int PeopleConnectConfiguration = 3;

		public const int MailboxFileStore = 4;

		public const int Cafe = 5;

		public const int DiagnosticHandlers = 6;

		public const int E4E = 7;

		public const int SyncCalendar = 8;

		public static Guid guid = new Guid("A9F57445-AB0E-47ff-90F3-9593E8D23B6F");
	}
}
