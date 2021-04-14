using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ElcFolderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Name = "ElcFolderType", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public enum ElcFolderType
	{
		Calendar = 1,
		Contacts,
		DeletedItems,
		Drafts,
		Inbox,
		JunkEmail,
		Journal,
		Notes,
		Outbox,
		SentItems,
		Tasks,
		All,
		ManagedCustomFolder,
		RssSubscriptions,
		SyncIssues,
		ConversationHistory,
		Personal,
		RecoverableItems,
		NonIpmRoot
	}
}
