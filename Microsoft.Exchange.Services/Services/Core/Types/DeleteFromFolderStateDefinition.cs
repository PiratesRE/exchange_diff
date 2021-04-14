using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "DeleteFromFolderStateDefinitionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public sealed class DeleteFromFolderStateDefinition : BaseCalendarItemStateDefinition
	{
	}
}
