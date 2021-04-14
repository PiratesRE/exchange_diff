using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public enum DistinguishedFolderIdNameType
	{
		calendar,
		contacts,
		deleteditems,
		drafts,
		inbox,
		journal,
		notes,
		outbox,
		sentitems,
		tasks,
		msgfolderroot,
		publicfoldersroot,
		root,
		junkemail,
		searchfolders,
		voicemail,
		recoverableitemsroot,
		recoverableitemsdeletions,
		recoverableitemsversions,
		recoverableitemspurges,
		archiveroot,
		archivemsgfolderroot,
		archivedeleteditems,
		archiveinbox,
		archiverecoverableitemsroot,
		archiverecoverableitemsdeletions,
		archiverecoverableitemsversions,
		archiverecoverableitemspurges,
		syncissues,
		conflicts,
		localfailures,
		serverfailures,
		recipientcache,
		quickcontacts,
		conversationhistory,
		adminauditlogs,
		todosearch,
		mycontacts,
		directory,
		imcontactlist,
		peopleconnect,
		favorites
	}
}
