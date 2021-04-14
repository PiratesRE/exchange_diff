using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SearchItemKindType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum SearchItemKind
	{
		Email,
		Meetings,
		Tasks,
		Notes,
		Docs,
		Journals,
		Contacts,
		Im,
		Voicemail,
		Faxes,
		Posts,
		Rssfeeds
	}
}
