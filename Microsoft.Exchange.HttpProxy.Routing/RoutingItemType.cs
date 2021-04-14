using System;

namespace Microsoft.Exchange.HttpProxy.Routing
{
	public enum RoutingItemType
	{
		ArchiveSmtp,
		DatabaseGuid,
		Error,
		MailboxGuid,
		Server,
		Smtp,
		ExternalDirectoryObjectId,
		LiveIdMemberName,
		Unknown
	}
}
