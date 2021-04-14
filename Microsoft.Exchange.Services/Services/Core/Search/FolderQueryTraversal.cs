using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Search
{
	[XmlType(TypeName = "FolderQueryTraversalType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum FolderQueryTraversal
	{
		Shallow,
		Deep = 2,
		SoftDeleted = 1
	}
}
