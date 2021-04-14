using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct RpcClientAccessTags
	{
		public const int RpcRawBuffer = 0;

		public const int FailedRop = 1;

		public const int RopLevelException = 2;

		public const int NotImplemented = 3;

		public const int NotificationHandler = 4;

		public const int NotificationDelivery = 5;

		public const int Attachment = 6;

		public const int Message = 7;

		public const int FailedRpc = 8;

		public const int ClientThrottled = 9;

		public const int ConnectRpc = 10;

		public const int FaultInjection = 11;

		public const int UnhandledException = 12;

		public const int AsyncRpc = 13;

		public const int AccessControl = 14;

		public const int AsyncRopHandler = 15;

		public const int ConnectXrop = 16;

		public const int FailedXrop = 17;

		public const int Availability = 18;

		public const int Logon = 19;

		public const int Folder = 20;

		public const int ExchangeAsyncDispatch = 21;

		public const int ExchangeDispatch = 22;

		public const int DispatchTask = 23;

		public const int RpcHttpConnectionRegistrationAsyncDispatch = 24;

		public static Guid guid = new Guid("E5EC0B19-2F45-4b2f-8B2B-4B0473209669");
	}
}
