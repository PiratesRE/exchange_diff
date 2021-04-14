using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DistinguishedFolderIdNameType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public enum DistinguishedFolderIdName
	{
		none,
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
		internalsubmission,
		fromfavoritesenders,
		clutter,
		favorites,
		unifiedinbox,
		workingset
	}
}
