using System;

namespace Microsoft.Exchange.Data.GroupMailbox.Consistency
{
	internal interface IReplicationAssistantInvoker
	{
		bool Invoke(string command, IAssociationAdaptor masterAdaptor, params MailboxAssociation[] associations);
	}
}
