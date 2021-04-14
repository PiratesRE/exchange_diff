using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum ElcFolderType
	{
		Calendar,
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
