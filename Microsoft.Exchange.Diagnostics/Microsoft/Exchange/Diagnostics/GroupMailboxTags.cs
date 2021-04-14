using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct GroupMailboxTags
	{
		public const int Cmdlets = 0;

		public const int WebServices = 1;

		public const int GroupMailboxAccessLayer = 2;

		public const int LocalAssociationStore = 3;

		public const int MailboxLocator = 4;

		public const int GroupAssociationAdaptor = 5;

		public const int UserAssociationAdaptor = 6;

		public const int UpdateAssociationCommand = 7;

		public const int AssociationReplication = 8;

		public const int AssociationReplicationAssistant = 9;

		public const int GroupEmailNotificationHandler = 10;

		public const int FaultInjection = 11;

		public const int GroupMailboxAssistant = 12;

		public const int UnseenDataUserAssociationAdaptor = 13;

		public static Guid guid = new Guid("902B6BA0-4553-4533-8594-4AD6DA001FB7");
	}
}
