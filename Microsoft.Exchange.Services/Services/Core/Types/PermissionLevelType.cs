using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum PermissionLevelType
	{
		None,
		Owner,
		PublishingEditor,
		Editor,
		PublishingAuthor,
		Author,
		NoneditingAuthor,
		Reviewer,
		Contributor,
		Custom
	}
}
