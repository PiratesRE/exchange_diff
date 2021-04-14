using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_MapiTags
	{
		public const int General = 0;

		public const int SchemaMapEntryAdded = 1;

		public const int SchemaMapEntryUpdated = 2;

		public const int PropertyMapping = 3;

		public const int GetPropsProperties = 4;

		public const int SetPropsProperties = 5;

		public const int DeletePropsProperties = 6;

		public const int CopyOperations = 7;

		public const int StreamOperations = 8;

		public const int AttachmentOperations = 9;

		public const int Notification = 10;

		public const int CreateLogon = 11;

		public const int CreateSession = 12;

		public const int SubmitMessage = 13;

		public const int AccessCheck = 14;

		public const int TimedEvents = 15;

		public const int DeferredSend = 16;

		public const int MailboxSignature = 17;

		public const int Quota = 18;

		public const int FillRow = 19;

		public const int SecurityContextManager = 20;

		public const int InTransitTransitions = 21;

		public const int Restrict = 22;

		public const int FaultInjection = 30;

		public const int EnableBadItemInjection = 31;

		public const int CreateMailbox = 32;

		public static Guid guid = new Guid("7927e3f9-b2bc-461f-96e7-c78d73ed4f04");
	}
}
